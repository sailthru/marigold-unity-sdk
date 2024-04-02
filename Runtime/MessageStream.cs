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
		public void UnreadCount () {
			#if UNITY_IOS
			MessageStream._unreadCount ();
			#elif UNITY_ANDROID
			CallAndroid("unreadCount");
			#endif
		}
		
		/// <summary>
		/// Asyncronously returns a list of Messages.
		/// Messages will be returned with the OnMessagesReceivedEvent - add a handler to handle this. 
		/// </summary>
		public void GetMessages() {
			#if UNITY_IOS
			MessageStream._messages ();
			#elif UNITY_ANDROID
			CallAndroid("getMessages");
			#endif
		}

		/// <summary>
		/// Shows the message detail for a given message
		/// </summary>
		/// <param name="message">The message for which you want to see the detail for</param>
		public void ShowMessageDetail (Message message) {
			#if UNITY_IOS
			MessageStream._showMessageDetail (GetJsonForMessage(message).ToString());
			#elif UNITY_ANDROID
			CallAndroid("showMessageDetail", message.messageID);
			#endif
		}

		/// <summary>
		/// Dismisses the message detail. Does nothing on Android.
		/// </summary>
		public void DismissMessageDetail () {
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
		public void RegisterImpression(Message message, ImpressionType type) {
			string messageString = GetJsonForMessage(message).ToString();
			#if UNITY_IOS
			MessageStream._registerImpression(messageString, (int)type);
			#elif UNITY_ANDROID
			CallAndroid("registerMessageImpression", messageString, (int)type);
			#endif
		}

		/// <summary>
		/// Removes a given message from the user's stream or from a call to GetMessages.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
		/// </summary>
		/// <param name="message">Message the message to be removed.</param>
		public void RemoveMessage(Message message) {
			string messageString = GetJsonForMessage(message).ToString();
			#if UNITY_IOS
			MessageStream._removeMessage(messageString);
			#elif UNITY_ANDROID
			CallAndroid("removeMessage", messageString);
			#endif
		}

		/// <summary>
		/// Marks a given message as read.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
		/// </summary>
		public void MarkMessageAsRead (Message message) {
			string messageString = GetJsonForMessage(message).ToString();
			#if UNITY_IOS
			MessageStream._markMessageAsRead(messageString);
			#elif UNITY_ANDROID
			CallAndroid("markMessageAsRead", messageString);
			#endif
		}

		/// <summary>
		/// Marks given list of messages as read.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle thi
		/// </summary>
		public void MarkMessagesAsRead (List<Message> messages) {
			string messagesString = GetJsonForMessages(messages).ToString();
			#if UNITY_IOS
			MessageStream._markMessagesAsRead(messagesString);
			#elif UNITY_ANDROID
			CallAndroid("markMessagesAsRead", messagesString);
			#endif
		}

		private void CallAndroid(string methodName, params object[] args) {
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
				m.videoURL = jsonMessagesArray[i]["card_media_url"];
				m.imageURL = jsonMessagesArray[i]["card_image_url"];
				m.type = Message.StringToMessageType(jsonMessagesArray[i]["type"]);
				m.createdAt = jsonMessagesArray[i]["created_at"];
				m.isRead = jsonMessagesArray[i]["is_read"].AsBool;
				m.htmlText = jsonMessagesArray[i]["html_text"];
				m.attributes = jsonMessagesArray[i]["custom"].AsObject;
				messages.Add(m);
			}

			MessagesReceivedEventArgs args = new MessagesReceivedEventArgs ();
			args.messages = messages;
			OnMessagesReceivedEvent(this, args);
		}
		#endregion

		#region Helpers
        

#nullable enable
		public JSONClass GetJsonForMessage (Message message) {
			JSONClass jsonObject = new JSONClass();
			if (message.messageID != null) jsonObject["id"] = message.messageID;
			if (message.title != null) jsonObject["title"] = message.title;
			if (message.text != null) jsonObject["text"] = message.text;
			if (message.createdAt != null) jsonObject["created_at"] = message.createdAt;
			if (message.URL != null) jsonObject["url"] = message.URL;
			if (message.videoURL != null) jsonObject["card_media_url"] = message.videoURL;
			if (message.imageURL != null) jsonObject["card_image_url"] = message.imageURL;
			string? typeString = Message.MessageTypeToString(message.type);
			if (typeString != null) jsonObject["type"] = typeString;
			jsonObject["is_read"].AsBool = message.isRead;
			if (message.htmlText != null) jsonObject["html_text"] = message.htmlText;
			if (message.attributes != null) jsonObject["custom"] = message.attributes;
			jsonObject["notifications"] = new JSONArray();
			return jsonObject;
		}
#nullable disable

		public JSONArray GetJsonForMessages (List<Message> messages) {
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

#nullable enable
	/// <summary>
	/// Marigold message class.
	/// </summary>
	public class Message {
		public string? messageID { get; set; }
		public string? title { get; set; }
		public string? text { get; set; }
		public string? createdAt { get; set; }
		public bool isRead { get; set; }
		public string? URL { get; set; }
		public string? videoURL { get; set; }
		public string? imageURL { get; set; }
		public string? htmlText { get; set; }
		public JSONClass? attributes { get; set; }
		public MessageType? type { get; set; }
		public enum MessageType {Text, Video, Link, Image, Push};

		public static string? MessageTypeToString(MessageType? type) {
			return type switch {
				MessageType.Text => "text_message",
				MessageType.Video => "video_message",
				MessageType.Link => "link_message",
				MessageType.Image => "image_message",
				MessageType.Push => "push_message",
				_ => null
			};
		}

		public static MessageType? StringToMessageType(string? typeString) {
			return typeString switch {
				"text_message" => MessageType.Text,
				"video_message" => MessageType.Video,
				"link_message" => MessageType.Link,
				"image_message" => MessageType.Image,
				"push_message" => MessageType.Push,
				_ => null
			};
		}
	}
#nullable disable

	public enum ImpressionType {InAppNotificationView, StreamView, DetailView};
}
