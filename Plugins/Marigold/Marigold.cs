using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;
using MarigoldSimpleJSON;

namespace MarigoldSDK {
	public class Marigold : MonoBehaviour
	{

		#region Externals
		#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern void _start ();

		[DllImport("__Internal")]
		private static extern void _updateLocation (double lat, double lon);

		[DllImport("__Internal")]
		private static extern void _deviceID ();

	#nullable enable
		[DllImport("__Internal")]
		private static extern void _logRegistrationEvent (string? userID);
	#nullable disable

		[DllImport("__Internal")]
		private static extern void _setInAppNotificationsEnabled (bool enabled);

		[DllImport("__Internal")]
		private static extern void _setGeoIpTrackingEnabled (bool enabled);

		[DllImport("__Internal")]
		private static extern void _setGeoIpTrackingDefault (bool enabled);
		
		[DllImport("__Internal")]
		private static extern void _unreadCount ();

		[DllImport("__Internal")]
		private static extern void _messages ();

		[DllImport("__Internal")]
		private static extern void _showMessageDetail (string messageJSON);

		[DllImport("__Internal")]
		private static extern void _dismissMessageDetail ();

		[DllImport("__Internal")]
		private static extern void _registerImpression(string messageJSON, int impressionType);

		[DllImport("__Internal")]
		private static extern void _removeMessage (string messageJSON);

		[DllImport("__Internal")]
		private static extern void _markMessageAsRead (string messageJSON);

		#endif
		#endregion
		
		
		#region Marigold SDK methods
		private const string AndroidMarigoldWrapper = "com.marigold.sdk.unity.MarigoldWrapper";

		/// <summary>
		///  Initialises the Marigold SDK on the Unity side and sets up some callbacks.
		/// </summary>
		public static void Start()
		{
			#if UNITY_IOS
			Marigold._start ();
			#elif UNITY_ANDROID
			CallAndroidStatic("start");
			#endif
		}
			
		/// <summary>
		/// Forward a location to the Marigold SDK. 
		/// This method can be used when you’re already tracking location in your app and you just want to forward your existing calls to the Marigold SDK.
		/// </summary>
		/// <param name="lat">Lat component of the device location.</param>
		/// <param name="lon">Lon component of the device location.</param>
		public static void UpdateLocation(double lat, double lon) {
			#if UNITY_IOS
			Marigold._updateLocation (lat, lon);
			#elif UNITY_ANDROID
			CallAndroidStatic("updateLocation", lat, lon);
			#endif
		}

		/// <summary>
		/// Asyncronously gets the device ID.
		/// String of ID will be called back with an OnDeviceIdReceivedEvent - add a handler to handle this.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
		/// </summary>
		public static void DeviceID () {
			#if UNITY_IOS
			Marigold._deviceID ();
			#elif UNITY_ANDROID
			CallAndroidStatic("deviceId");
			#endif
		}

	#nullable enable
		/// <summary>
		/// Logs a registration event to sign a user in or out. Provide the user ID for login
		/// or a null/empty string for logout.
		/// </summary>
		/// <param name="userID">The string value of the user ID.</param>
		public static void LogRegistrationEvent(string? userID){
			#if UNITY_IOS
			Marigold._logRegistrationEvent(userID);
			#elif UNITY_ANDROID
			CallAndroidStatic("logRegistrationEvent", userID);
			#endif
		}
	#nullable disable

		/// <summary>
		/// Enables or disables the showing of in-app notifications.
		/// </summary>
		/// <param name="enabled">A boolean value indicating whether in-app notfications are enabled</param>
		public static void SetInAppNotificationsEnabled (bool enabled) {
			#if UNITY_IOS
			Marigold._setInAppNotificationsEnabled (enabled);
			#elif UNITY_ANDROID
			CallAndroidStatic("setInAppNotificationsEnabled", enabled);
			#endif
		}

