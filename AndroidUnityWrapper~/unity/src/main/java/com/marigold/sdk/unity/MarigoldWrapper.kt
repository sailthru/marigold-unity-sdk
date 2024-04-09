package com.marigold.sdk.unity

import android.app.Activity
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.location.Location
import android.os.Handler
import android.os.Looper
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import com.marigold.sdk.Marigold
import com.marigold.sdk.MessageStream
import com.marigold.sdk.unity.UnitySender.Companion.MARIGOLD_RECEIVE_DEVICE_ID
import com.marigold.sdk.unity.UnitySender.Companion.MARIGOLD_UNITY
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_RECEIVE_UNREAD_COUNT
import com.marigold.sdk.unity.UnitySender.Companion.MESSAGE_STREAM_UNITY
import com.unity3d.player.UnityPlayer
import org.jetbrains.annotations.VisibleForTesting
import java.lang.reflect.InvocationTargetException

object MarigoldWrapper {
    @VisibleForTesting
    internal var marigold = Marigold()
    @VisibleForTesting
    internal var messageStream = MessageStream()
    @VisibleForTesting
    internal var unitySender = UnitySender()
    @VisibleForTesting
    internal var started = false

    fun start() {
        if (started) return
        started = true

        val activity = UnityPlayer.currentActivity
        setupMessageHandling(activity)

        val broadcastManager = LocalBroadcastManager.getInstance(activity)
        broadcastManager.registerReceiver(object : BroadcastReceiver() {
            override fun onReceive(context: Context, intent: Intent) {
                val count = intent.getIntExtra(MessageStream.EXTRA_UNREAD_MESSAGE_COUNT, 0)
                unitySender.sendUnityMessage(MESSAGE_STREAM_UNITY, MESSAGE_STREAM_RECEIVE_UNREAD_COUNT, count.toString())
            }
        }, IntentFilter(Marigold.ACTION_MESSAGE_COUNT_UPDATE))

        setWrapperInfo()
    }

    fun updateLocation(latitude: Double, longitude: Double) {
        marigold.updateLocation(Location("Unity").apply {
            this.latitude = latitude
            this.longitude = longitude
        })
    }

    fun deviceId() {
        marigold.getDeviceId(object : Marigold.MarigoldHandler<String?> {
            override fun onSuccess(value: String?) {
                unitySender.sendUnityMessage(MARIGOLD_UNITY, MARIGOLD_RECEIVE_DEVICE_ID, value ?: "")
            }
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(MARIGOLD_UNITY, error)
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
                unitySender.sendErrorMessage(MARIGOLD_UNITY, error)
            }
        })
    }

    fun requestNotificationPermission() {
        marigold.requestNotificationPermission(UnityPlayer.currentActivity)
    }

    fun syncNotificationSettings() {
        marigold.syncNotificationSettings()
    }

    //region Private

    private fun setWrapperInfo() {
        try {
            val cArg: Array<Class<String>?> = arrayOfNulls(2)
            cArg[0] = String::class.java
            cArg[1] = String::class.java
            val setWrapperMethod = Marigold.Companion::class.java.getDeclaredMethod("setWrapper", *cArg)
            setWrapperMethod.isAccessible = true
            setWrapperMethod.invoke(Marigold.Companion, "Unity", "2.1.0")
        } catch (e: NoSuchMethodException) {
            e.printStackTrace()
        } catch (e: IllegalAccessException) {
            e.printStackTrace()
        } catch (e: InvocationTargetException) {
            e.printStackTrace()
        }
    }

    /**
     * Workarounds to ensure in-app handling works correctly with Unity activity lifecycle.
     */
    private fun setupMessageHandling(activity: Activity) {
        // Handle app launched
        activity.intent.extras?.getString(MessageStream.EXTRA_MESSAGE_ID)?.let { messageId ->
            MessageStreamWrapper.showMessageDetail(messageId)
        }

        // Handle app opened from background
        marigold.addNotificationTappedListener { _, bundle ->
            bundle.getString(MessageStream.EXTRA_MESSAGE_ID)?.let { messageId ->
                Handler(Looper.getMainLooper()).postDelayed({
                    MessageStreamWrapper.showMessageDetail(messageId)
                }, 1000)
            }
        }

        // Handle in-app tap
        messageStream.setInAppOnClickListener { _, message ->
            MessageStreamWrapper.showMessageDetail(message.messageID)
        }
    }

    //endregion
}