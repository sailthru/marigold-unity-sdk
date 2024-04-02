package com.marigold.sdk.unity

import com.marigold.sdk.MessageActivity
import com.marigold.sdk.MessageStream
import com.marigold.sdk.enums.ImpressionType
import com.marigold.sdk.model.Message
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_RECEIVE_MESSAGES
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_RECEIVE_UNREAD_COUNT
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_UNITY
import com.unity3d.player.UnityPlayer
import org.jetbrains.annotations.VisibleForTesting
import org.json.JSONArray
import org.json.JSONObject
import java.lang.reflect.Constructor

object MessageStreamWrapper {
    @VisibleForTesting
    internal var messageStream = MessageStream()
    @VisibleForTesting
    internal var unitySender = UnitySender()

    fun unreadCount() {
        messageStream.getUnreadMessageCount(object : MessageStream.MessageStreamHandler<Int> {
            override fun onSuccess(value: Int) {
                unitySender.sendUnityMessage(MESSAGE_STREAM_UNITY, MESSAGE_STREAM_RECEIVE_UNREAD_COUNT, value.toString())
            }
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(MESSAGE_STREAM_UNITY, error)
            }
        })
    }

    fun getMessages() {
        messageStream.getMessages(object : MessageStream.MessagesHandler {
            override fun onSuccess(messages: ArrayList<Message>) {
                val messagesJson = JSONArray()
                try {
                    val toJsonMethod = Message::class.java.getDeclaredMethod("toJSON")
                    toJsonMethod.isAccessible = true
                    for (message in messages) {
                        val messageJson = toJsonMethod.invoke(message) as JSONObject
                        messagesJson.put(messageJson)
                    }
                } catch (e: Exception) {
                    unitySender.sendErrorMessage(MESSAGE_STREAM_UNITY, e)
                }
                unitySender.sendUnityMessage(MESSAGE_STREAM_UNITY, MESSAGE_STREAM_RECEIVE_MESSAGES, messagesJson.toString())
            }

            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(MESSAGE_STREAM_UNITY, error)
            }
        })
    }

    fun showMessageDetail(messageId: String) {
        UnityPlayer.currentActivity.runOnUiThread {
            val intent = MessageActivity.intentForMessage(UnityPlayer.currentActivity, null, messageId)
            UnityPlayer.currentActivity.startActivity(intent)
        }
    }

    fun registerMessageImpression(messageString: String, typeCode: Int) {
        val message = getMessage(messageString) ?: return
        val type: ImpressionType = when (typeCode) {
            0 -> ImpressionType.IMPRESSION_TYPE_IN_APP_VIEW
            1 -> ImpressionType.IMPRESSION_TYPE_STREAM_VIEW
            2 -> ImpressionType.IMPRESSION_TYPE_DETAIL_VIEW
            else -> {
                unitySender.sendErrorMessage(MESSAGE_STREAM_UNITY, Error("Unable to determine Impression Type for: $typeCode"))
                return
            }
        }
        messageStream.registerMessageImpression(type, message)
    }

    fun removeMessage(messageString: String) {
        val message = getMessage(messageString) ?: return
        messageStream.deleteMessage(message, object : MessageStream.MessageDeletedHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(MESSAGE_STREAM_UNITY, error)
            }
        })
    }

    fun markMessageAsRead(messageString: String) {
        val message = getMessage(messageString) ?: return
        messageStream.setMessageRead(message, object : MessageStream.MessagesReadHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(MESSAGE_STREAM_UNITY, error)
            }
        })
    }

    fun markMessagesAsRead(messagesString: String) {
        val messages = getMessageListFromJSONArrayString(messagesString) ?: return
        messageStream.setMessagesRead(messages, object : MessageStream.MessagesReadHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(MESSAGE_STREAM_UNITY, error)
            }
        })
    }

    // region Private

    private fun getMessageListFromJSONArrayString(arrayString: String?): List<Message>? {
        arrayString ?: return null

        return try {
            val list = mutableListOf<Message>()
            val jsonArray = JSONArray(arrayString)
            for (i in 0 until jsonArray.length()) {
                val message = getMessage(jsonArray.getString(i)) ?: continue
                list.add(message)
            }
            list
        } catch (e: Exception) {
            null
        }
    }

    private fun getMessage(messageString: String): Message? = try {
        val constructor: Constructor<Message> = Message::class.java.getDeclaredConstructor(String::class.java)
        constructor.isAccessible = true
        constructor.newInstance(messageString)
    } catch (e: Exception) {
        MarigoldWrapper.unitySender.sendErrorMessage(MESSAGE_STREAM_UNITY, e)
        null
    }

    // endregion
}