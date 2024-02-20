package com.marigold.sdk.unity

import android.app.Activity
import android.content.Intent
import com.marigold.sdk.MessageActivity
import com.marigold.sdk.MessageStream
import com.marigold.sdk.enums.ImpressionType
import com.marigold.sdk.model.Message
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_RECEIVE_MESSAGES
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_RECEIVE_UNREAD_COUNT
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_UNITY
import com.unity3d.player.UnityPlayer
import org.json.JSONObject
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.ArgumentCaptor
import org.mockito.Captor
import org.mockito.Mock
import org.mockito.MockitoAnnotations
import org.mockito.kotlin.any
import org.mockito.kotlin.capture
import org.mockito.kotlin.doNothing
import org.mockito.kotlin.doReturn
import org.mockito.kotlin.eq
import org.mockito.kotlin.mock
import org.mockito.kotlin.never
import org.mockito.kotlin.stub
import org.mockito.kotlin.verify
import org.mockito.kotlin.verifyNoInteractions
import org.mockito.kotlin.whenever
import org.robolectric.RobolectricTestRunner
import java.lang.Error

@RunWith(RobolectricTestRunner::class)
class MessageStreamWrapperTest {
    @Mock
    private lateinit var messageStream: MessageStream
    @Mock
    private lateinit var unitySender: UnitySender
    @Mock
    private lateinit var activity: Activity

    @Captor
    private lateinit var messageStreamIntHandlerCaptor: ArgumentCaptor<MessageStream.MessageStreamHandler<Int>>
    @Captor
    private lateinit var messagesHandlerCaptor: ArgumentCaptor<MessageStream.MessagesHandler>
    @Captor
    private lateinit var messageDeletedHandlerCaptor: ArgumentCaptor<MessageStream.MessageDeletedHandler>
    @Captor
    private lateinit var messagesReadHandlerCaptor: ArgumentCaptor<MessageStream.MessagesReadHandler>
    @Captor
    private lateinit var runnableCaptor: ArgumentCaptor<Runnable>
    @Captor
    private lateinit var intentCaptor: ArgumentCaptor<Intent>
    @Captor
    private lateinit var errorCaptor: ArgumentCaptor<Error>
    @Captor
    private lateinit var messageCaptor: ArgumentCaptor<Message>
    @Captor
    private lateinit var messagesCaptor: ArgumentCaptor<ArrayList<Message>>

    private val emptyIntent = Intent()
    private val error = Error("Test Error")
    private val messageString = "{\"id\":\"12345\",\"title\":\"test\"}"
    private val messagesString = "[{\"id\":\"12345\",\"title\":\"test\"},{\"id\":\"23456\",\"title\":\"me\"}]"

    @Before
    fun setup() {
        MockitoAnnotations.openMocks(this)
        UnityPlayer.currentActivity = activity
        activity.stub { activity ->
            whenever(activity.intent).thenReturn(emptyIntent)
        }
        MessageStreamWrapper.messageStream = messageStream
        MessageStreamWrapper.unitySender = unitySender
    }

    @After
    fun tearDown() {
        UnityPlayer.currentActivity = null
    }

    @Test
    fun `test unreadCount with success response`() {
        MessageStreamWrapper.unreadCount()
        verify(messageStream).getUnreadMessageCount(capture(messageStreamIntHandlerCaptor))

        val handler = messageStreamIntHandlerCaptor.value
        handler.onSuccess(23)

        verify(unitySender).sendUnityMessage(MESSAGE_STREAM_UNITY, MESSAGE_STREAM_RECEIVE_UNREAD_COUNT, "23")
    }

    @Test
    fun `test unreadCount with error response`() {
        MessageStreamWrapper.unreadCount()
        verify(messageStream).getUnreadMessageCount(capture(messageStreamIntHandlerCaptor))

        val handler = messageStreamIntHandlerCaptor.value

        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(MESSAGE_STREAM_UNITY, error)
    }

    @Test
    fun `test getMessages with success response`() {
        MessageStreamWrapper.getMessages()
        verify(messageStream).getMessages(capture(messagesHandlerCaptor))

        val handler = messagesHandlerCaptor.value
        val message: Message = mock()
        val messageJson = JSONObject(messageString)
        doReturn(messageJson).whenever(message).toJSON()
        val messages = arrayListOf(message)
        handler.onSuccess(messages)

        verify(unitySender).sendUnityMessage(MESSAGE_STREAM_UNITY, MESSAGE_STREAM_RECEIVE_MESSAGES, "[${messageJson}]")
    }

    @Test
    fun `test getMessages with error response`() {
        MessageStreamWrapper.getMessages()
        verify(messageStream).getMessages(capture(messagesHandlerCaptor))

        val handler = messagesHandlerCaptor.value

        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(MESSAGE_STREAM_UNITY, error)
    }

