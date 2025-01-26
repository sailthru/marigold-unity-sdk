#include <stdint.h>
#import <Foundation/Foundation.h>

@interface EngageBySailthruWrapper : NSObject {}

extern "C" void _trackPageview(const char *url, const char *tags);

extern "C" void _trackImpression(const char *sectionId, const char *urls);

extern "C" void _trackClick(const char *sectionId, const char *url);

extern "C" void _setUserId(const char *userID);

extern "C" void _setUserEmail(const char *userEmail);

extern "C" void _logEvent(const char *event, const char *varsString);

extern "C" void _clearEvents();

extern "C" void _setProfileVars(const char *varsString);

extern "C" void _getProfileVars();

extern "C" void _handleSailthruLink(const char *linkString);

extern "C" void _logPurchase(const char *purchaseString);

extern "C" void _logAbandonedCart(const char *purchaseString);

extern "C" void _setAttributes(const char *attributesString);

extern "C" void _removeAttribute(const char *key);

extern "C" void _clearAttributes();

@end
