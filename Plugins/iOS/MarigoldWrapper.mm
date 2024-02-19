#import "MarigoldWrapper.h"
#import <Marigold/Marigold.h>
#import <Foundation/Foundation.h>

const char *MAR_MARIGOLD = "Marigold";
const char *MAR_RECEIVE_ERROR = "ReceiveError";
const char *MAR_RECEIVE_DEVICE_ID = "ReceiveDeviceID";
const char *MAR_RECEIVE_UNREAD_COUNT = "ReceiveUnreadCount";
const char *MAR_RECEIVE_MESSAGES = "ReceiveMessagesJSONData";

@interface MARMessage ()

- (nullable instancetype)initWithDictionary:(nonnull NSDictionary *)dictionary;
- (nonnull NSDictionary *)dictionary;

@end

@interface Marigold ()
- (void)setWrapperName:(NSString *)wrapperName andVersion:(NSString *)wrapperVersion;
@end

@interface MarigoldWrapper ()
/*
 * We need to hold these blocks to make sure they are not released 
 * by ARC before they're executed and the scope variable are destroyed. 
 * Seems to be unique to the Unity runtime and Objective-C++.
 */
@property (nonatomic, copy) void (^errorBlock)(NSError *error);
@property (nonatomic, copy) void (^messagesBlock)(NSArray *messages, NSError *error);
@property (nonatomic, copy) void (^deviceIDBlock)(NSString *deviceID, NSError *error);
@property (nonatomic, copy) void (^unreadCountBlock)(NSUInteger unreadCount, NSError *error);
@property (nonatomic, strong) NSArray *messages;
@property (nonatomic, strong) Marigold *marigold;
@property (nonatomic, strong) MARMessageStream *messageStream;
@end

@implementation MarigoldWrapper

# pragma mark - C Methods

/*
 * Use of marigoldInstance is to effectively call self inside C++ methods, which you can't do.
 */
void initMarigold () {
    if (!marigoldInstance) {
        marigoldInstance = [[MarigoldWrapper alloc] init];
    }
}

void _start() {
    initMarigold();
    [marigoldInstance.marigold setWrapperName:@"Unity" andVersion:@"1.0.0"];
}

# pragma mark Location

void _updateLocation(double lat, double lon) {
    initMarigold();
    CLLocation *loc = [[CLLocation alloc] initWithLatitude:lat longitude:lon];
    [marigoldInstance.marigold updateLocation:loc];
}

#pragma mark Device ID 

void _deviceID() {
    initMarigold();
    [marigoldInstance deviceID];
}

# pragma mark Registration

void _logRegistrationEvent (const char *userID) {
    initMarigold();
    NSString *userIdString = nil;
    if (userID) {
        userIdString = [NSString stringWithUTF8String:userID];
    }
    [marigoldInstance.marigold logRegistrationEvent:userIdString];
}

# pragma mark - In App Notifications

void _setInAppNotificationsEnabled(bool enabled) {
    initMarigold();
    [marigoldInstance.marigold setInAppNotificationsEnabled:enabled];
}

# pragma mark - Geo IP tracking

void _setGeoIpTrackingEnabled (bool enabled) {
    initMarigold();
    [marigoldInstance setGeoIpEnabled:enabled];
}

void _setGeoIpTrackingDefault (bool enabled) {
    initMarigold();
    [marigoldInstance.marigold setGeoIPTrackingDefault:enabled];
}

# pragma mark - Unread Count

void _unreadCount() {
    initMarigold();
    [marigoldInstance unreadCount];
}

# pragma mark Messages

void _messages () {
    initMarigold();
    [marigoldInstance sendMessages];
}

void _showMessageDetail(char *messageJSON) {
    initMarigold();
    MARMessage *message = [marigoldInstance messageFromJSON:[NSString stringWithUTF8String:messageJSON]];
    [marigoldInstance.messageStream presentMessageDetailForMessage:message];
}

void _dismissMessageDetail() {
    initMarigold();
    [marigoldInstance.messageStream dismissMessageDetail];
}