		/// <summary>
		/// Asyncronously enable or disable location tracking based on IP Address. Tracking location tracking
		/// is enabled by default.
		/// Use this method for users who may not want to have their location tracked at all. 
		/// Errors will be called back with an OnErrorEvent - add a handler to handle this.
		/// </summary>
		/// <param name="enabled">A boolean value indicating whether or not to disable location based on IP Address.</param>
		public static void SetGeoIpTrackingEnabled (bool enabled) {
			#if UNITY_IOS
			Marigold._setGeoIpTrackingEnabled (enabled);
			#elif UNITY_ANDROID
			CallAndroidStatic("setGeoIpTrackingEnabled", enabled);
			#endif
		}

		/// <summary>
		/// Set whether location tracking based on IP Address will be enabled or disabled by default when a device is created.
		/// This method must be called before startEngine.
		/// </summary>
		/// <param name="enabled">A boolean value indicating whether or not location based on IP Address should be enabled by default.</param>
		public static void SetGeoIpTrackingDefault (bool enabled) {
			#if UNITY_IOS
			Marigold._setGeoIpTrackingEnabled (enabled);
			#elif UNITY_ANDROID
			CallAndroidStatic("setGeoIpTrackingEnabled", enabled);
			#endif
		}
		
		/// <summary>
		/// Asyncronously gets the unread count for the message stream.
		/// Unread Count will be returned with an OnUnreadCountReceivedEvent - add a handler to handle this. 
		/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
		/// </summary>
		public static void UnreadCount () {
			#if UNITY_IOS
			Marigold._unreadCount ();
			#elif UNITY_ANDROID
			CallAndroidStatic("unreadCount");
			#endif
		}
		
		/// <summary>
		/// Asyncronously returns a list of Messages.
		/// Messages will be returned with the OnMessagesReceivedEvent - add a handler to handle this. 
		/// </summary>
		public static void GetMessages() {
			#if UNITY_IOS
			Marigold._messages ();
			#elif UNITY_ANDROID
			CallAndroidStatic("getMessages");
			#endif
		}

		/// <summary>
		/// Shows the message detail for a given message
		/// </summary>
		/// <param name="message">The message for which you want to see the detail for</param>
		public static void ShowMessageDetail (Message message) {
			#if UNITY_IOS
			Marigold._showMessageDetail (GetJsonForMessage(message).ToString());
			#elif UNITY_ANDROID
			CallAndroidStatic("showMessageDetail", message.messageID);
			#endif
		}

		/// <summary>
		/// Dismisses the message detail. Does nothing on Android.
		/// </summary>
		public static void DismissMessageDetail () {
			#if UNITY_IOS
			Marigold._dismissMessageDetail();
			#elif UNITY_ANDROID
			//Do Nothing
			#endif
		}

		/// <summary>
		/// Registers an impression for a given message.
		/// </summary>
		/// <param name="message">Marigold Message to create the impression on.</param>
		/// <param name="type">The Type of impression to create.</param>
		public static void RegisterImpression(Message message, ImpressionType type) {
			#if UNITY_IOS
			Marigold._registerImpression(GetJsonForMessage(message).ToString(), (int)type);
			#elif UNITY_ANDROID
			CallAndroidStatic("registerMessageImpression", GetJsonForMessage(message).ToString(), (int)type);
			#endif
		}

		/// <summary>
		/// Removes a given message from the user's stream or from a call to GetMessages.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
		/// </summary>
		/// <param name="message">Message the message to be removed.</param>
		public static void RemoveMessage(Message message) {
			#if UNITY_IOS
			Marigold._removeMessage(GetJsonForMessage(message).ToString());
			#elif UNITY_ANDROID
			CallAndroidStatic("removeMessage", GetJsonForMessage(message).ToString());
			#endif
		}

		/// <summary>
		/// Marks a given message as read.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
		/// </summary>
		public static void MarkMessageAsRead (Message message) {
			#if UNITY_IOS
			Marigold._markMessageAsRead(GetJsonForMessage(message).ToString());
			#elif UNITY_ANDROID
			CallAndroidStatic("markMessageAsRead", GetJsonForMessage(message).ToString());
			#endif
		}

		private static void CallAndroidStatic(string methodName, params object[] args) {
			using (AndroidJavaClass androidWrapper = new AndroidJavaClass(AndroidMarigoldWrapper))
			using (AndroidJavaObject instance = androidWrapper.GetStatic<AndroidJavaObject> ("INSTANCE"))
			{
				instance.Call(methodName, args);
			}
		}

