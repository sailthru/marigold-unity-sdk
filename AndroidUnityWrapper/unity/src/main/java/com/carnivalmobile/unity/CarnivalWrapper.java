package com.carnivalmobile.unity;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.location.Location;
import android.support.v4.content.LocalBroadcastManager;
import android.text.TextUtils;

import com.carnival.sdk.Carnival;
import com.carnival.sdk.CarnivalImpressionType;
import com.carnival.sdk.CarnivalStreamActivity;
import com.carnival.sdk.Message;
import com.carnival.sdk.MessageActivity;
import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONObject;

import java.lang.reflect.Constructor;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.List;

/**
 * Created by Affian on 10/06/15.
 *
 */
@SuppressWarnings("unused")
public class CarnivalWrapper {

    private static final String CARNIVAL_UNITY = "Carnival";

    public static void startEngine(String projectNumber, String appKey) {
        Activity activity = UnityPlayer.currentActivity;

        String appPackage = activity.getApplicationContext().getPackageName();
        int resourceId = activity.getResources().getIdentifier("app_icon", "drawable", appPackage);
        if (resourceId != 0) {
            Carnival.setNotificationIcon(resourceId);
        }

        Carnival.startEngine(activity, projectNumber, appKey);

        LocalBroadcastManager broadcastManager = LocalBroadcastManager.getInstance(activity);
        broadcastManager.registerReceiver(new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                int count = intent.getIntExtra(Carnival.EXTRA_UNREAD_MESSAGE_COUNT, 0);
                UnityPlayer.UnitySendMessage(CARNIVAL_UNITY, "ReceiveUnreadCount", String.valueOf(count));
            }
        }, new IntentFilter(Carnival.ACTION_MESSAGE_COUNT_UPDATE));
    }

    public static void openStream() {
        Activity activity = UnityPlayer.currentActivity;
        Intent intent = new Intent(activity, CarnivalStreamActivity.class);
        activity.startActivity(intent);
    }

    public static void getTags() {
        Carnival.getTags(new GenericErrorHandler() {
            @Override
            public void onSuccess(List<String> tags) {
                String concat = TextUtils.join(",", tags);
                UnityPlayer.UnitySendMessage(CARNIVAL_UNITY, "ReceiveTags", concat);
            }
        });
    }

    public static void setTags(String concatList) {
        String[] list = concatList.split(",");
        List<String> tags = new ArrayList<>(Arrays.asList(list));

        Carnival.setTagsWithResponse(tags, new GenericErrorHandler());
    }

    public static void updateLocation(double latitude, double longitude) {
        Location location = new Location("Unity");
        location.setLatitude(latitude);
        location.setLongitude(longitude);

        Carnival.updateLocation(location);
    }

    public static void deviceId() {
        UnityPlayer.UnitySendMessage(CARNIVAL_UNITY, "ReceiveDeviceID", String.valueOf(Carnival.getDeviceId()));
    }

    public static void logEvent(String value) {
        Carnival.logEvent(value);
    }

    public static void getMessages() {
        Carnival.getMessages(new GenericErrorHandler() {
            @Override
            public void onSuccess(ArrayList<Message> messages) {
                JSONArray messagesJson = new JSONArray();
                try {
                    Method toJsonMethod = Message.class.getDeclaredMethod("toJSON");
                    toJsonMethod.setAccessible(true);

                    for (Message message : messages) {
                        JSONObject messageJson = (JSONObject) toJsonMethod.invoke(message);
                        messagesJson.put(messageJson);
                    }
                } catch (Exception e) {
                    UnityPlayer.UnitySendMessage(CARNIVAL_UNITY, "ReceiveError", e.getLocalizedMessage());
                }

                UnityPlayer.UnitySendMessage(CARNIVAL_UNITY, "ReceiveMessagesJSONData", messagesJson.toString());
            }
        });
    }

    public static void setBooleanAttribute (String key, boolean value) {
        Carnival.setAttribute(key, value);
    }

    public static void setIntegerAttribute (String key, int value) {
        Carnival.setAttribute(key, value);
    }

    public static void setFloatAttribute (String key, float value) {
        Carnival.setAttribute(key, value);
    }

    public static void setStringAttribute (String key, String value) {
        Carnival.setAttribute(key, value);
    }

    public static void setDateAttribute (String key, long value) {
        Date date = new Date(value);
        Carnival.setAttribute(key, date, new GenericErrorHandler());
    }

    public static void removeAttribute(String key) {
        Carnival.removeAttribute(key, new GenericErrorHandler());
    }

    public static void setUserId(String userId) {
        Carnival.setUserId(userId, new GenericErrorHandler());
    }

    public static void unreadCount() {
        Carnival.getUnreadMessageCount();
    }

    public static void removeMessage(String messageString) {
        Message message = getMessage(messageString);
        Carnival.deleteMessage(message, new GenericErrorHandler());
    }

    public static void registerMessageImpression(String messageString, int typeCode) {
        Message message = getMessage(messageString);
        CarnivalImpressionType type = null;

        if (typeCode == 0) type = CarnivalImpressionType.IMPRESSION_TYPE_IN_APP_VIEW;
        else if (typeCode == 1) type = CarnivalImpressionType.IMPRESSION_TYPE_STREAM_VIEW;
        else if (typeCode == 2) type = CarnivalImpressionType.IMPRESSION_TYPE_DETAIL_VIEW;

        if (type != null) {
            Carnival.registerMessageImpression(type, message);
        } else {
            UnityPlayer.UnitySendMessage(CARNIVAL_UNITY, "ReceiveError", "Unable to determine Carnival Impression Type: " + typeCode);
        }

    }

    public static void markMessageAsRead(String messageString) {
        Message message = getMessage(messageString);
        Carnival.setMessageRead(message, new GenericErrorHandler());
    }

    private static Message getMessage(String messageString) {
        Message message = null;
        try {
            Constructor<Message> constructor;
            constructor = Message.class.getDeclaredConstructor(String.class);
            constructor.setAccessible(true);
            message = constructor.newInstance(messageString);
        } catch (Exception e) {
            UnityPlayer.UnitySendMessage(CARNIVAL_UNITY, "ReceiveError", e.getLocalizedMessage());
        }
        return message;
    }

    public static void showMessageDetail(String messageId) {
        Intent i = new Intent(UnityPlayer.currentActivity, MessageActivity.class);
        i.putExtra(Carnival.EXTRA_MESSAGE_ID, messageId);
        UnityPlayer.currentActivity.startActivity(i);
    }

    private static class GenericErrorHandler implements Carnival.TagsHandler,
                                                        Carnival.AttributesHandler,
                                                        Carnival.MessagesHandler,
                                                        Carnival.MessageDeletedHandler,
                                                        Carnival.MessagesReadHandler,
                                                        Carnival.CarnivalHandler<Void> {
        @Override
        public void onSuccess() { }

        @Override
        public void onSuccess(List<String> tags) { }

        @Override
        public void onSuccess(ArrayList<Message> arrayList) { }

        @Override
        public void onSuccess(Void aVoid) { }

        @Override
        public void onFailure(Error error) {
            UnityPlayer.UnitySendMessage(CARNIVAL_UNITY, "ReceiveError", error.getLocalizedMessage());
        }
    }
}