void _registerImpression(const char *messageJSON, int impressionType) {
    initMarigold();
    NSString *messageJsonString = [NSString stringWithUTF8String:messageJSON];
    NSLog(@"Message JSON String: %@", messageJsonString);
    MARMessage *message = [marigoldInstance messageFromJSON:messageJsonString];
    if (impressionType == 0) {
        [marigoldInstance.messageStream registerImpressionWithType:MARImpressionTypeInAppNotificationView forMessage:message];
    } else if (impressionType == 1) {
         [marigoldInstance.messageStream registerImpressionWithType:MARImpressionTypeStreamView forMessage:message];
    } else if (impressionType == 2){
        [marigoldInstance.messageStream registerImpressionWithType:MARImpressionTypeDetailView forMessage:message];
    } else {
        NSLog(@"Impression type not supported");
    }
}

void _removeMessage(const char *messageJSON) {
    initMarigold();
    [marigoldInstance removeMessage:[marigoldInstance messageFromJSON:[NSString stringWithUTF8String:messageJSON]]];
}

void _markMessageAsRead(char *messageJSON) {
    initMarigold();
    MARMessage *message = [marigoldInstance messageFromJSON:[NSString stringWithUTF8String:messageJSON]];
    [marigoldInstance markMessageAsRead:message];
}

# pragma mark - Obj-C Methods

# pragma mark Init

- (id) init {
    self = [super init];
    if (self) {
        self.marigold = [Marigold new];
        self.messageStream = [MARMessageStream new];
        [self setupCallbacks];
    }
    return self;
}

- (void)setupCallbacks {
    self.errorBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage(MAR_MARIGOLD, MAR_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        }
    };
    
    self.deviceIDBlock = ^(NSString *deviceID, NSError *error) {
        if (error) {
            UnitySendMessage(MAR_MARIGOLD, MAR_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        } else {
            UnitySendMessage(MAR_MARIGOLD, MAR_RECEIVE_DEVICE_ID, [deviceID UTF8String]);
        }
    };
    
    self.unreadCountBlock = ^(NSUInteger unreadCount, NSError *error) {
        if (error) {
            UnitySendMessage(MAR_MARIGOLD, MAR_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        }
        UnitySendMessage(MAR_MARIGOLD, MAR_RECEIVE_UNREAD_COUNT, [[NSString stringWithFormat:@"%lu", (unsigned long)unreadCount] UTF8String]);
    };
    
    __weak __typeof__(self) weakSelf = self;
    self.messagesBlock = ^(NSArray *messages, NSError *error) {
        if (error) {
            UnitySendMessage(MAR_MARIGOLD, MAR_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        } else {
            __weak __typeof__(self) strongSelf = weakSelf;
            if (strongSelf) {
                NSData *jsonData = [NSJSONSerialization dataWithJSONObject:[strongSelf arrayOfMessageDictionariesFromMessageArray:messages] options:NSJSONWritingPrettyPrinted error:nil];
                NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                UnitySendMessage(MAR_MARIGOLD, MAR_RECEIVE_MESSAGES, [jsonString UTF8String]);
            }
        }
    };
}

# pragma mark Device ID

- (void)deviceID {
    [self.marigold deviceID:self.deviceIDBlock];
}

# pragma mark Geo IP Tracking

- (void)setGeoIpEnabled:(BOOL)enabled {
    [self.marigold setGeoIPTrackingEnabled:enabled withResponse:self.errorBlock];
}

# pragma mark  Unread Count

- (void)unreadCount {
    [self.messageStream unreadCount:self.unreadCountBlock];
}

# pragma mark - Messages

- (void)sendMessages {
    [self.messageStream messages:self.messagesBlock];
}

- (void)removeMessage:(MARMessage *)message {
    [self.messageStream removeMessage:message withResponse:self.errorBlock];
}

- (void)markMessageAsRead:(MARMessage *)message {
    [self.messageStream markMessageAsRead:message withResponse:self.errorBlock];
}

# pragma mark - Helper Methods

- (NSArray *)arrayOfMessageDictionariesFromMessageArray:(NSArray *)messageArray {
    NSMutableArray *messageDictionaries = [NSMutableArray array];
    for (MARMessage *message in messageArray) {
        [messageDictionaries addObject:[message dictionary]];
    }
    return messageDictionaries;
}

- (MARMessage *)messageFromJSON:(NSString *)JSONString {
    NSData *data = [JSONString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *deserializeError = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers error:&deserializeError];
    
    NSLog(@"Message Dict: %@", dict);
    
    if (!deserializeError) {
        return [[MARMessage alloc] initWithDictionary:dict];
    }
    return nil;
}

@end
