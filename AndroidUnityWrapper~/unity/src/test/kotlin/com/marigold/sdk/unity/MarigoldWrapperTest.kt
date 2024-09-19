package com.marigold.sdk.unity

import android.app.Activity
import android.content.Intent
import android.location.Location
import androidx.test.core.app.ApplicationProvider
import com.marigold.sdk.Marigold
import com.marigold.sdk.MessageStream
import com.marigold.sdk.interfaces.UnreadMessageCountListener
import com.marigold.sdk.unity.UnitySender.Companion.MARIGOLD_RECEIVE_DEVICE_ID
import com.marigold.sdk.unity.UnitySender.Companion.MARIGOLD_UNITY
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_RECEIVE_UNREAD_COUNT
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_UNITY
import com.unity3d.player.UnityPlayer
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
import org.mockito.kotlin.doReturn
import org.mockito.kotlin.eq
import org.mockito.kotlin.stub
import org.mockito.kotlin.times
import org.mockito.kotlin.verify
import org.mockito.kotlin.verifyNoInteractions
import org.mockito.kotlin.whenever
import org.robolectric.RobolectricTestRunner
import java.lang.Error

@RunWith(RobolectricTestRunner::class)
class MarigoldWrapperTest {
    @Mock
    private lateinit var marigold: Marigold
    @Mock
    private lateinit var messageStream: MessageStream
    @Mock
    private lateinit var unitySender: UnitySender
    @Mock
    private lateinit var activity: Activity

    @Captor
    private lateinit var locationCaptor: ArgumentCaptor<Location>
    @Captor
    private lateinit var marigoldStringHandlerCaptor: ArgumentCaptor<Marigold.MarigoldHandler<String?>>
    @Captor
    private lateinit var marigoldVoidHandlerCaptor: ArgumentCaptor<Marigold.MarigoldHandler<Void?>>
    @Captor
    private lateinit var unreadCountListenerCaptor: ArgumentCaptor<UnreadMessageCountListener>

    private val emptyIntent = Intent()
    private val error = Error("Test Error")

    @Before
    fun setup() {
        MockitoAnnotations.openMocks(this)
        UnityPlayer.currentActivity = activity
        activity.stub { activity ->
            whenever(activity.intent).thenReturn(emptyIntent)
        }
        MarigoldWrapper.marigold = marigold
        MarigoldWrapper.messageStream = messageStream
        MarigoldWrapper.unitySender = unitySender
    }

    @After
    fun tearDown() {
        UnityPlayer.currentActivity = null
        MarigoldWrapper.started = false
    }

    @Test
    fun `test start`() {
        doReturn(ApplicationProvider.getApplicationContext()).whenever(activity).applicationContext
        MarigoldWrapper.start()
        verify(activity).intent
        verify(marigold).addNotificationTappedListener(any())
        verify(messageStream).setInAppOnClickListener(any())
        verify(messageStream).addUnreadMessageCountListener(capture(unreadCountListenerCaptor))

        val unreadMessageCountListener = unreadCountListenerCaptor.value
        unreadMessageCountListener.onUnreadMessageCountUpdated(activity, 21)
        verify(unitySender).sendUnityMessage(MESSAGE_STREAM_UNITY, MESSAGE_STREAM_RECEIVE_UNREAD_COUNT, "21")
    }

    @Test
    fun `test start multiple calls`() {
        doReturn(ApplicationProvider.getApplicationContext()).whenever(activity).applicationContext
        MarigoldWrapper.start()
        MarigoldWrapper.start()
        verify(activity, times(1)).intent
        verify(marigold, times(1)).addNotificationTappedListener(any())
        verify(messageStream, times(1)).setInAppOnClickListener(any())
        verify(messageStream, times(1)).addUnreadMessageCountListener(any())
    }

    @Test
    fun `test updateLocation`() {
        MarigoldWrapper.updateLocation(5.1, 6.2)
        verify(marigold).updateLocation(capture(locationCaptor))

        assertEquals("Unity", locationCaptor.value.provider)
        assertEquals(5.1, locationCaptor.value.latitude, 0.0)
        assertEquals(6.2, locationCaptor.value.longitude, 0.0)
    }

    @Test
    fun `test deviceId with success response`() {
        MarigoldWrapper.deviceId()
        verify(marigold).getDeviceId(capture(marigoldStringHandlerCaptor))

        val handler = marigoldStringHandlerCaptor.value
        handler.onSuccess("test ID")

        verify(unitySender).sendUnityMessage(MARIGOLD_UNITY, MARIGOLD_RECEIVE_DEVICE_ID, "test ID")
    }

    @Test
    fun `test deviceId with error response`() {
        MarigoldWrapper.deviceId()
        verify(marigold).getDeviceId(capture(marigoldStringHandlerCaptor))

        val handler = marigoldStringHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(MARIGOLD_UNITY, error)
    }

    @Test
    fun `test logRegistrationEvent`() {
        MarigoldWrapper.logRegistrationEvent("test me")
        verify(marigold).logRegistrationEvent("test me")
    }

    @Test
    fun `test setInAppNotificationsEnabled`() {
        MarigoldWrapper.setInAppNotificationsEnabled(true)
        verify(marigold).setInAppNotificationsEnabled(true)
    }

    @Test
    fun `test setGeoIpTrackingEnabled with success response`() {
        MarigoldWrapper.setGeoIpTrackingEnabled(true)
        verify(marigold).setGeoIpTrackingEnabled(eq(true), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test setGeoIpTrackingEnabled with error response`() {
        MarigoldWrapper.setGeoIpTrackingEnabled(true)
        verify(marigold).setGeoIpTrackingEnabled(eq(true), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(MARIGOLD_UNITY, error)
    }

    @Test
    fun `test requestNotificationPermission`() {
        MarigoldWrapper.requestNotificationPermission()
        verify(marigold).requestNotificationPermission(activity)
    }

    @Test
    fun `test syncNotificationSettings`() {
        MarigoldWrapper.syncNotificationSettings()
        verify(marigold).syncNotificationSettings()
    }
}