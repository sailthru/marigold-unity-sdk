using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;
using MarigoldSimpleJSON;

namespace MarigoldSDK {
	public class EngageBySailthru : MonoBehaviour
	{
		#region Externals
		#if UNITY_IOS
		
	#nullable enable
		[DllImport("__Internal")]
		private static extern void _trackPageview (string url, string? tags);

		[DllImport("__Internal")]
		private static extern void _trackImpression (string sectionId, string? urls);
	#nullable disable

		[DllImport("__Internal")]
		private static extern void _trackClick (string sectionId, string url);

	#nullable enable
		[DllImport("__Internal")]
		private static extern void _setUserEmail (string? userEmail);

		[DllImport("__Internal")]
		private static extern void _setUserId (string? userId);
		
		[DllImport("__Internal")]
		private static extern void _logEvent (string value, string? varsString);
	#nullable disable
		
		[DllImport("__Internal")]
		private static extern void _clearEvents ();

		[DllImport("__Internal")]
		private static extern void _setProfileVars (string varsString);

		[DllImport("__Internal")]
		private static extern void _getProfileVars ();

		[DllImport("__Internal")]
		private static extern void _handleSailthruLink (string linkString);

		[DllImport("__Internal")]
		private static extern void _logPurchase(string purchaseString);

		[DllImport("__Internal")]
		private static extern void _logAbandonedCart (string purchaseString);

		[DllImport("__Internal")]
		private static extern void _setAttributes (string attributeString);
		
		[DllImport("__Internal")]
		private static extern void _removeAttribute (string key);

		[DllImport("__Internal")]
		private static extern void _clearAttributes ();

		#endif
		#endregion
		
		
		#region EngageBySailthru methods
		private const string AndroidEngageBySailthruWrapper = "com.marigold.sdk.unity.EngageBySailthruWrapper";

	#nullable enable
		/// <summary>
		/// Asynchronously registers that the given "page" has been viewed with Sailthru SPM.
		/// Note that a Content View here directly corresponds to a page view in SPM.
		/// </summary>
		/// <param name="url">The URL of the page we're tracking a view of. Must be a valid URL with protocol (eg. http:// or https://) - this generally should correspond to the web link of the content being tracked, and the stored URL in the Marigold content collection. This must not be null..</param>
		/// <param name="tags">Tags for this page. May be null.</param>
		public void TrackPageview(string url, string[]? tags) {
			string? tagsString = StringArrayToJSONArrayString(tags);
			#if UNITY_IOS
			EngageBySailthru._trackPageview (url, tagsString);
			#elif UNITY_ANDROID
			CallAndroid("trackPageview", url, tagsString);
			#endif
		}

		/// <summary>
		/// Asynchronously registers an impression - a reasonable expectation that a user has seen a piece of content - with
		/// Sailthru SPM.
		/// </summary>
		/// <param name="sectionId">The Section ID on Marigold SPM corresponding to the section being viewed. Must not be null.</param>
		/// <param name="urls">a List of the URLs of the items contained within this section. Useful if multiple items of content are contained within a section, otherwise just pass a single-item array. May be null.</param>
		public void TrackImpression (string sectionId, string[]? urls) {
			string? urlsString = StringArrayToJSONArrayString(urls);
			#if UNITY_IOS
			EngageBySailthru._trackImpression (sectionId, urlsString);
			#elif UNITY_ANDROID
			CallAndroid("trackImpression", sectionId, urlsString);
			#endif
		}
	#nullable disable

		/// <summary>
		/// Asynchronously registers with Sailthru SPM that a section has been clicked/tapped on,
		/// transitioning the user to a detail view.
		/// </summary>
		/// <param name="sectionId">The ID of the section to track a click for. Must not be null.</param>
		/// <param name="url">The URL of the detail being transitioned to. Must not be null.</param>
		public void TrackClick(string sectionId, string url){
			#if UNITY_IOS
			EngageBySailthru._trackClick(sectionId, url);
			#elif UNITY_ANDROID
			CallAndroid("trackClick", sectionId, url);
			#endif
		}

