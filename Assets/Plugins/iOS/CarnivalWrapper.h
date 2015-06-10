#import <Carnival/Carnival.h>
#include <stdint.h>

@class CarnivalWrapper;

CarnivalWrapper *carnivalInstance;


@interface CarnivalWrapper : NSObject
{
    
}

extern "C" void _startEngine(char *apiKey);

extern "C" void _setTags(char *tagString);
extern "C" void _getTags();

extern "C" void _showMessageStream();

extern "C" void _updateLocation(double lat, double lon);

extern "C" void _logEvent(const char *event);

extern "C" void _logEvent(const char *event);

extern "C" void _setString(const char *string, const char *key);
extern "C" void _setBool(bool boolValue, const char *key);
extern "C" void _setDate(int64_t secondsSince1970, const char *key);
extern "C" void _setFloat(float floatValue, const char *key);
extern "C" void _setInteger(int64_t intValue, const char *key);
extern "C" void _removeAttribute(const char *key);

//Device ID

@end