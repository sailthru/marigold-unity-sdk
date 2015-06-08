using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;
public class Carnival
{
#if UNITY_IOS

	// Start Engine
	[DllImport("__Internal")]
	private static extern void _startEngine (string apiKey);
	public static void StartEngine(string apiKey)
	{
		Debug.Log ("Start Engine is getting called");
		Carnival._startEngine (apiKey);
	}

	//Tags
	[DllImport("__Internal")]
	private static extern void _setTags (string tags, string GameObjectName, string TagCallback, string ErrorCallback);
	public static void SetTagsInBackground (string[] tags, string GameObjectName, string TagCallback, string ErrorCallback)
	{
		Carnival._setTags(string.Join(",", tags), GameObjectName, TagCallback, ErrorCallback);
	}

	[DllImport("__Internal")]
	private static extern void _getTags (string GameObjectName, string TagCallback, string ErrorCallback);
	public static void GetTagsInBackground (string GameObjectName, string TagCallback, string ErrorCallback) {
		Carnival._getTags(GameObjectName, TagCallback, ErrorCallback);
	}

	//Message Stream
	[DllImport("__Internal")]
	private static extern void _showMessageStream();
	public static void ShowMessageStream() {

		Carnival._showMessageStream ();
	}

	//Location
	[DllImport("__Internal")]
	private static extern void _updateLocation (double lat, double lon);
	public static void UpdateLocation(double lat, double lon) {
		Carnival._updateLocation (lat, lon);
	}

	//Events
	[DllImport("__Internal")]
	private static extern void _logEvent (string eventName);
	public static void LogEvent(string eventName) {
		Carnival._logEvent (eventName);
	}

	//Attributes
	[DllImport("__Internal")]
	private static extern void _setString (string stringValue, string key, string gameObjectName, string errorCallback);
	public static void SetString (string stringValue, string key, string gameObjectName, string errorCallback) {
		Carnival._setString (stringValue, key, gameObjectName, errorCallback);
	}

	[DllImport("__Internal")]
	private static extern void _setBool (bool boolValue, string key, string gameObjectName, string errorCallback);
	public static void SetBool (bool boolValue, string key, string gameObjectName, string errorCallback) {
		Carnival._setBool (boolValue, key, gameObjectName, errorCallback);
	}

	[DllImport("__Internal")]
	private static extern void _setDate (UInt64 date, string key, string gameObjectName, string errorCallback);
	public static void SetDate (DateTime date , string key, string gameObjectName, string errorCallback) {
		Carnival._setDate ((UInt64) (date.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds), key, gameObjectName, errorCallback);
	}

	[DllImport("__Internal")]
	private static extern void _setFloat (float floatValue, string key, string gameObjectName, string errorCallback);
	public static void SetFloat (float floatValue, string key, string gameObjectName, string errorCallback) {
		Carnival._setFloat (floatValue, key, gameObjectName, errorCallback);
	}

	[DllImport("__Internal")]
	private static extern void _setInteger (UInt64 integerValue, string key, string gameObjectName, string errorCallback);
	public static void SetInteger (UInt64 integerValue, string key, string gameObjectName, string errorCallback) {
		Carnival._setInteger (integerValue, key, gameObjectName, errorCallback);
	}

	[DllImport("__Internal")]
	private static extern void _removeAttribute (string key, string gameObjectName, string errorCallback);
	public static void RemoveAttribute (string key, string gameObjectName, string errorCallback) {
		Carnival._removeAttribute (key, gameObjectName, errorCallback);
	}

#elif UNITY_ANDROID
#endif
}
