using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;
using MarigoldSimpleJSON;

namespace MarigoldSDK {
	public class MessageStream : MonoBehaviour
	{
        
		#region Externals
		#if UNITY_IOS

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

		[DllImport("__Internal")]
		private static extern void _markMessagesAsRead (string messagesJSON);

		#endif
		#endregion
		
		
		#region MessageStream SDK methods
		private const string AndroidMessageStreamWrapper = "com.marigold.sdk.unity.MessageStreamWrapper";

		/// <summary>
		/// Asyncronously gets the unread count for the message stream.
		/// Unread Count will be returned with an OnUnreadCountReceivedEvent - add a handler to handle this. 
		/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
		/// </summary>
		public static void UnreadCount () {
			#if UNITY_IOS
			MessageStream._unreadCount ();
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
			MessageStream._messages ();
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
			MessageStream._showMessageDetail (GetJsonForMessage(message).ToString());
			#elif UNITY_ANDROID
			CallAndroidStatic("showMessageDetail", message.messageID);
			#endif
		}

		/// <summary>
		/// Dismisses the message detail. Does nothing on Android.
		/// </summary>
		public static void DismissMessageDetail () {
			#if UNITY_IOS
			MessageStream._dismissMessageDetail();
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
			string messageString = GetJsonForMessage(message).ToString();
			#if UNITY_IOS
			MessageStream._registerImpression(messageString, (int)type);
			#elif UNITY_ANDROID
			CallAndroidStatic("registerMessageImpression", messageString, (int)type);
			#endif
		}

		/// <summary>
		/// Removes a given message from the user's stream or from a call to GetMessages.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
		/// </summary>
		/// <param name="message">Message the message to be removed.</param>
		public static void RemoveMessage(Message message) {
			string messageString = GetJsonForMessage(message).ToString();
			#if UNITY_IOS
			MessageStream._removeMessage(messageString);
			#elif UNITY_ANDROID
			CallAndroidStatic("removeMessage", messageString);
			#endif
		}

		/// <summary>
		/// Marks a given message as read.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
		/// </summary>
		public static void MarkMessageAsRead (Message message) {
			string messageString = GetJsonForMessage(message).ToString();
			#if UNITY_IOS
			MessageStream._markMessageAsRead(messageString);
			#elif UNITY_ANDROID
			CallAndroidStatic("markMessageAsRead", messageString);
			#endif
		}

		/// <summary>
		/// Marks given list of messages as read.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
		/// </summary>
		public static void MarkMessagesAsRead (List<Message> messages) {
			string messagesString = GetJsonForMessages(messages).ToString();
			#if UNITY_IOS
			MessageStream._markMessagesAsRead(messagesString);
			#elif UNITY_ANDROID
			CallAndroidStatic("markMessagesAsRead", messagesString);
			#endif
		}

		private static void CallAndroidStatic(string methodName, params object[] args) {
			using (AndroidJavaClass androidWrapper = new AndroidJavaClass(AndroidMessageStreamWrapper))
			using (AndroidJavaObject instance = androidWrapper.GetStatic<AndroidJavaObject> ("INSTANCE"))
			{
				instance.Call(methodName, args);
			}
		}

		/// <summary>
		/// Used only by the underlying unity plugin code. Do not call.
		/// </summary>
		public void ReceiveError(string errorDescription) {
			MessageStreamErrorEventArgs args = new MessageStreamErrorEventArgs ();
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

		public static JSONArray GetJsonForMessages (List<Message> messages) {
			JSONArray jsonArray = new JSONArray();
			foreach (Message message in messages) {
				JSONClass messageJson = GetJsonForMessage(message);
				jsonArray.Add("", messageJson);
			}
			return jsonArray;
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
		public static event EventHandler<MessageStreamErrorEventArgs> OnErrorEvent;
		public static event EventHandler<MessagesReceivedEventArgs> OnMessagesReceivedEvent;
		public static event EventHandler<UnreadCountReceivedEventArgs> OnUnreadCountReceivedEvent;
		#endregion
	}

	/// <summary>
	/// Marigold error event arguments.
	/// </summary>
	public class MessageStreamErrorEventArgs :EventArgs {
		public string ErrorDescription { get; set; }
	}


	/// <summary>
	/// Marigold messages received event.
	/// </summary>
	public class MessagesReceivedEventArgs :EventArgs {
		public List<Message> messages { get; set; }
	}

	/// <summary>
	/// Marigold unread count received event.
	/// </summary>
	public class UnreadCountReceivedEventArgs :EventArgs {
		public string UnreadCount { get; set; }
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
}
