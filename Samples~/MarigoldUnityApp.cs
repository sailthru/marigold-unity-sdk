using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MarigoldSimpleJSON;
using MarigoldSDK;

public class MarigoldUnityApp : MonoBehaviour {
	List<Message> theMessages = null;

	void Start () {
		Marigold marigold = new Marigold();

		// Start up Marigold SDK on the Unity side
		marigold.Start();

		// Set up Marigold handlers 
		Marigold.OnErrorEvent += (object sender, MarigoldErrorEventArgs e) => {
			Debug.Log ("Marigold Error returned: " + e.ErrorDescription);
		};
		Marigold.OnDeviceIdReceivedEvent += (object sender, DeviceIDReceivedEventArgs e) => {
			Debug.Log ("DeviceId: " + e.DeviceID);
		};

		// Set up EngageBySailthru handlers
		EngageBySailthru.OnErrorEvent += (object sender, EngageBySailthruErrorEventArgs e) => {
			Debug.Log ("Engage by Sailthru Error returned: " + e.ErrorDescription);
		};
		EngageBySailthru.OnProfileVarsReceivedEvent += (object sender, ProfileVarsReceivedEventArgs args) => {
			Debug.Log ("Profile Vars: " + args.ProfileVars.ToString());
		};
		EngageBySailthru.OnUnwrappedLinkReceivedEvent += (object sender, UwrappedLinkReceivedEventArgs args) => {
			Debug.Log ("Unwrapped Link: " + args.UnwrappedLink.ToString());
		};

		// Set up MessageStream handlers
		MessageStream.OnErrorEvent += (object sender, MessageStreamErrorEventArgs e) => {
			Debug.Log ("Message Stream Error returned: " + e.ErrorDescription);
		};
		MessageStream.OnMessagesReceivedEvent += (object sender, MessagesReceivedEventArgs e) => {
			if (e.messages != null && e.messages.Count > 0) {
				Message message = e.messages[0];
				Debug.Log ("First Marigold message");
				Debug.Log (message.title);
				Debug.Log (message.messageID);
				Debug.Log (message.createdAt);
				Debug.Log (message.imageURL);
				Debug.Log (message.videoURL);
				Debug.Log (message.type);
				Debug.Log (message.text);
				MessageStream messageStream = new MessageStream();
				messageStream.RegisterImpression(message, ImpressionType.StreamView); 
			}
			this.theMessages = e.messages;
		};
		MessageStream.OnUnreadCountReceivedEvent += (object sender, UnreadCountReceivedEventArgs e) => {
			Debug.Log ("Unread Count: " + e.UnreadCount);
		};


		// Log user in
		marigold.LogRegistrationEvent("user-name");

		// Set Location
		marigold.UpdateLocation (-44.01899F,176.565915F);

		marigold.SetInAppNotificationsEnabled(true);

		// Get the device ID
		marigold.DeviceID();


		MessageStream messageStream = new MessageStream();

		//Get some messages
		messageStream.GetMessages();

		//Get the unread count
		messageStream.UnreadCount();



		EngageBySailthru engageBySailthru = new EngageBySailthru();

		// Set user details
		engageBySailthru.SetUserId("unity-user-1234");
		engageBySailthru.SetUserEmail("unity-user-1234@sailthru.com");

		// Log an event 
		engageBySailthru.LogEvent ("User started playing game");

		// Set profile vars
		JSONClass jsonObject = new JSONClass();
		jsonObject["test"] = "me";
		engageBySailthru.SetProfileVars(jsonObject);

		// Get profile vars
		engageBySailthru.GetProfileVars();

		// Make a purchase
		JSONClass itemVars = new JSONClass();
		itemVars["item"] = "var";
		string[] tags = { "tag1", "tag2" };
		PurchaseItem[] purchaseItems = new PurchaseItem[] {
			new () { 
				quantity = 2,
				title = "test title",
				price = 1234,
				id = "test id",
				url = new Uri("https://www.sailthru.com/not-a-real-item"),
				vars = itemVars,
				tags = tags,

			}
		};

		PurchaseAdjustment[] purchaseAdjustments = new PurchaseAdjustment[] {
			new () {
				title = "tax",
				price = -20
			}
		};

		JSONClass purchaseVars = new JSONClass();
		purchaseVars["purchase"] = "with var";
		Purchase purchase = new Purchase() {
			purchaseItems = purchaseItems,
			purchaseAdjustments = purchaseAdjustments,
			vars = purchaseVars

		};
		engageBySailthru.LogPurchase(purchase);
	}
	public void OnMDClick() {
		if (this.theMessages == null || this.theMessages.Count > 0) {
			MessageStream messageStream = new MessageStream();
			messageStream.ShowMessageDetail (this.theMessages [0]);

			//Not required, but an example of marking a message as read
			messageStream.MarkMessageAsRead (this.theMessages [0]);
		} else {
			Debug.Log ("There are no messages to show");
		}
	}
}