	#nullable enable
		/// <summary>
		/// Set the user's Email.
		/// </summary>
		/// <param name="userEmail">The user's email address.</param>
		public void SetUserEmail (string? userEmail) {
			#if UNITY_IOS
			EngageBySailthru._setUserEmail (userEmail);
			#elif UNITY_ANDROID
			CallAndroid("setUserEmail", userEmail);
			#endif
		}

		/// <summary>
		/// Set an arbitrary external User ID.
		/// </summary>
		/// <param name="userId">An external User ID.</param>
		public void SetUserId (string? userId) {
			#if UNITY_IOS
			EngageBySailthru._setUserId (userId);
			#elif UNITY_ANDROID
			CallAndroid("setUserId", userId);
			#endif
		}

		/// <summary>
		/// Log a custom event. If value is null or an empty string, no event will be generated.
		/// </summary>
		/// <param name="value">The event's name.</param>
		public void LogEvent (string value) {
			#if UNITY_IOS
			EngageBySailthru._logEvent (value, null);
			#elif UNITY_ANDROID
			CallAndroid("logEvent", value);
			#endif
		}
	#nullable disable
		
		/// <summary>
		/// Log a custom event with associated vars. If value is null or an empty string, no event will
		/// be generated.
		/// </summary>
		/// <param name="value">The event's name.</param>
		/// <param name="varsString"></param>
		public void LogEvent (string value, JSONClass vars) {
			#if UNITY_IOS
			EngageBySailthru._logEvent (value, vars.ToString());
			#elif UNITY_ANDROID
			CallAndroid("logEvent", value, vars.ToString());
			#endif
		}

		/// <summary>
		/// Clear the custom events from the device data.
		/// </summary>
		public void ClearEvents() {
			#if UNITY_IOS
			EngageBySailthru._clearEvents ();
			#elif UNITY_ANDROID
			CallAndroid("clearEvents");
			#endif
		}
		
		/// <summary>
		/// Set the profile vars on the user profile.
		/// </summary>
		/// <param name="varsString">JSON string containing the vars to be set</param>
		public void SetProfileVars(JSONClass vars) {
			#if UNITY_IOS
			EngageBySailthru._setProfileVars (vars.ToString());
			#elif UNITY_ANDROID
			CallAndroid("setProfileVars", vars.ToString());
			#endif
		}

		/// <summary>
		/// Retrieve the profile vars from the user profile.
		/// </summary>
		public void GetProfileVars () {
			#if UNITY_IOS
			EngageBySailthru._getProfileVars ();
			#elif UNITY_ANDROID
			CallAndroid("getProfileVars");
			#endif
		}

		/// <summary>
		/// If you're using Sailthru email with universal links, your application will open with an encoded Sailthru link url.
		/// This method will decode the link URL and return its canonical location (ie https://link.sailthru.com/blahblah will redirect to https://www.sailthru.com/careers/list/, the link's canonical location),
		/// as well as making sure that the clickthrough metrics for this link are correctly attributed in the Marigold platform.
		/// </summary>
		/// <param name="linkString">The Sailthru Link to be unrolled.</param>
		public void HandleSailthruLink (Uri link) {
			#if UNITY_IOS
			EngageBySailthru._handleSailthruLink(link.ToString());
			#elif UNITY_ANDROID
			CallAndroid("handleSailthruLink", link.ToString());
			#endif
		}

		/// <summary>
		/// Logs a purchase with the Marigold platform. This can be used for mobile purchase attribution.
		/// </summary>
		/// <param name="purchase">The purchase to log with the platform.</param>
		public void LogPurchase(Purchase purchase) {
			string purchaseString = GetJsonForPurchase(purchase).ToString();
			#if UNITY_IOS
			EngageBySailthru._logPurchase(purchaseString);
			#elif UNITY_ANDROID
			CallAndroid("logPurchase", purchaseString);
			#endif
		}

		/// <summary>
		/// Logs a cart abandonment with the Marigold platform. Use this to initiate cart abandoned flows.
		/// </summary>
		/// <param name="purchase">The abandoned purchase to log with the platform.</param>
		public void LogAbandonedCart(Purchase purchase) {
			string purchaseString = GetJsonForPurchase(purchase).ToString();
			#if UNITY_IOS
			EngageBySailthru._logAbandonedCart(purchaseString);
			#elif UNITY_ANDROID
			CallAndroid("logAbandonedCart", purchaseString);
			#endif
		}

