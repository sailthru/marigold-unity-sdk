package com.marigold.sdk.unity

import com.unity3d.player.UnityPlayer

internal class UnitySender {
    fun sendUnityMessage(receiver: String, method: String, message: String) {
        UnityPlayer.UnitySendMessage(receiver, method, message)
    }

     fun sendErrorMessage(receiver: String, error: Throwable) {
        UnityPlayer.UnitySendMessage(receiver, MARIGOLD_RECEIVE_ERROR, error.toString())
    }

    companion object {
        // Receivers
        internal const val MARIGOLD_UNITY = "Marigold"
        internal const val ENGAGE_ST_UNITY = "EngageBySailthru"
        internal const val MESSAGE_STREAM_UNITY = "MessageStream"

        // Marigold methods
        internal const val MARIGOLD_RECEIVE_ERROR = "ReceiveError"
        internal const val MARIGOLD_RECEIVE_DEVICE_ID = "ReceiveDeviceID"

        // EngageBySailthru methods
        internal const val ENGAGE_ST_RECEIVE_VARS = "ReceiveProfileVars"
        internal const val ENGAGE_ST_RECEIVE_LINK = "ReceiveUnwrappedLink"

        // Message Stream methods
        internal const val MESSAGE_STREAM_RECEIVE_UNREAD_COUNT = "ReceiveUnreadCount"
        internal const val MESSAGE_STREAM_RECEIVE_MESSAGES = "ReceiveMessagesJSONData"
    }
}