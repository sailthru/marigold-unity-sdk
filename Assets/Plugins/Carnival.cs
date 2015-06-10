using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;
public class Carnival
{

	#region Externals
	[DllImport("__Internal")]
	private static extern void _startEngine (string apiKey);

	[DllImport("__Internal")]
	private static extern void _setTags (string tags, string GameObjectName, string TagCallback, string ErrorCallback);

	[DllImport("__Internal")]
	private static extern void _getTags (string GameObjectName, string TagCallback, string ErrorCallback);

	[DllImport("__Internal")]
	private static extern void _showMessageStream();

	[DllImport("__Internal")]
	private static extern void _updateLocation (double lat, double lon);

	[DllImport("__Internal")]
	private static extern void _logEvent (string eventName);

	[DllImport("__Internal")]
	private static extern void _setString (string stringValue, string key, string gameObjectName, string errorCallback);

	[DllImport("__Internal")]
	private static extern void _setBool (bool boolValue, string key, string gameObjectName, string errorCallback);

	[DllImport("__Internal")]
	private static extern void _setDate (UInt64 date, string key, string gameObjectName, string errorCallback);

	[DllImport("__Internal")]
	private static extern void _setFloat (float floatValue, string key, string gameObjectName, string errorCallback);

	[DllImport("__Internal")]
	private static extern void _setInteger (UInt64 integerValue, string key, string gameObjectName, string errorCallback);

	[DllImport("__Internal")]
	private static extern void _removeAttribute (string key, string gameObjectName, string errorCallback);
	#endregion






	// Start Engine

	public static void StartEngine(string apiKey)
	{
		Debug.Log ("Start Engine is getting called");
		#if UNITY_IOS
		Carnival._startEngine (apiKey);
		#elif UNITY_ANDROID
		#endif
	}

	// Tags

	public static void SetTags(string[] tags, string GameObjectName, string TagCallback, string ErrorCallback)
	{
		#if UNITY_IOS
		Carnival._setTags(string.Join(",", tags), GameObjectName, TagCallback, ErrorCallback);
		#elif UNITY_ANDROID
		#endif
	}


	public static void GetTags(string GameObjectName, string TagCallback, string ErrorCallback)
	{
		#if UNITY_IOS
		Carnival._getTags(GameObjectName, TagCallback, ErrorCallback);
		#elif UNITY_ANDROID
		#endif
	
	}

	// Message Stream

	public static void ShowMessageStream()
	{
		#if UNITY_IOS
		Carnival._showMessageStream ();
		#elif UNITY_ANDROID
		#endif
	}

	// Location

	public static void UpdateLocation(double lat, double lon) {
		#if UNITY_IOS
		Carnival._updateLocation (lat, lon);
		#elif UNITY_ANDROID
		#endif
	}

	// Custom Events

	public static void LogEvent(string eventName) {
		#if UNITY_IOS
		Carnival._logEvent (eventName);
		#elif UNITY_ANDROID
		#endif
	}

	// Custom Attributes

	public static void SetString (string stringValue, string key, string gameObjectName, string errorCallback) {
		#if UNITY_IOS
		Carnival._setString (stringValue, key, gameObjectName, errorCallback);
		#elif UNITY_ANDROID
		#endif
	}


	public static void SetBool (bool boolValue, string key, string gameObjectName, string errorCallback) {
		#if UNITY_IOS
		Carnival._setBool (boolValue, key, gameObjectName, errorCallback);
		#elif UNITY_ANDROID
		#endif
	}


	public static void SetDate (DateTime date , string key, string gameObjectName, string errorCallback) {
		#if UNITY_IOS
		Carnival._setDate ((UInt64) (date.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds), key, gameObjectName, errorCallback);
		#elif UNITY_ANDROID
		#endif
	}


	public static void SetFloat (float floatValue, string key, string gameObjectName, string errorCallback) {
		#if UNITY_IOS
		Carnival._setFloat (floatValue, key, gameObjectName, errorCallback);
		#elif UNITY_ANDROID
		#endif
	}


	public static void SetInteger (UInt64 integerValue, string key, string gameObjectName, string errorCallback) {
		#if UNITY_IOS
		Carnival._setInteger (integerValue, key, gameObjectName, errorCallback);
		#elif UNITY_ANDROID
		#endif
	}

	public static void RemoveAttribute (string key, string gameObjectName, string errorCallback) {
		#if UNITY_IOS
		Carnival._removeAttribute (key, gameObjectName, errorCallback);
		#elif UNITY_ANDROID
		#endif
	}


}
