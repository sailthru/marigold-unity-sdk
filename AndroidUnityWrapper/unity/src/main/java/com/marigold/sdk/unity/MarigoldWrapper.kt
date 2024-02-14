package com.marigold.sdk.unity

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.location.Location
import android.os.Handler
import android.os.Looper
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import com.marigold.sdk.Marigold
import com.marigold.sdk.MessageActivity
import com.marigold.sdk.MessageStream
import com.marigold.sdk.enums.ImpressionType
import com.marigold.sdk.model.Message
import com.unity3d.player.UnityPlayer
import org.json.JSONArray
import org.json.JSONObject
import java.lang.reflect.Constructor
import java.lang.reflect.InvocationTargetException

/**
 * Created by Affian on 10/06/15.
 *
 */
@Suppress("unused")
object MarigoldWrapper {
    private const val MARIGOLD_UNITY = "Marigold"
    private const val MARIGOLD_RECEIVE_ERROR = "ReceiveError"
    private const val MARIGOLD_RECEIVE_DEVICE_ID = "ReceiveDeviceID"
    private const val MARIGOLD_RECEIVE_UNREAD_COUNT = "ReceiveUnreadCount"
    private const val MARIGOLD_RECEIVE_MESSAGES = "ReceiveMessagesJSONData"
    private val marigold = Marigold()
    private val messageStream = MessageStream()

    init {
        setupMessageHandling()
    }

    //region Marigold
    fun start() {
        val activity = UnityPlayer.currentActivity
        val broadcastManager = LocalBroadcastManager.getInstance(activity)
        broadcastManager.registerReceiver(object : BroadcastReceiver() {
            override fun onReceive(context: Context, intent: Intent) {
                val count = intent.getIntExtra(MessageStream.EXTRA_UNREAD_MESSAGE_COUNT, 0)
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, MARIGOLD_RECEIVE_UNREAD_COUNT, count.toString())
            }
        }, IntentFilter(Marigold.ACTION_MESSAGE_COUNT_UPDATE))
        setWrapperInfo()
        marigold.requestNotificationPermission(activity)
    }

    fun updateLocation(latitude: Double, longitude: Double) {
        val location = Location("Unity")
        location.latitude = latitude
        location.longitude = longitude
        marigold.updateLocation(location)
    }

    fun deviceId() {
        marigold.getDeviceId(object : Marigold.MarigoldHandler<String?> {
            override fun onSuccess(value: String?) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, MARIGOLD_RECEIVE_DEVICE_ID, value)
            }
            override fun onFailure(error: Error) {
                sendErrorMessage(error)
            }
        })
    }

    fun logRegistrationEvent(userId: String?) {
        marigold.logRegistrationEvent(userId)
    }

    fun setInAppNotificationsEnabled(enabled: Boolean) {
        marigold.setInAppNotificationsEnabled(enabled)
    }

    fun setGeoIpTrackingEnabled(enabled: Boolean) {
        marigold.setGeoIpTrackingEnabled(enabled, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                sendErrorMessage(error)
            }
        })
    }

    fun setGeoIpTrackingDefault(enabled: Boolean) {
        marigold.setGeoIpTrackingDefault(enabled)
    }

    //endregion
    //region MessageStream
    fun unreadCount() {
        messageStream.getUnreadMessageCount(object : MessageStream.MessageStreamHandler<Int> {
            override fun onSuccess(value: Int) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, MARIGOLD_RECEIVE_UNREAD_COUNT, value.toString())
            }
            override fun onFailure(error: Error) {
                sendErrorMessage(error)
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
                    sendErrorMessage(e)
                }
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, MARIGOLD_RECEIVE_MESSAGES, messagesJson.toString())
            }

            override fun onFailure(error: Error) {
                sendErrorMessage(error)
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
                sendErrorMessage(Error("Unable to determine Impression Type for: $typeCode"))
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
                sendErrorMessage(error)
            }
        })
    }

    fun markMessageAsRead(messageString: String) {
        val message = getMessage(messageString) ?: return
        messageStream.setMessageRead(message, object : MessageStream.MessagesReadHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                sendErrorMessage(error)
            }
        })
    }

    //endregion

    internal fun sendErrorMessage(error: Throwable) {
        UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, MARIGOLD_RECEIVE_ERROR, error.toString())
    }

    //region Private

    private fun getMessage(messageString: String): Message? = try {
        val constructor: Constructor<Message> = Message::class.java.getDeclaredConstructor(String::class.java)
        constructor.isAccessible = true
        constructor.newInstance(messageString)
    } catch (e: Exception) {
        sendErrorMessage(e)
        null
    }
    private fun setWrapperInfo() {
        try {
            val cArg: Array<Class<String>?> = arrayOfNulls(2)
            cArg[0] = String::class.java
            cArg[1] = String::class.java
            val setWrapperMethod = Marigold.Companion::class.java.getDeclaredMethod("setWrapper", *cArg)
            setWrapperMethod.isAccessible = true
            setWrapperMethod.invoke(Marigold.Companion, "Unity", "1.0.0")
        } catch (e: NoSuchMethodException) {
            e.printStackTrace()
        } catch (e: IllegalAccessException) {
            e.printStackTrace()
        } catch (e: InvocationTargetException) {
            e.printStackTrace()
        }
    }

    /**
     * Workaround to prevent duplicated open metrics when launching push with in-app attached.
     */
    private fun setupMessageHandling() {
        // Handle app launched
        UnityPlayer.currentActivity.intent.extras?.getString(MessageStream.EXTRA_MESSAGE_ID)?.let { messageId ->
            showMessageDetail(messageId)
        }

        // Handle app opened from background
        Marigold().addNotificationTappedListener { _, bundle ->
            bundle.getString(MessageStream.EXTRA_MESSAGE_ID)?.let { messageId ->
                Handler(Looper.getMainLooper()).postDelayed({
                    showMessageDetail(messageId)
                }, 1000)
            }
        }
    }

    //endregion
}