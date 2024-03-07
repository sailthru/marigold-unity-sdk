# Carnival Unity SDK

## Purpose
This Unity Plugin allows you to communicate with Carnival and show the Carnival mesage stream to enable Rich, Targeted Messaging in your App.

## Setup

Required Tools:
- [Unity](https://unity3d.com/get-unity)
	- At installation time, select Android and iOS plugins for downloading as well.
- [Xcode](https://itunes.apple.com/nz/app/xcode/id497799835?mt=12)
- [Android Studio](https://developer.android.com/studio/index.html)

## Integration Instructions

1. Open your project in the Unity Editor.
2. Add the Marigold Unity SDK to your project using the Unity Package Manager.
3. In the scenes you wish to include Marigold, create an object with the Marigold.cs script attached to it. You can also add MessageStream.cs or EngageBySailthru.cs if you need functionality from these scripts.


## iOS Instructions

1. To generate your project for iOS, use the File menu to select Build Settings. Switch to the iOS platform and choose Build.
2. In the resulting xcode project:
	* In Build Phases -> Link Binary With Libraries, check the following frameworks are listed; they must be added if they are missing:
	 * CoreLocation
	* Add the Marigold.xcframework to the project in Xcode. It can be obtained through Swift Package Manager, Cocoapods, Carthage or directly from Github. See our [documentation](https://docs.mobile.sailthru.com/docs/ios-integration) for more details.
	* Add Marigold.xcframework to the Frameworks and Libraries section in Unity-iPhone -> General. You should add the framework to both your app target and the `UnityFramework` target.
	* Call `startEngine` on the `Marigold` class using your SDK key during the `application:didFinishLaunchingWithOptions:` method which will implemented in the app's `UnityAppController.m` file.
3. Run your application. 


## Android Instructions
1. Go to **File > Build Settings**
2. Select Android, use Gradle as the build system, and check **Export Project**
3. Open the exported in Android Studio. Allow Gradle import. 
4. Open up the Android manifest in **/Assets/Plugins/Android/AndroidManifest.xml** and replace all instances of `${applicationId}` with your Package Name.
5. Update your App's build.gradle to include the following changes:

Delete the first generated line to stop Unity auto-removing the changes. 

To `repositories`, add 
```
	maven {
		url "https://github.com/sailthru/maven-repository/raw/master/"
	}
```
To `dependencies` add:
```
	implementation 'com.marigold.sdk:marigold:20.1.0'
```

This will add Marigold and its dependencies. Gradle sync should complete, and you should be able to run your Unity application on a device or emulator. 

You should then add an Application class to your app and call `startEngine` with your SDK key in the `onCreate` method.
You will also need to override the default notification configuration using the example code below. This prevents the Unity
lifecycle handling and the Marigold SDK in-app message handling from interfering with one another.

Application class:
```
import android.app.Application
import android.app.PendingIntent
import android.content.Intent
import com.marigold.sdk.Marigold
import com.marigold.sdk.NotificationConfig
import com.unity3d.player.UnityPlayerActivity
import java.util.Date

class TestApplication: Application() {
    override fun onCreate() {
        super.onCreate()
        Marigold().startEngine(this, "<your-sdk-key>")
        // Override the default intent for notification handling. This is important to
        // prevent duplicate opens when using combined push/in-app.
        val intent = Intent(applicationContext, UnityPlayerActivity::class.java)
        val requestCode = Date().time.toInt()
        Marigold().setNotificationConfig(NotificationConfig().apply {
            setDefaultContentIntent(intent, requestCode, PendingIntent.FLAG_UPDATE_CURRENT)
        })
    }
}
```

Then add the application class to the AndroidManifest:
```
<application android:label="@string/app_name" android:icon="@mipmap/app_icon" android:banner="@drawable/app_banner" android:name=".TestApplication" />
```


## Documentation

More documentation can be found in our [docs](https://docs.mobile.sailthru.com).
