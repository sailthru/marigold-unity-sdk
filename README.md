# Carnival Unity SDK

## Purpose
This Unity Plugin allows you to communicate with Carnival and show the Carnival mesage stream to enable Rich, Targeted Messaging in your App.

## Integration Instructions

1. Open your project in the Unity Editor
2. Import the `Carnival.unitypackage` file and select all files
3. In the scenes you wish to include Carnival, create an object with the Carnival.cs script attached to it. 


## iOS Instructions

1. To generate your project for iOS, use the File menu to select Build Settings. Switch to the iOS platform and choose Build.
2. In the resulting xcode project:
	* Add the -ObjC linker flag to your Other Linker Flags under Project -> Build Settings.
	* Add the required frameworks:
	 * UIKit 
	 * Foundation
	 * CoreLocation
	 * CoreGraphics 
	 * AVFoundation
	 * MediaPlayer
	 * QuartzCore
	* Add [Carnival.embeddedframework](https://github.com/carnivalmobile/carnival-ios-sdk/tree/master/Carnival.embeddedframework) to the Frameworks folder of your project in Xcode. Be sure to check "Copy items into destination groups' folder". 
3. Run your application. 

## Android Instructions

1. Find your applications "Bundle Identifier", this can be found in the Player Settings pane, accessible from **File > Build Settings**, selecting the Android platform and clicking "Player Settings". The Bundle Identifier appears under the Identification header in the Inspector.
2. Open up the Android manifest in **/Assets/Plugins/Android/AndroidManifest.xml** and replace all instances of `${applicationId}` with your Bundle Identifier.
3. Run your application.

## Documentation

More documentation can be found at [docs.carnival.io](http://docs.carnival.io).