		/// <summary>
		/// Set the device attributes on the device in the Marigold platform.
		/// </summary>
		/// <param name="attributeMap">The attributes to set on the device.</param>
		public void SetAttributes(AttributeMap attributeMap) {
			string attributeString = GetJsonForDeviceAttributes(attributeMap).ToString();
			#if UNITY_IOS
			EngageBySailthru._setAttributes(attributeString);
			#elif UNITY_ANDROID
			CallAndroid("setAttributes", attributeString);
			#endif
		}

		/// <summary>
		/// Remove the device attribute for the given key from the device in the Marigold platform.
		/// </summary>
		/// <param name="key">The key of the attribute to remove.</param>
		public void RemoveAttribute(string key) {
			#if UNITY_IOS
			EngageBySailthru._removeAttribute(key);
			#elif UNITY_ANDROID
			CallAndroid("removeAttribute", key);
			#endif
		}

		/// <summary>
		/// Clear all device attributes from the device in the Marigold platform.
		/// </summary>
		public void ClearAttributes() {
			#if UNITY_IOS
			EngageBySailthru._clearAttributes();
			#elif UNITY_ANDROID
			CallAndroid("clearAttributes");
			#endif
		}

		#endregion

		#region Helpers 

		private void CallAndroid(string methodName, params object[] args) {
			using (AndroidJavaClass androidWrapper = new AndroidJavaClass(AndroidEngageBySailthruWrapper))
			using (AndroidJavaObject instance = androidWrapper.GetStatic<AndroidJavaObject> ("INSTANCE"))
			{
				instance.Call(methodName, args);
			}
		}

	#nullable enable
		private string? StringArrayToJSONArrayString(string[]? array) {
			if (array == null) {
				return null;
			}

			JSONArray jsonArray = new JSONArray();
			foreach (string value in array) {
				jsonArray.Add("", value);
			}
			return jsonArray.ToString();
		}
	#nullable disable

		/// <summary>
		/// Used only by the underlying unity plugin code. Do not call.
		/// </summary>
		public void ReceiveError(string errorDescription) {
			EngageBySailthruErrorEventArgs args = new EngageBySailthruErrorEventArgs ();
			args.ErrorDescription = errorDescription;
			OnErrorEvent.Invoke(this, args);
		}

		/// <summary>
		/// Used only by the underlying unity plugin code. Do not call.
		/// </summary>
		public void ReceiveProfileVars(string profileVarsJSON) {
			ProfileVarsReceivedEventArgs args = new ProfileVarsReceivedEventArgs ();
			args.ProfileVars = (JSONClass)JSONClass.Parse(profileVarsJSON);
			OnProfileVarsReceivedEvent.Invoke(this, args);
		}
		
		/// <summary>
		/// Used only by the underlying unity plugin code. Do not call.
		/// </summary>
		public void ReceiveUnwrappedLink(string unwrappedLink) {
			UwrappedLinkReceivedEventArgs args = new UwrappedLinkReceivedEventArgs ();
			args.UnwrappedLink = new Uri(unwrappedLink);
			OnUnwrappedLinkReceivedEvent.Invoke(this, args);
		}

		public JSONClass GetJsonForPurchase (Purchase purchase) {
			JSONClass jsonObject = new JSONClass();
			JSONArray itemArray = new JSONArray();
			foreach (PurchaseItem item in purchase.purchaseItems) {
				itemArray.Add("", GetJsonForPurchaseItem(item));
			}
			jsonObject["items"] = itemArray;
			if (purchase.purchaseAdjustments != null) {
				JSONArray adjustmentArray = new JSONArray();
				foreach (PurchaseAdjustment adjustment in purchase.purchaseAdjustments) {
					adjustmentArray.Add("", GetJsonForPurchaseAdjustment(adjustment));
				}
				jsonObject["adjustments"] = adjustmentArray;
			}
			if (purchase.messageID != null) jsonObject["message_id"] = purchase.messageID;
			if (purchase.vars != null) jsonObject["vars"] = purchase.vars;
			return jsonObject;
		}
		
