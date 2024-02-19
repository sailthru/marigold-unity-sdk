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
		// Start up Marigold SDK on the Unity side
		Marigold.Start();

		// Set up Marigold handlers 
		Marigold.OnErrorEvent += (object sender, ErrorEventArgs e) => {
			Debug.Log ("Error returned: " + e.ErrorDescription);
		};
		Marigold.OnMessagesReceivedEvent += (object sender, MessagesReceivedEventArgs e) => {
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
				Marigold.RegisterImpression(message, ImpressionType.StreamView); 
			}
			this.theMessages = e.messages;
		};
		Marigold.OnUnreadCountReceivedEvent += (object sender, UnreadCountReceivedEventArgs e) => {
			Debug.Log ("Unread Count: " + e.UnreadCount);
		};
		Marigold.OnDeviceIdReceivedEvent += (object sender, DeviceIDReceivedEventArgs e) => {
			Debug.Log ("DeviceId: " + e.DeviceID);
		};

		// Set up EngageBySailthru handlers
		EngageBySailthru.OnProfileVarsReceivedEvent += (object sender, EngageBySailthruProfileVarsReceivedEventArgs args) => {
			Debug.Log ("Profile Vars: " + args.ProfileVars.ToString());
		};
		EngageBySailthru.OnUnwrappedLinkReceivedEvent += (object sender, EngageBySailthruUwrappedLinkReceivedEventArgs args) => {
			Debug.Log ("Unwrapped Link: " + args.UnwrappedLink.ToString());
		};


		// Set Location
		Marigold.UpdateLocation (-44.01899F,176.565915F);

		Marigold.SetInAppNotificationsEnabled(true);

		//Get some messages
		Marigold.GetMessages();

		//Get the unread count
		Marigold.UnreadCount();

		// Get the device ID
		Marigold.DeviceID();


		// Set user details
		EngageBySailthru.SetUserId("unity-user-1234");
		EngageBySailthru.SetUserEmail("unity-user-1234@carnival.io");

		// Log an event 
		EngageBySailthru.LogEvent ("User started playing game");

		// Set profile vars
		JSONClass jsonObject = new JSONClass();
		jsonObject["test"] = "me";
		EngageBySailthru.SetProfileVars(jsonObject);

		// Get profile vars
		EngageBySailthru.GetProfileVars();

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
		EngageBySailthru.LogPurchase(purchase);
	}
	public void OnMDClick() {
		if (this.theMessages == null || this.theMessages.Count > 0) {
			Marigold.ShowMessageDetail (this.theMessages [0]);

			//Not required, but an example of marking a message as read
			Marigold.MarkMessageAsRead (this.theMessages [0]);
		} else {
			Debug.Log ("There are no messages to show");
		}
	}
}