		/// <summary>
		/// Used only by the underlying unity plugin code. Do not call.
		/// </summary>
		public void ReceiveError(string errorDescription) {
			ErrorEventArgs args = new ErrorEventArgs ();
			args.ErrorDescription = errorDescription;
			OnErrorEvent.Invoke(this, args);
		}

		public void ReceiveMessagesJSONData(string messagesJSON) {
			List<Message> messages = new List<Message>();
			var jsonMessagesArray = JSON.Parse(messagesJSON);

			for (int i = 0; i < jsonMessagesArray.Count; i++ ) {
				Message m = new Message();
				m.title = jsonMessagesArray[i]["title"];
				m.messageID = jsonMessagesArray[i]["id"];
				m.text = jsonMessagesArray[i]["text"];
				m.URL = jsonMessagesArray[i]["url"];
				m.videoURL = jsonMessagesArray[i]["videoURL"];
				m.imageURL = jsonMessagesArray[i]["imageURL"];
				m.type = (Message.MessageType)jsonMessagesArray[i]["type"].AsInt;
				m.createdAt = jsonMessagesArray[i]["created_at"];
				messages.Add(m);
			}

			MessagesReceivedEventArgs args = new MessagesReceivedEventArgs ();
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

		public static JSONClass GetJsonForMessage (Message message) {
			JSONClass jsonObject = new JSONClass();
			if (message.messageID != null) jsonObject["id"] = message.messageID;
			if (message.title != null) jsonObject["title"] = message.title;
			if (message.text != null) jsonObject["text"] = message.text;
			if (message.createdAt != null) jsonObject["created_at"] = message.createdAt;
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
			DeviceIDReceivedEventArgs args = new DeviceIDReceivedEventArgs ();
			args.DeviceID = deviceID;
			OnDeviceIdReceivedEvent.Invoke(this, args);
		}
		
		/// <summary>
		/// Used only by the underlying unity plugin code. Do not call.
		/// </summary>
		public void ReceiveUnreadCount(string unreadCount) {
			UnreadCountReceivedEventArgs args = new UnreadCountReceivedEventArgs ();
			args.UnreadCount = unreadCount;
			OnUnreadCountReceivedEvent.Invoke(this, args);
		}
		
		#endregion
		
		#region Callbacks
		public static event EventHandler<ErrorEventArgs> OnErrorEvent;
		public static event EventHandler<MessagesReceivedEventArgs> OnMessagesReceivedEvent;
		public static event EventHandler<DeviceIDReceivedEventArgs> OnDeviceIdReceivedEvent;
		public static event EventHandler<UnreadCountReceivedEventArgs> OnUnreadCountReceivedEvent;
		#endregion
	}

	/// <summary>
	/// Marigold error event arguments.
	/// </summary>
	public class ErrorEventArgs :EventArgs {
		public string ErrorDescription { get; set; }
	}


	/// <summary>
	/// Marigold messages received event.
	/// </summary>
	public class MessagesReceivedEventArgs :EventArgs {
		public List<Message> messages { get; set; }
	}

	/// <summary>
	/// Marigold message class.
	/// </summary>
	public class Message {
		// TODO - compare with native values
		public string messageID { get; set; }
		public string title { get; set; }
		public string text { get; set; }
		public string URL { get; set; }
		public string videoURL { get; set; }
		public string imageURL { get; set; }
		public string createdAt { get; set; }
		public MessageType type { get; set; }
		public enum MessageType {Text, Image, Link, Video, FakePhoneCall, Other};
	}

	public enum ImpressionType {InAppNotificationView, StreamView, DetailView};

	/// <summary>
	/// Marigold Device ID received event.
	/// </summary>
	public class DeviceIDReceivedEventArgs :EventArgs {
		public string DeviceID { get; set; }
	}

	/// <summary>
	/// Marigold unread count received event.
	/// </summary>
	public class UnreadCountReceivedEventArgs :EventArgs {
		public string UnreadCount { get; set; }
	}
}