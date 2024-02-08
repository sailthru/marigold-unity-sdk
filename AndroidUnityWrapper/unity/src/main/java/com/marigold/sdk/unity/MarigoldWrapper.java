package com.marigold.sdk.unity;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.location.Location;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.marigold.sdk.Marigold;
import com.marigold.sdk.MessageActivity;
import com.marigold.sdk.MessageStream;
import com.marigold.sdk.enums.ImpressionType;
import com.marigold.sdk.model.Message;
import com.unity3d.player.UnityPlayer;

import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;
import org.json.JSONArray;
import org.json.JSONObject;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.ArrayList;

/**
 * Created by Affian on 10/06/15.
 *
 */
@SuppressWarnings("unused, unchecked")
public class MarigoldWrapper {

    private static final String MARIGOLD_UNITY = "Marigold";

    private static final Marigold marigold = new Marigold();
    private static final MessageStream messageStream = new MessageStream();

    //region Marigold

    public static void startEngine(String appKey) {
        Activity activity = UnityPlayer.currentActivity;

        marigold.startEngine(activity, appKey);

        LocalBroadcastManager broadcastManager = LocalBroadcastManager.getInstance(activity);
        broadcastManager.registerReceiver(new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                int count = intent.getIntExtra(MessageStream.EXTRA_UNREAD_MESSAGE_COUNT, 0);
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveUnreadCount", String.valueOf(count));
            }
        }, new IntentFilter(Marigold.ACTION_MESSAGE_COUNT_UPDATE));
        setWrapperInfo();
    }

    public static void updateLocation(double latitude, double longitude) {
        Location location = new Location("Unity");
        location.setLatitude(latitude);
        location.setLongitude(longitude);

        marigold.updateLocation(location);
    }

    public static void deviceId() {
        marigold.getDeviceId(new Marigold.MarigoldHandler<String>() {
            @Override
            public void onSuccess(String deviceId) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveDeviceID", String.valueOf(deviceId));
            }

            @Override
            public void onFailure(@NotNull Error error) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", error.getLocalizedMessage());
            }
        });
    }

    public static void logRegistrationEvent(@Nullable String userId) {
        marigold.logRegistrationEvent(userId);
    }

    public static void setInAppNotificationsEnabled(boolean enabled) {
        marigold.setInAppNotificationsEnabled(enabled);
    }

    public static void setGeoIpTrackingEnabled(boolean enabled) {
        marigold.setGeoIpTrackingEnabled(enabled, new Marigold.MarigoldHandler<Void>() {
            @Override
            public void onSuccess(Void unused) {}

            @Override
            public void onFailure(@NotNull Error error) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", error.getLocalizedMessage());
            }
        });
    }

    public static void setGeoIpTrackingDefault(boolean enabled) {
        marigold.setGeoIpTrackingDefault(enabled);
    }

    //endregion

    //region MessageStream

    public static void getMessages() {
        messageStream.getMessages(new MessageStream.MessagesHandler() {
            @Override
            public void onSuccess(@NotNull ArrayList<Message> messages) {
                JSONArray messagesJson = new JSONArray();
                try {
                    Method toJsonMethod = Message.class.getDeclaredMethod("toJSON");
                    toJsonMethod.setAccessible(true);

                    for (Message message : messages) {
                        JSONObject messageJson = (JSONObject) toJsonMethod.invoke(message);
                        messagesJson.put(messageJson);
                    }
                } catch (Exception e) {
                    UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", e.getLocalizedMessage());
                }

                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveMessagesJSONData", messagesJson.toString());
            }

            @Override
            public void onFailure(@NotNull Error error) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", error.getLocalizedMessage());
            }
        });
    }

    public static void unreadCount() {
        messageStream.getUnreadMessageCount(new MessageStream.MessageStreamHandler<Integer>() {
            @Override
            public void onSuccess(Integer unreadCount) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveUnreadCount", String.valueOf(unreadCount));
            }

            @Override
            public void onFailure(@NotNull Error error) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", error.getLocalizedMessage());
            }
        });
    }

    public static void removeMessage(@NotNull String messageString) {
        Message message = getMessage(messageString);
        messageStream.deleteMessage(message, new MessageStream.MessageDeletedHandler() {
            @Override
            public void onSuccess() {}

            @Override
            public void onFailure(@NotNull Error error) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", error.getLocalizedMessage());
            }
        });
    }

    public static void registerMessageImpression(String messageString, int typeCode) {
        Message message = getMessage(messageString);
        ImpressionType type = null;

        if (typeCode == 0) type = ImpressionType.IMPRESSION_TYPE_IN_APP_VIEW;
        else if (typeCode == 1) type = ImpressionType.IMPRESSION_TYPE_STREAM_VIEW;
        else if (typeCode == 2) type = ImpressionType.IMPRESSION_TYPE_DETAIL_VIEW;

        if (type != null) {
            messageStream.registerMessageImpression(type, message);
        } else {
            UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", "Unable to determine Carnival Impression Type: " + typeCode);
        }

    }

    public static void markMessageAsRead(String messageString) {
        Message message = getMessage(messageString);
        messageStream.setMessageRead(message, new MessageStream.MessagesReadHandler() {
            @Override
            public void onSuccess() {}

            @Override
            public void onFailure(@NotNull Error error) {
                UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", error.getLocalizedMessage());
            }
        });
    }

    private static Message getMessage(String messageString) {
        Message message = null;
        try {
            Constructor<Message> constructor;
            constructor = Message.class.getDeclaredConstructor(String.class);
            constructor.setAccessible(true);
            message = constructor.newInstance(messageString);
        } catch (Exception e) {
            UnityPlayer.UnitySendMessage(MARIGOLD_UNITY, "ReceiveError", e.getLocalizedMessage());
        }
        return message;
    }

    public static void showMessageDetail(String messageId) {
        Intent i = new Intent(UnityPlayer.currentActivity, MessageActivity.class);
        i.putExtra(MessageStream.EXTRA_MESSAGE_ID, messageId);
        UnityPlayer.currentActivity.startActivity(i);
    }

    //endregion

    //region Private

    private static void setWrapperInfo(){
        Method setWrapperMethod;
        try {
            Class<String>[] cArg = new Class[2];
            cArg[0] = String.class;
            cArg[1] = String.class;

            setWrapperMethod = Marigold.Companion.class.getDeclaredMethod("setWrapper", cArg);
            setWrapperMethod.setAccessible(true);
            setWrapperMethod.invoke(null, "Unity", "1.0.0");
        } catch (NoSuchMethodException | IllegalAccessException | InvocationTargetException e) {
            e.printStackTrace();
        }
    }

    //endregion
}