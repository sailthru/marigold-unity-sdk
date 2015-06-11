package com.carnival.sdk.unitywrapper;

import android.app.Activity;
import android.content.Intent;
import android.location.Location;
import android.text.TextUtils;

import com.carnival.sdk.Carnival;
import com.carnival.sdk.CarnivalStreamActivity;
import com.unity3d.player.UnityPlayer;

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

    public static void startEngine(String projectNumber, String appKey) {
        Activity activity = UnityPlayer.currentActivity;
        Carnival.startEngine(activity, projectNumber, appKey);
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
                UnityPlayer.UnitySendMessage("Carnival", "ReceiveTags", concat);
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
    public static String getDeviceId() {
        return Carnival.getDeviceId();
    }

    public static void logEvent(String value) {
        Carnival.logEvent(value);
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

    public static void setUniqueId(String uniqueId) {
        Carnival.setUniqueId(uniqueId);
    }

    private static class GenericErrorHandler implements Carnival.TagsHandler, Carnival.AttributesHandler {

        @Override
        public void onSuccess() {

        }

        @Override
        public void onSuccess(List<String> tags) {

        }

        @Override
        public void onFailure(Error error) {
            UnityPlayer.UnitySendMessage("Carnival", "ReceiveError", error.getLocalizedMessage());
        }
    }
}
