using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

public class Carnival
{
#if UNITY_IOS

	// Start Engine
	[DllImport("__Internal")]
	private static extern void start_engine (string apiKey);
	public static void StartEngine(string apiKey)
	{
		Debug.Log ("Start Engine is getting called");
		Carnival.start_engine (apiKey);
	}

	//Tags
	[DllImport("__Internal")]
	private static extern void set_tags (string apiKey);
	public static void SetTagsInBackground (string[] tags)
	{
		Carnival.set_tags(string.Join(",", tags));
	}
	[DllImport("__Internal")]
	private static extern void get_tags (string GameObjectName, string TagCallback, string ErrorCallback);

	public static void GetTagsInBackground (string GameObjectName, string TagCallback, string ErrorCallback) {
		Carnival.get_tags(GameObjectName, TagCallback, ErrorCallback);
	}

	//Message Stream
	[DllImport("__Internal")]
	private static extern void show_message_stream ();
	public static void ShowMessageStream() {

		Carnival.show_message_stream ();
	}

	//Location
	[DllImport("__Internal")]
	private static extern void update_location (double lat, double lon);
	public static void UpdateLocation(double lat, double lon) {
		Carnival.update_location (lat, lon);
	}

#elif UNITY_ANDROID
#endif
}