		public JSONClass GetJsonForPurchaseItem (PurchaseItem purchaseItem) {
			JSONClass jsonObject = new JSONClass();
			jsonObject["qty"].AsInt = purchaseItem.quantity;
			jsonObject["title"] = purchaseItem.title;
			jsonObject["price"].AsInt = purchaseItem.price;
			jsonObject["id"] = purchaseItem.id;
			if (purchaseItem.url != null) jsonObject["url"] = purchaseItem.url.ToString();

			if (purchaseItem.tags != null) {
				JSONArray tagsArray = new JSONArray();
				foreach (string tag in purchaseItem.tags) {
					tagsArray.Add("", tag);
				}
				jsonObject["tags"] = tagsArray;
			}
			if (purchaseItem.vars != null) jsonObject["vars"] = purchaseItem.vars;
			if (purchaseItem.images != null) jsonObject["images"] = purchaseItem.images;
			return jsonObject;
		}

		public JSONClass GetJsonForPurchaseAdjustment (PurchaseAdjustment purchaseAdjustment) {
			JSONClass jsonObject = new JSONClass();
			jsonObject["title"] = purchaseAdjustment.title;
			jsonObject["price"].AsInt = purchaseAdjustment.price;
			return jsonObject;
		}

		public JSONClass GetJsonForDeviceAttributes (AttributeMap attributeMap) {
			JSONClass jsonObject = new JSONClass();
			jsonObject["mergeRule"].AsInt = (int)attributeMap.mergeRules;
			jsonObject["attributes"] = attributeMap.attributes;
			return jsonObject;
		}
		
		#endregion
		
		#region Callbacks
		public static event EventHandler<EngageBySailthruErrorEventArgs> OnErrorEvent;
		public static event EventHandler<ProfileVarsReceivedEventArgs> OnProfileVarsReceivedEvent;
		public static event EventHandler<UwrappedLinkReceivedEventArgs> OnUnwrappedLinkReceivedEvent;
		#endregion
	}

	/// <summary>
	/// Marigold error event arguments.
	/// </summary>
	public class EngageBySailthruErrorEventArgs :EventArgs {
		public string ErrorDescription { get; set; }
	}

	/// <summary>
	/// Marigold messages received event.
	/// </summary>
	public class ProfileVarsReceivedEventArgs :EventArgs {
		public JSONClass ProfileVars { get; set; }
	}


	/// <summary>
	/// Marigold messages received event.
	/// </summary>
	public class UwrappedLinkReceivedEventArgs :EventArgs {
		public Uri UnwrappedLink { get; set; }
	}

	#nullable enable
	/// <summary>
	/// Marigold purchase class, contains all data related to a purchase/cart.
	/// </summary>
	public class Purchase {
		/** Required fields **/
		/// <summary>
		/// Array of the PurchaseItems being purchased.
		/// </summary>
		public PurchaseItem[] purchaseItems { get; set; } = new PurchaseItem[0];

		/** Optional fields **/
		/// <summary>
		/// Array of the PurchaseAdjustments to add to the purchase.
		/// </summary>
		public PurchaseAdjustment[]? purchaseAdjustments { get; set; }
		/// <summary>
		/// Open format JSON for purchase vars.
		/// </summary>
		public JSONClass? vars { get; set; }
		/// <summary>
		/// ID of the Sailthru email to attribute this purchase to.
		/// </summary>
		public string? messageID { get; set; }
	}

	/// <summary>
	/// Marigold purchase item class, represents the actual items being purchased.
	/// </summary>
	public class PurchaseItem {
		/** Required fields **/
		/// <summary>
		/// How many of the item are being purchased.
		/// </summary>
		public int quantity { get; set; } = 1;
		/// <summary>
		/// The name of the item being purchased.
		/// </summary>
		public string title { get; set; } = string.Empty;
		/// <summary>
		/// The price of the item being purchased in cents.
		/// </summary>
		public int price { get; set; } = 0;
		/// <summary>
		/// The ID of the item being purchased.
		/// </summary>
		public string id { get; set; } = string.Empty;
		/// <summary>
		/// A url poiting to the content of the item being purchased.
		/// </summary>
		public Uri? url { get; set; }

