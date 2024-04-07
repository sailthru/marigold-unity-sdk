#include <stdint.h>
#import <Foundation/Foundation.h>

@interface MessageStreamWrapper : NSObject {}

extern "C" void _unreadCount();

extern "C" void _messages ();

extern "C" void _showMessageDetail(const char *messageJSON);

extern "C" void _dismissMessageDetail();

extern "C" void _registerImpression(const char *messageJSON, int impressionType);

extern "C" void _removeMessage(const char *messageJSON);

extern "C" void _clearMessages();

extern "C" void _markMessageAsRead(const char *messageJSON);

extern "C" void _markMessagesAsRead(const char *messagesJSON);

@end
