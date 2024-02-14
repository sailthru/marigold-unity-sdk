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

1. Open your project in the Unity Editor
2. Import the `Carnival.unitypackage` file and select all files
3. In the scenes you wish to include Carnival, create an object with the Carnival.cs script attached to it. 


## iOS Instructions

1. To generate your project for iOS, use the File menu to select Build Settings. Switch to the iOS platform and choose Build.
2. In the resulting xcode project:
	* Add the -ObjC linker flag to your Other Linker Flags under Project -> Build Settings.
	* In Build Phases -> Link Binary With Libraries, check the following frameworks are listed; they must be added if they are missing:
	 * UIKit 
	 * Foundation
	 * CoreLocation
	 * CoreGraphics 
	 * AVFoundation
	 * MediaPlayer
	 * QuartzCore
	* Add [Carnival.framework](https://github.com/carnivalmobile/carnival-ios-sdk/tree/master/Carnival.framework) to the Frameworks folder of your project in Xcode. Be sure to check "Copy items into destination groups' folder". 
	* Add Carnival.framework to the Embedded Binaries section in Unity-iPhone -> General
3. Run your application. 


## Android Instructions
1. Go to **File > Build Settings**
2. Select Android, use Gradle as the build system, and check **Export Project**
3. Open the exported in Android Studio 3+. Allow Gradle import. 
4. Open up the Android manifest in **/Assets/Plugins/Android/AndroidManifest.xml** and replace all instances of `${applicationId}` with your Package Name.
5. Update your App's build.gradle to include the following changes:

Delete the first generated line to stop Unity auto-removing the changes. 

To `allProjects > repositories`, add 
```
		maven {
			url "https://github.com/sailthru/maven-repository/raw/master/"
		}

		google()
		mavenCentral()
```
To `dependencies` add:
```
	compile 'com.marigold.sdk:marigold:20.0.0'
```

Inside `android`, set the following fields:
```
	compileSdk 34
	buildToolsVersion = '34.0.0'

	defaultConfig {
		targetSdk 34
		applicationId 'com.marigold.sdk.unitytestapp'
		multiDexEnabled true
	}
```

Finally, in `buildscript`, set the following fields
```
buildscript {
	repositories {
		jcenter()
		google()
		mavenCentral()
	}

	dependencies {
		classpath 'com.android.tools.build:gradle:3.0.0'
		classpath 'com.jfrog.bintray.gradle:gradle-bintray-plugin:1.4'
		classpath 'com.github.dcendents:android-maven-gradle-plugin:1.3'
	}
}
```

This will add Carnival and it's dependencies. Gradle sync should complete, and you should be able to run your Unity application on a device or emulator. 


## Documentation

More documentation can be found at [docs.carnival.io](docs.carnival.io).
