using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;
using CarnivalSimpleJSON;

public class Carnival : MonoBehaviour
{
	#region Externals
	#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void _startEngine (string apiKey);
	
	[DllImport("__Internal")]
	private static extern void _setTags (string tags);
	
	[DllImport("__Internal")]
	private static extern void _getTags ();
	
	[DllImport("__Internal")]
	private static extern void _updateLocation (double lat, double lon);
	
	[DllImport("__Internal")]
	private static extern void _logEvent (string eventName);
	
	[DllImport("__Internal")]
	private static extern void _setString (string stringValue, string key);
	
	[DllImport("__Internal")]
	private static extern void _setBool (bool boolValue, string key);
	
	[DllImport("__Internal")]
	private static extern void _setDate (Int64 date, string key);
	
	[DllImport("__Internal")]
	private static extern void _setFloat (float floatValue, string key);
	
	[DllImport("__Internal")]
	private static extern void _setInteger (int integerValue, string key);
	
	[DllImport("__Internal")]
	private static extern void _removeAttribute (string key);

	[DllImport("__Internal")]
	private static extern void _setInAppNotificationsEnabled (bool enabled);

	[DllImport("__Internal")]
	private static extern void _setUserID (string userID);

	[DllImport("__Internal")]
	private static extern void _messages ();

	[DllImport("__Internal")]
	private static extern void _showMessageDetail (string messageJSON);

	[DllImport("__Internal")]
	private static extern void _dismissMessageDetail ();

	[DllImport("__Internal")]
	private static extern void _markMessageAsRead (string messageJSON);

	[DllImport("__Internal")]
	private static extern void _removeMessage (string messageJSON);

	[DllImport("__Internal")]
	private static extern void _registerImpression(string messageJSON, int impressionType);

	[DllImport("__Internal")]
	private static extern void _deviceID ();
	
	[DllImport("__Internal")]
	private static extern void _unreadCount ();

	#endif
	#endregion
	
	
	#region Carnival SDK methods
	/// <summary>
	///  Sets the Carnival appKey credentials for this app. 
	///  This MUST be done before calling any other Carnival methods.
	///  Call this method if you plan to target iOS.  
	/// </summary>
	/// <param name="apiKey">The api key you received when setting up your iOS app at http://app.carnivalmobile.com </param>
	public static void StartEngineIOS(string apiKey)
	{
		#if UNITY_IOS
		Carnival._startEngine (apiKey);
		#endif
	}
	/// <summary>
	///  Sets the Carnival appKey credentials for this app. 
	///  This MUST be done before calling any other Carnival methods.
	///  Call this method if you plan to target Android.  
	/// </summary>
	/// <param name="apiKey">The api key you received when setting up your Android app at http://app.carnivalmobile.com </param>
	/// <param name="googleProjectNumber">The Project Number from your GCM dashboard </param>  
	public static void StartEngineAndroid(string apiKey, string googleProjectNumber)
	{
		#if UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("startEngine", googleProjectNumber, apiKey);
		#endif
	}
	
	/// <summary>
	/// Asyncronously sets the tags for Carnival for this Device.
	/// Calling this method will overwrite any previously set tags for this Device.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	/// <param name="tags"> A list of tags to set for the device. An empty list will clear the tags for this device.</param>
	public static void SetTags(string[] tags)
	{
		#if UNITY_IOS
		Carnival._setTags(string.Join(",", tags));
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("setTags", string.Join(",", tags));
		#endif
	}
	
	/// <summary>
	/// Asyncronously gets the tags for Carnival for this Device.
	/// Tags will be returned with an OnTagsReceivedEvent - add a handler to handle this. 
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	public static void GetTags()
	{
		#if UNITY_IOS
		Carnival._getTags();
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("getTags");
		#endif
	}
	
