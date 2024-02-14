using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MarigoldSimpleJSON;

public class MarigoldUnityApp : MonoBehaviour {
	List<MarigoldMessage> theMessages = null;

	void Start () {
		// Start up Marigold SDK on the Unity side
		Marigold.Start();

		// Set up Marigold handlers 
		Marigold.OnErrorEvent += (object sender, MarigoldErrorEventArgs e) => {
			Debug.Log ("Error returned: " + e.ErrorDescription);
		};
		Marigold.OnMessagesReceivedEvent += (object sender, MarigoldMessagesReceivedEventArgs e) => {
			if (e.messages != null && e.messages.Count > 0) {
				MarigoldMessage message = e.messages[0];
				Debug.Log ("First Marigold message");
				Debug.Log (message.title);
				Debug.Log (message.messageID);
				Debug.Log (message.createdAt);
				Debug.Log (message.imageURL);
				Debug.Log (message.videoURL);
				Debug.Log (message.type);
				Debug.Log (message.text);
				Marigold.RegisterImpression(message, MarigoldImpressionType.StreamView); 
			}
			this.theMessages = e.messages;
		};
		Marigold.OnUnreadCountReceivedEvent += (object sender, MarigoldUnreadCountReceivedEventArgs e) => {
			Debug.Log ("Unread Count: " + e.UnreadCount);
		};
		Marigold.OnDeviceIdReceivedEvent += (object sender, MarigoldDeviceIDReceivedEventArgs e) => {
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
		MarigoldPurchaseItem[] purchaseItems = new MarigoldPurchaseItem[] {
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

		MarigoldPurchaseAdjustment[] purchaseAdjustments = new MarigoldPurchaseAdjustment[] {
			new () {
				title = "tax",
				price = -20
			}
		};

		JSONClass purchaseVars = new JSONClass();
		purchaseVars["purchase"] = "with var";
		MarigoldPurchase purchase = new MarigoldPurchase() {
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

	public void OnSetUserIdClick() {
		EngageBySailthru.SetUserId("test me");
	}
}
