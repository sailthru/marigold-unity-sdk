using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;

public class Carnival : MonoBehaviour
{
	#region Externals
	[DllImport("__Internal")]
	private static extern void _startEngine (string apiKey);

	[DllImport("__Internal")]
	private static extern void _setTags (string tags);

	[DllImport("__Internal")]
	private static extern void _getTags ();

	[DllImport("__Internal")]
	private static extern void _showMessageStream();

	[DllImport("__Internal")]
	private static extern void _updateLocation (double lat, double lon);

	[DllImport("__Internal")]
	private static extern void _logEvent (string eventName);

	[DllImport("__Internal")]
	private static extern void _setString (string stringValue, string key);

	[DllImport("__Internal")]
	private static extern void _setBool (bool boolValue, string key);

	[DllImport("__Internal")]
	private static extern void _setDate (UInt64 date, string key);

	[DllImport("__Internal")]
	private static extern void _setFloat (double floatValue, string key);

	[DllImport("__Internal")]
	private static extern void _setInteger (UInt64 integerValue, string key);

	[DllImport("__Internal")]
	private static extern void _removeAttribute (string key);
	#endregion


	#region Carnival SDK methods
	// Start Engine

	public static void StartEngine(string apiKey, string googleProjectNumber)
	{
		Debug.Log ("Start Engine is getting called");
		#if UNITY_IOS
		Carnival._startEngine (apiKey);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("startEngine", googleProjectNumber, apiKey);
		#endif
	}

	// Tags

	public static void SetTags(string[] tags)
	{
		#if UNITY_IOS
		Carnival._setTags(string.Join(",", tags));
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("setTags", string.Join(",", tags));
		#endif
	}


	public static void GetTags()
	{
		#if UNITY_IOS
		Carnival._getTags();
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("getTags");
		#endif
	
	}

	// Message Stream

	public static void ShowMessageStream()
	{
		#if UNITY_IOS
		Carnival._showMessageStream ();
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("showStream");
		#endif
	}

	// Location

	public static void UpdateLocation(double lat, double lon) {
		#if UNITY_IOS
		Carnival._updateLocation (lat, lon);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("updateLocation", lat, lon);
		#endif
	}

	// Custom Events

	public static void LogEvent(string eventName) {
		#if UNITY_IOS
		Carnival._logEvent (eventName);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("logEvent", eventName);
		#endif
	}

	// Custom Attributes

	public static void SetString (string stringValue, string key) {
		#if UNITY_IOS
		Carnival._setString (stringValue, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("setStringAttribute", key, stringValue);
		#endif
	}


	public static void SetBool (bool boolValue, string key) {
		#if UNITY_IOS
		Carnival._setBool (boolValue, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("setBooleanAttribute", key, boolValue);
		#endif
	}


	public static void SetDate (DateTime date , string key) {
		UInt64 unixTimestamp = (UInt64)(-1 * (date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
		#if UNITY_IOS
		Carnival._setDate (unixTimestamp, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("setDateAttribute", key, unixTimestamp);
		#endif
	}


	public static void SetFloat (float floatValue, string key) {
		#if UNITY_IOS
		Carnival._setFloat (floatValue, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("setFloatAttribute", key, floatValue);
		#endif
	}


	public static void SetInteger (UInt64 integerValue, string key) {
		#if UNITY_IOS
		Carnival._setInteger (integerValue, key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("setIntegerAttribute", key, integerValue);
		#endif
	}

	public static void RemoveAttribute (string key) {
		#if UNITY_IOS
		Carnival._removeAttribute (key);
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("removeAttribute", key);
		#endif
	}

	public void ReceiveError(string errorDescription) {
		CarnivalErrorEventArgs args = new CarnivalErrorEventArgs ();
		args.ErrorDescription = errorDescription;
		OnErrorEvent(this, args);
	}

	public void ReceiveTags(string tags) {
		CarnivalTagsRecievedEvent args = new CarnivalTagsRecievedEvent ();
		args.Tags = tags.Split (',');
		OnTagsRecievedEvent (this, args);
	}
	#endregion

	#region Callbabcks
	public static event EventHandler<CarnivalErrorEventArgs> OnErrorEvent;
	public static event EventHandler<CarnivalTagsRecievedEvent> OnTagsRecievedEvent;
	#endregion


}

public class CarnivalErrorEventArgs : EventArgs {
	public string ErrorDescription { get; set; }
}

public class CarnivalTagsRecievedEvent :EventArgs {
	public string[] Tags { get; set; }
}