	/// <summary>
	/// Forward a location to the Carnival iOS SDK. 
	/// This method can be used when you’re already tracking location in your app and you just want to forward your existing calls to the Carnival iOS SDK.
	/// </summary>
	/// <param name="lat">Lat component of the device location.</param>
	/// <param name="lon">Lon component of the device location.</param>
	public static void UpdateLocation(double lat, double lon) {
		#if UNITY_IOS
		Carnival._updateLocation (lat, lon);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("updateLocation", lat, lon);
		#endif
	}
	
	/// <summary>
	/// Logs a custom event with the given name
	/// </summary>
	/// <param name="eventName">The name of the custom event to be logged.</param>
	public static void LogEvent(string eventName) {
		#if UNITY_IOS
		Carnival._logEvent (eventName);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("logEvent", eventName);
		#endif
	}
	
	/// <summary>
	/// Asyncronously sets a string value for a given key.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	/// <param name="stringValue">The string value to be set. </param>
	/// <param name="key">The string value of the key. </param>
	public static void SetString (string stringValue, string key) {
		#if UNITY_IOS
		Carnival._setString (stringValue, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("setStringAttribute", key, stringValue);
		#endif
	}
	
	/// <summary>
	/// Asyncronously sets a boolean value for a given key.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	/// <param name="stringValue">The boolean value to be set. </param>
	/// <param name="key">The string value of the key. </param>
	public static void SetBool (bool boolValue, string key) {
		#if UNITY_IOS
		Carnival._setBool (boolValue, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("setBooleanAttribute", key, boolValue);
		#endif
	}
	
	/// <summary>
	/// Asyncronously sets a date value for a given key.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	/// <param name="stringValue">The date value to be set. </param>
	/// <param name="key">The string value of the key. </param>
	public static void SetDate (DateTime date , string key) {
		Int64 unixTimestamp = (Int64)((date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
		#if UNITY_IOS
		Carnival._setDate (unixTimestamp, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("setDateAttribute", key, unixTimestamp);
		#endif
	}
	
	/// <summary>
	/// Asyncronously sets a float value for a given key.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	/// <param name="stringValue">The float value to be set. </param>
	/// <param name="key">The string value of the key. </param>
	public static void SetFloat (float floatValue, string key) {
		#if UNITY_IOS
		Carnival._setFloat (floatValue, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("setFloatAttribute", key, floatValue);
		#endif
	}
	
	/// <summary>
	/// Asyncronously sets a integer value for a given key.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	/// <param name="stringValue">The integer value to be set. </param>
	/// <param name="key">The string value of the key. </param>
	public static void SetInteger (int integerValue, string key) {
		#if UNITY_IOS
		Carnival._setInteger (integerValue, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("setIntegerAttribute", key, integerValue);
		#endif
	}
	
	/// <summary>
	/// Asyncronously removes a value for a given key.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	/// <param name="key">The string value of the key. </param>
	public static void RemoveAttribute (string key) {
		#if UNITY_IOS
		Carnival._removeAttribute (key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("removeAttribute", key);
		#endif
	}

	/// <summary>
	/// Asyncronously gets the device ID.
	/// String of ID will be called back with an OnDeviceIdReceivedEvent - add a handler to handle this.
    /// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	public static void DeviceID () {
		#if UNITY_IOS
		Carnival._deviceID ();
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("deviceId");
		#endif
	}
	
	/// <summary>
	/// Asyncronously gets the unread count for the message stream.
	/// Unread Count will be returned with an OnUnreadCountReceivedEvent - add a handler to handle this. 
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	public static void UnreadCount () {
		#if UNITY_IOS
		Carnival._unreadCount ();
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("unreadCount");
		#endif
	}
	
	/// <summary>
	/// Asyncronously returns a list of Messages.
	/// Messages will be returned with the OnMessagesReceivedEvent - add a handler to handle this. 
	/// </summary>
	public static void GetMessages() {
		#if UNITY_IOS
		Carnival._messages ();
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("getMessages");
		#endif
	}

	/// <summary>
	/// Shows the message detail for a given message
	/// </summary>
	/// <param name="message">The message for which you want to see the detail for</param>
	public static void ShowMessageDetail (CarnivalMessage message) {
		#if UNITY_IOS
		Carnival._showMessageDetail (GetJsonForMessage(message).ToString());
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("showMessageDetail", message.messageID);
		#endif
	}

	/// <summary>
	/// Dismisses the message detail. Does nothing on Android.
	/// </summary>
	public static void DismissMessageDetail () {
		#if UNITY_IOS
		Carnival._dismissMessageDetail();
		#elif UNITY_ANDROID
		//Do Nothing
		#endif
	}

	/// <summary>
	/// Asyncronously sets the user ID 
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	/// <param name="userID">The string value of the user ID.</param>
	public static void SetUserID(string userID){
		#if UNITY_IOS
		Carnival._setUserID(userID);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("setUserId", userID);
		#endif
	}

	/// <summary>
	/// Registers an impression for a given message.
	/// </summary>
	/// <param name="message">Carnival Message to create the impression on.</param>
	/// <param name="type">The Type of impression to create.</param>
	public static void RegisterImpression(CarnivalMessage message, CarnivalImpressionType type) {
		#if UNITY_IOS
		Carnival._registerImpression(GetJsonForMessage(message).ToString(), (int)type);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("registerMessageImpression", GetJsonForMessage(message).ToString(), (int)type);
		#endif
	}

	/// <summary>
	/// Removes a given message from the user's stream or from a call to GetMessages.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
	/// </summary>
	/// <param name="message">Message the message to be removed.</param>
	public static void RemoveMessage(CarnivalMessage message) {
		#if UNITY_IOS
		Carnival._removeMessage(GetJsonForMessage(message).ToString());
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("removeMessage", GetJsonForMessage(message).ToString());
		#endif
	}

	/// <summary>
	/// Marks a given message as read.
	/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
	/// </summary>
	public static void MarkMessageAsRead (CarnivalMessage message) {
		#if UNITY_IOS
		Carnival._markMessageAsRead(GetJsonForMessage(message).ToString());
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("markMessageAsRead", GetJsonForMessage(message).ToString());
		#endif
	}

	/// <summary>
	/// Enables or disables the showing of in-app notifications.
	/// </summary>
	/// <param name="enabled">A boolean value indicating whether in-app notfications are enabled</param>
	public static void SetInAppNotificationsEnabled (bool enabled) {
		#if UNITY_IOS
		Carnival._setInAppNotificationsEnabled (enabled);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnivalmobile.unity.CarnivalWrapper");
		_plugin.CallStatic("setInAppNotificationsEnabled", enabled);
		#endif
	}

	/// <summary>
	/// Used only by the underlying unity plugin code. Do not call.
	/// </summary>
	public void ReceiveError(string errorDescription) {
		CarnivalErrorEventArgs args = new CarnivalErrorEventArgs ();
		args.ErrorDescription = errorDescription;
		OnErrorEvent(this, args);
	}
	
	/// <summary>
	/// Used only by the underlying unity plugin code. Do not call.
	/// </summary>
	public void ReceiveTags(string tags) {
		CarnivalTagsReceivedEvent args = new CarnivalTagsReceivedEvent ();
		args.Tags = tags.Split (',');
		OnTagsReceivedEvent (this, args);
	}

	public void ReceiveMessagesJSONData(string messagesJSON) {
		List<CarnivalMessage> messages = new List<CarnivalMessage>();
		var jsonMessagesArray = JSON.Parse(messagesJSON);

		for (int i = 0; i < jsonMessagesArray.Count; i++ ) {
			CarnivalMessage m = new CarnivalMessage();
			m.title = jsonMessagesArray[i]["title"];
			m.messageID = jsonMessagesArray[i]["id"];
			m.text = jsonMessagesArray[i]["text"];
			m.URL = jsonMessagesArray[i]["url"];
			m.videoURL = jsonMessagesArray[i]["videoURL"];
			m.imageURL = jsonMessagesArray[i]["imageURL"];
			m.type = (CarnivalMessage.CarnivalMessageType)jsonMessagesArray[i]["type"].AsInt;
			m.createdAt = UnixTimeStampToDateTime(jsonMessagesArray[i]["created_at"].AsDouble);
			messages.Add(m);
		}

		CarnivalMessagesReceivedEvent args = new CarnivalMessagesReceivedEvent ();
		args.messages = messages;
		OnMessagesReceivedEvent(this, args);
	}
	#endregion

	#region Helpers 
	public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
	{
		// Unix timestamp is seconds past epoch
		System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
		return dtDateTime;
	}

	public static JSONClass GetJsonForMessage (CarnivalMessage message) {
		JSONClass jsonObject = new JSONClass();
		if (message.messageID != null) jsonObject["id"] = message.messageID;
		if (message.title != null) jsonObject["title"] = message.title;
		if (message.text != null) jsonObject["text"] = message.text;
		if (message.createdAt != null) jsonObject["createdAt"].AsDouble = (message.createdAt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		if (message.URL != null) jsonObject["url"] = message.URL;
		if (message.videoURL != null) jsonObject["card_media_url"] = message.videoURL;
		if (message.imageURL != null) jsonObject["card_image_url"] = message.imageURL;
		jsonObject["notifications"] = new JSONArray();
		return jsonObject;
	}

	/// <summary>
	/// Used only by the underlying unity plugin code. Do not call.
	/// </summary>
	public void ReceiveDeviceID(string deviceID) {
		CarnivalDeviceIDReceivedEvent args = new CarnivalDeviceIDReceivedEvent ();
		args.DeviceID = deviceID;
		OnDeviceIdReceivedEvent (this, args);
	}
	
	/// <summary>
	/// Used only by the underlying unity plugin code. Do not call.
	/// </summary>
	public void ReceiveUnreadCount(string unreadCount) {
		CarnivalUnreadCountReceivedEvent args = new CarnivalUnreadCountReceivedEvent ();
		args.UnreadCount = unreadCount;
		OnUnreadCountReceivedEvent (this, args);
	}
	
	#endregion
	
	#region Callbabcks
	public static event EventHandler<CarnivalErrorEventArgs> OnErrorEvent;
	public static event EventHandler<CarnivalTagsReceivedEvent> OnTagsReceivedEvent;
	public static event EventHandler<CarnivalMessagesReceivedEvent> OnMessagesReceivedEvent;
	public static event EventHandler<CarnivalDeviceIDReceivedEvent> OnDeviceIdReceivedEvent;
	public static event EventHandler<CarnivalUnreadCountReceivedEvent> OnUnreadCountReceivedEvent;
	#endregion
}

/// <summary>
/// Carnival error event arguments.
/// </summary>
public class CarnivalErrorEventArgs :EventArgs {
	public string ErrorDescription { get; set; }
}

/// <summary>
/// Carnival tags received event.
/// </summary>
public class CarnivalTagsReceivedEvent :EventArgs {
	public string[] Tags { get; set; }
}

/// <summary>
/// Carnival messages received event.
/// </summary>
public class CarnivalMessagesReceivedEvent :EventArgs {
	public List<CarnivalMessage> messages { get; set; }
}

/// <summary>
/// Carnival message class.
/// </summary>
public class CarnivalMessage {
	public string messageID { get; set; }
	public string title { get; set; }
	public string text { get; set; }
	public string URL { get; set; }
	public string videoURL { get; set; }
	public string imageURL { get; set; }
	public DateTime createdAt { get; set; }
	public CarnivalMessageType type { get; set; }
	public enum CarnivalMessageType {Text, Image, Link, Video, FakePhoneCall, Other};
}

public enum CarnivalImpressionType {InAppNotificationView, StreamView, DetailView};

/// <summary>
/// Carnival tags received event.
/// </summary>
public class CarnivalDeviceIDReceivedEvent :EventArgs {
	public string DeviceID { get; set; }
}

/// <summary>
/// Carnival unread count received event.
/// </summary>
public class CarnivalUnreadCountReceivedEvent :EventArgs {
	public string UnreadCount { get; set; }
}