		/** Optional fields **/
		/// <summary>
		/// Array of strings to add as tags for the purchase item.
		/// </summary>
		public string[]? tags { get; set; }
		/// <summary>
		/// Open format JSON for purchase item vars.
		/// </summary>
		public JSONClass? vars { get; set; }
		/// <summary>
		/// JSON in the following format to set images:
		/// <code>
		/// {
		///		“full” : {
		/// 		“url” : “http://example.com/f.jpg”
		/// 	},
		/// 	“thumb” : {
		/// 		“url” : “http://example.com/t.jpg”
		/// 	}
		/// }
		/// </code>
		/// "full" and "thumb" are both optional.
		/// </summary>
		public JSONClass? images { get; set; }
	}

	public class PurchaseAdjustment {
		/** Required fields **/
		/// <summary>
		/// The name of the adjustment being added.
		/// </summary>
		public string title { get; set; } = string.Empty;
		/// <summary>
		/// The price of the adjustment being added in cents. Can be positive or negative.
		/// </summary>
		public int price { get; set; } = 0;
	}

    public enum MergeRules {
		UPDATE,
		REPLACE
	}

	public class AttributeMap {
		private const string AttributeMapKeyType = "type";
		private const string AttributeMapKeyValue = "value";

		private const string AttributeMapValueString = "string";
		private const string AttributeMapValueStringArray = "stringArray";
		private const string AttributeMapValueInteger = "integer";
		private const string AttributeMapValueIntegerArray = "integerArray";
		private const string AttributeMapValueFloat = "float";
		private const string AttributeMapValueFloatArray = "floatArray";
		private const string AttributeMapValueBoolean = "boolean";
		private const string AttributeMapValueDate = "date";
		private const string AttributeMapValueDateArray = "dateArray";

		public MergeRules mergeRules { get; set; } = MergeRules.UPDATE;
		public JSONClass attributes { get; set; } = new JSONClass();

		public void SetString (string key, string value) {
			JSONClass attribute = CreateAttribute (AttributeMapValueString);
			attribute[AttributeMapKeyValue] = value;
			attributes[key] = attribute;
		}

		public string? GetString (string key) {
			JSONClass? attribute = GetAttribute(key, AttributeMapValueString);
			if (attribute == null) {
				return null;
			}
			return attribute[AttributeMapKeyValue].Value;
		}

		public void SetStringArray (string key, string[] values) {
			JSONClass attribute = CreateAttribute (AttributeMapValueStringArray);
			JSONArray jsonArray = attribute[AttributeMapKeyValue].AsArray;
			foreach (string value in values) {
				jsonArray[-1] = value;
			}
			attributes[key] = attribute;
		}

		public string[]? GetStringArray (string key) {
			JSONArray? jsonArray = GetAttributeArray(key, AttributeMapValueStringArray);
			if (jsonArray == null) {
				return null;
			}
			string[] array = new string[jsonArray.Count];
			for (int i = 0; i < jsonArray.Count; i++) {
				array[i] = jsonArray[i].Value;
			}
			return array;
		}

		public void SetInteger (string key, int value) {
			JSONClass attribute = CreateAttribute (AttributeMapValueInteger);
			attribute[AttributeMapKeyValue].AsInt = value;
			attributes[key] = attribute;
		}

		public int? GetInteger (string key) {
			JSONClass? attribute = GetAttribute(key, AttributeMapValueInteger);
			if (attribute == null) {
				return null;
			}
			return attribute[AttributeMapKeyValue].AsInt;
		}

		public void SetIntegerArray (string key, int[] values) {
			JSONClass attribute = CreateAttribute (AttributeMapValueIntegerArray);
			JSONArray jsonArray = attribute[AttributeMapKeyValue].AsArray;
			foreach (int value in values) {
				jsonArray[-1].AsInt = value;
			}
			attributes[key] = attribute;
		}

		public int[]? GetIntegerArray (string key) {
			JSONArray? jsonArray = GetAttributeArray(key, AttributeMapValueIntegerArray);
			if (jsonArray == null) {
				return null;
			}
			int[] array = new int[jsonArray.Count];
			for (int i = 0; i < jsonArray.Count; i++) {
				array[i] = jsonArray[i].AsInt;
			}

			return array;
		}

