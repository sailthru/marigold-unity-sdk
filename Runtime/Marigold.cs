using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MarigoldSDK {
	public class Marigold : MonoBehaviour
	{

		#region Externals
		#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern void _start ();

		[DllImport("__Internal")]
		private static extern void _updateLocation (double lat, double lon);

		[DllImport("__Internal")]
		private static extern void _deviceID ();

	#nullable enable
		[DllImport("__Internal")]
		private static extern void _logRegistrationEvent (string? userID);
	#nullable disable

		[DllImport("__Internal")]
		private static extern void _setInAppNotificationsEnabled (bool enabled);

		[DllImport("__Internal")]
		private static extern void _setGeoIpTrackingEnabled (bool enabled);

		[DllImport("__Internal")]
		private static extern void _setGeoIpTrackingDefault (bool enabled);

		#endif
		#endregion
		
		
		#region Marigold SDK methods
		private const string AndroidMarigoldWrapper = "com.marigold.sdk.unity.MarigoldWrapper";

		/// <summary>
		///  Initialises the Marigold SDK on the Unity side and sets up some callbacks.
		/// </summary>
		public void Start()
		{
			#if UNITY_IOS
			Marigold._start ();
			#elif UNITY_ANDROID
			CallAndroid("start");
			#endif
		}
			
		/// <summary>
		/// Forward a location to the Marigold SDK. 
		/// This method can be used when you’re already tracking location in your app and you just want to forward your existing calls to the Marigold SDK.
		/// </summary>
		/// <param name="lat">Lat component of the device location.</param>
		/// <param name="lon">Lon component of the device location.</param>
		public void UpdateLocation(double lat, double lon) {
			#if UNITY_IOS
			Marigold._updateLocation (lat, lon);
			#elif UNITY_ANDROID
			CallAndroid("updateLocation", lat, lon);
			#endif
		}

		/// <summary>
		/// Asyncronously gets the device ID.
		/// String of ID will be called back with an OnDeviceIdReceivedEvent - add a handler to handle this.
		/// Errors will be called back with an OnErrorEvent - add a handler to handle this. 
		/// </summary>
		public void DeviceID () {
			#if UNITY_IOS
			Marigold._deviceID ();
			#elif UNITY_ANDROID
			CallAndroid("deviceId");
			#endif
		}

	#nullable enable
		/// <summary>
		/// Logs a registration event to sign a user in or out. Provide the user ID for login
		/// or a null/empty string for logout.
		/// </summary>
		/// <param name="userID">The string value of the user ID.</param>
		public void LogRegistrationEvent(string? userID){
			#if UNITY_IOS
			Marigold._logRegistrationEvent(userID);
			#elif UNITY_ANDROID
			CallAndroid("logRegistrationEvent", userID);
			#endif
		}
	#nullable disable

		/// <summary>
		/// Enables or disables the showing of in-app notifications.
		/// </summary>
		/// <param name="enabled">A boolean value indicating whether in-app notfications are enabled</param>
		public void SetInAppNotificationsEnabled (bool enabled) {
			#if UNITY_IOS
			Marigold._setInAppNotificationsEnabled (enabled);
			#elif UNITY_ANDROID
			CallAndroid("setInAppNotificationsEnabled", enabled);
			#endif
		}

		/// <summary>
		/// Asyncronously enable or disable location tracking based on IP Address. Tracking location tracking
		/// is enabled by default.
		/// Use this method for users who may not want to have their location tracked at all. 
		/// Errors will be called back with an OnErrorEvent - add a handler to handle this.
		/// </summary>
		/// <param name="enabled">A boolean value indicating whether or not to disable location based on IP Address.</param>
		public void SetGeoIpTrackingEnabled (bool enabled) {
			#if UNITY_IOS
			Marigold._setGeoIpTrackingEnabled (enabled);
			#elif UNITY_ANDROID
			CallAndroid("setGeoIpTrackingEnabled", enabled);
			#endif
		}

		/// <summary>
		/// Set whether location tracking based on IP Address will be enabled or disabled by default when a device is created.
		/// This method must be called before startEngine.
		/// </summary>
		/// <param name="enabled">A boolean value indicating whether or not location based on IP Address should be enabled by default.</param>
		public void SetGeoIpTrackingDefault (bool enabled) {
			#if UNITY_IOS
			Marigold._setGeoIpTrackingEnabled (enabled);
			#elif UNITY_ANDROID
			CallAndroid("setGeoIpTrackingEnabled", enabled);
			#endif
		}

		private void CallAndroid(string methodName, params object[] args) {
			using (AndroidJavaClass androidWrapper = new AndroidJavaClass(AndroidMarigoldWrapper))
			using (AndroidJavaObject instance = androidWrapper.GetStatic<AndroidJavaObject> ("INSTANCE"))
			{
				instance.Call(methodName, args);
			}
		}

		
		#endregion

		#region Helpers 
		public DateTime UnixTimeStampToDateTime( double unixTimeStamp )
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
			return dtDateTime;
		}

		/// <summary>
		/// Used only by the underlying unity plugin code. Do not call.
		/// </summary>
		public void ReceiveError(string errorDescription) {
			MarigoldErrorEventArgs args = new MarigoldErrorEventArgs ();
			args.ErrorDescription = errorDescription;
			OnErrorEvent.Invoke(this, args);
		}

		/// <summary>
		/// Used only by the underlying unity plugin code. Do not call.
		/// </summary>
		public void ReceiveDeviceID(string deviceID) {
			DeviceIDReceivedEventArgs args = new DeviceIDReceivedEventArgs ();
			args.DeviceID = deviceID;
			OnDeviceIdReceivedEvent.Invoke(this, args);
		}
		
		#endregion
		
		#region Callbacks
		public static event EventHandler<MarigoldErrorEventArgs> OnErrorEvent;
		public static event EventHandler<DeviceIDReceivedEventArgs> OnDeviceIdReceivedEvent;
		#endregion
	}

	/// <summary>
	/// Marigold error event arguments.
	/// </summary>
	public class MarigoldErrorEventArgs :EventArgs {
		public string ErrorDescription { get; set; }
	}

	/// <summary>
	/// Marigold Device ID received event.
	/// </summary>
	public class DeviceIDReceivedEventArgs :EventArgs {
		public string DeviceID { get; set; }
	}
}
