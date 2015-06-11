using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;

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
	private static extern void _setDate (Int64 date, string key);

	[DllImport("__Internal")]
	private static extern void _setFloat (float floatValue, string key);

	[DllImport("__Internal")]
	private static extern void _setInteger (UInt64 integerValue, string key);

	[DllImport("__Internal")]
	private static extern void _removeAttribute (string key);
	#endif
	#endregion


	#region Carnival SDK methods
	/// <summary>
	///  Sets the Carnival appKey credentials for this app. 
	///  This MUST be done before calling any other Carnival methods.
	///  Call this method if you plan to target iOS.  
	/// </summary>
	/// <param name="apiKey">The api key you recieved when setting up your iOS app at http://app.carnivalmobile.com </param>
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
	/// <param name="apiKey">The api key you recieved when setting up your Android app at http://app.carnivalmobile.com </param>
	/// <param name="googleProjectNumber">The Project Number from your GCM dashboard </param>  
	public static void StartEngineAndroid(string apiKey, string googleProjectNumber)
	{
		#if UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("setTags", string.Join(",", tags));
		#endif
	}

	/// <summary>
	/// Asyncronously gets the tags for Carnival for this Device.
	/// Tags will be called returned with an OnTagsRecievedEvent - add a handler to handle this. 
	/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
	/// </summary>
	public static void GetTags()
	{
		#if UNITY_IOS
		Carnival._getTags();
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("getTags");
		#endif
	
	}

	/// <summary>
	/// Shows the message stream.
	/// </summary>
	public static void ShowMessageStream()
	{
		#if UNITY_IOS
		Carnival._showMessageStream ();
		#elif UNITY_ANDROID
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("openStream");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
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
		AndroidJavaClass _plugin = new AndroidJavaClass("com.carnival.sdk.unitywrapper.CarnivalWrapper");
		_plugin.CallStatic("removeAttribute", key);
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

/// <summary>
/// Carnival error event arguments.
/// </summary>
public class CarnivalErrorEventArgs : EventArgs {
	public string ErrorDescription { get; set; }
}

/// <summary>
/// Carnival tags recieved event.
/// </summary>
public class CarnivalTagsRecievedEvent :EventArgs {
	public string[] Tags { get; set; }
}