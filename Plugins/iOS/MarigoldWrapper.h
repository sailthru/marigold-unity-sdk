#include <stdint.h>
#import <Foundation/Foundation.h>

@interface MarigoldWrapper : NSObject {}

extern "C" void _start();

extern "C" void _updateLocation(double lat, double lon);

extern "C" void _deviceID();

extern "C" void _logRegistrationEvent (const char * userID);

extern "C" void _setInAppNotificationsEnabled(bool enabled);

extern "C" void _setGeoIpTrackingEnabled (bool enabled);

extern "C" void _setGeoIpTrackingDefault (bool enabled);

extern "C" void _requestNotificationPermission();

extern "C" void _syncNotificationSettings();

@end