    @Test
    fun `test showMessageDetail`() {
        doNothing().whenever(activity).runOnUiThread(any())
        doNothing().whenever(activity).startActivity(any())
        MessageStreamWrapper.showMessageDetail("message ID")
        verify(activity).runOnUiThread(capture(runnableCaptor))

        val runnable = runnableCaptor.value
        runnable.run()
        verify(activity).startActivity(capture(intentCaptor))

        assertEquals("message ID", intentCaptor.value.extras?.getString(MessageStream.EXTRA_MESSAGE_ID))
        assertEquals(MessageActivity::class.qualifiedName, intentCaptor.value.component?.className)
    }

    @Test
    fun `test registerMessageImpression in-app`() {
        MessageStreamWrapper.registerMessageImpression(messageString, 0)
        verify(messageStream).registerMessageImpression(eq(ImpressionType.IMPRESSION_TYPE_IN_APP_VIEW), any())
    }

    @Test
    fun `test registerMessageImpression stream`() {
        MessageStreamWrapper.registerMessageImpression(messageString, 1)
        verify(messageStream).registerMessageImpression(eq(ImpressionType.IMPRESSION_TYPE_STREAM_VIEW), any())
    }

    @Test
    fun `test registerMessageImpression detail`() {
        MessageStreamWrapper.registerMessageImpression(messageString, 2)
        verify(messageStream).registerMessageImpression(eq(ImpressionType.IMPRESSION_TYPE_DETAIL_VIEW), any())
    }

    @Test
    fun `test registerMessageImpression other`() {
        MessageStreamWrapper.registerMessageImpression(messageString, 3)

        verify(unitySender).sendErrorMessage(eq(MESSAGE_STREAM_UNITY), capture(errorCaptor))
        verify(messageStream, never()).registerMessageImpression(any(), any())

        assertEquals("Unable to determine Impression Type for: 3", errorCaptor.value.localizedMessage)
    }

    @Test
    fun `test removeMessage with success response`() {
        MessageStreamWrapper.removeMessage(messageString)
        verify(messageStream).deleteMessage(capture(messageCaptor), capture(messageDeletedHandlerCaptor))

        val message = messageCaptor.value
        assertEquals("12345", message.messageID)
        assertEquals("test", message.title)

        val handler = messageDeletedHandlerCaptor.value
        handler.onSuccess()

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test removeMessage with error response`() {
        MessageStreamWrapper.removeMessage(messageString)
        verify(messageStream).deleteMessage(capture(messageCaptor), capture(messageDeletedHandlerCaptor))

        val message = messageCaptor.value
        assertEquals("12345", message.messageID)
        assertEquals("test", message.title)

        val handler = messageDeletedHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(MESSAGE_STREAM_UNITY, error)
    }

    @Test
    fun `test markMessageAsRead with success response`() {
        MessageStreamWrapper.markMessageAsRead(messageString)
        verify(messageStream).setMessageRead(capture(messageCaptor), capture(messagesReadHandlerCaptor))

        val message = messageCaptor.value
        assertEquals("12345", message.messageID)
        assertEquals("test", message.title)

        val handler = messagesReadHandlerCaptor.value
        handler.onSuccess()

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test markMessageAsRead with error response`() {
        MessageStreamWrapper.markMessageAsRead(messageString)
        verify(messageStream).setMessageRead(capture(messageCaptor), capture(messagesReadHandlerCaptor))

        val message = messageCaptor.value
        assertEquals("12345", message.messageID)
        assertEquals("test", message.title)

        val handler = messagesReadHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(MESSAGE_STREAM_UNITY, error)
    }

    @Test
    fun `test markMessagesAsRead with success response`() {
        MessageStreamWrapper.markMessagesAsRead(messagesString)
        verify(messageStream).setMessagesRead(capture(messagesCaptor), capture(messagesReadHandlerCaptor))

        val messages = messagesCaptor.value
        val message1 = messages[0]
        assertEquals("12345", message1.messageID)
        assertEquals("test", message1.title)
        val message2 = messages[1]
        assertEquals("23456", message2.messageID)
        assertEquals("me", message2.title)

        val handler = messagesReadHandlerCaptor.value
        handler.onSuccess()

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test markMessagesAsRead with error response`() {
        MessageStreamWrapper.markMessagesAsRead(messagesString)
        verify(messageStream).setMessagesRead(capture(messagesCaptor), capture(messagesReadHandlerCaptor))

        val messages = messagesCaptor.value
        val message1 = messages[0]
        assertEquals("12345", message1.messageID)
        assertEquals("test", message1.title)
        val message2 = messages[1]
        assertEquals("23456", message2.messageID)
        assertEquals("me", message2.title)

        val handler = messagesReadHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(MESSAGE_STREAM_UNITY, error)
    }
}