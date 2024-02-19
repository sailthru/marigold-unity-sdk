#include <stdint.h>

@class MarigoldWrapper;

MarigoldWrapper *marigoldInstance;


@interface MarigoldWrapper : NSObject
{
    
}

extern "C" void _start();

extern "C" void _updateLocation(double lat, double lon);

extern "C" void _deviceID();

extern "C" void _logRegistrationEvent (const char * userID);

extern "C" void _setInAppNotificationsEnabled(bool enabled);

extern "C" void _setGeoIpTrackingEnabled (bool enabled);

extern "C" void _setGeoIpTrackingDefault (bool enabled);

extern "C" void _unreadCount();

extern "C" void _messages ();

extern "C" void _showMessageDetail(char *messageJSON);

extern "C" void _dismissMessageDetail();

extern "C" void _registerImpression(const char *messageJSON, int impressionType);

extern "C" void _removeMessage(const char *messageJSON);

extern "C" void _markMessageAsRead(char *messageJSON);

@end