		public void SetFloat (string key, float value) {
			JSONClass attribute = CreateAttribute (AttributeMapValueFloat);
			attribute[AttributeMapKeyValue].AsFloat = value;
			attributes[key] = attribute;
		}

		public float? GetFloat (string key) {
			JSONClass? attribute = GetAttribute(key, AttributeMapValueFloat);
			if (attribute == null) {
				return null;
			}
			return attribute[AttributeMapKeyValue].AsFloat;
		}

		public void SetFloatArray (string key, float[] values) {
			JSONClass attribute = CreateAttribute (AttributeMapValueFloatArray);
			JSONArray jsonArray = attribute[AttributeMapKeyValue].AsArray;
			foreach (float value in values) {
				jsonArray[-1].AsFloat = value;
			}
			attributes[key] = attribute;
		}

		public float[]? GetFloatArray (string key) {
			JSONArray? jsonArray = GetAttributeArray(key, AttributeMapValueFloatArray);
			if (jsonArray == null) {
				return null;
			}
			float[] array = new float[jsonArray.Count];
			for (int i = 0; i < jsonArray.Count; i++) {
				array[i] = jsonArray[i].AsFloat;
			}

			return array;
		}

		public void SetBool (string key, bool value) {
			JSONClass attribute = CreateAttribute (AttributeMapValueBoolean);
			attribute[AttributeMapKeyValue].AsBool = value;
			attributes[key] = attribute;
		}

		public bool? GetBool (string key) {
			JSONClass? attribute = GetAttribute(key, AttributeMapValueBoolean);
			if (attribute == null) {
				return null;
			}
			return attribute[AttributeMapKeyValue].AsBool;
		}

		public void SetDate (string key, DateTimeOffset value) {
			JSONClass attribute = CreateAttribute (AttributeMapValueDate);
			attribute[AttributeMapKeyValue] = value.ToUnixTimeMilliseconds().ToString();
			attributes[key] = attribute;
		}

		public DateTimeOffset? GetDate (string key) {
			JSONClass? attribute = GetAttribute(key, AttributeMapValueDate);
			if (attribute == null) {
				return null;
			}
			string? value = attribute[AttributeMapKeyValue];
			if (value == null) {
				return null;
			}
			return DateTimeOffset.FromUnixTimeMilliseconds (long.Parse(value));
		}

		public void SetDateArray (string key, DateTimeOffset[] values) {
			JSONClass attribute = CreateAttribute (AttributeMapValueDateArray);
			JSONArray jsonArray = attribute[AttributeMapKeyValue].AsArray;
			foreach (DateTimeOffset value in values) {
				jsonArray[-1] = value.ToUnixTimeMilliseconds().ToString();
			}
			attributes[key] = attribute;
		}

		public DateTimeOffset[]? GetDateArray (string key) {
			JSONArray? jsonArray = GetAttributeArray(key, AttributeMapValueDateArray);
			if (jsonArray == null) {
				return null;
			}
			DateTimeOffset[] array = new DateTimeOffset[jsonArray.Count];
			for (int i = 0; i < jsonArray.Count; i++) {
				string? value = jsonArray[i];
				if (value == null) {
					continue;
				}
				array[i] = DateTimeOffset.FromUnixTimeMilliseconds (long.Parse(value));
			}

			return array;
		}

		private JSONClass CreateAttribute (string type) {
			JSONClass attribute = new JSONClass();
			attribute[AttributeMapKeyType] = type;
			return attribute;
		}

		private JSONClass? GetAttribute (string key, string type) {
			if (attributes[key] == null) {
				return null;
			}
			JSONClass? attribute = attributes[key].AsObject;
			if (attribute == null || !type.Equals(attribute[AttributeMapKeyType])) {
				return null;
			}
			return attribute;
		}

		private JSONArray? GetAttributeArray (string key, string type) {
			JSONClass? attribute = GetAttribute(key, type);
			if (attribute == null) {
				return null;
			}
			return attribute[AttributeMapKeyValue].AsArray;
		}
	}
	#nullable disable
}
