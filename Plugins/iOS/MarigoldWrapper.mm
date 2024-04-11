#import "MarigoldWrapper.h"
#import <Marigold/Marigold.h>
#import <UserNotifications/UserNotifications.h>
#import <Foundation/Foundation.h>

const char *MAR_MARIGOLD = "Marigold";
const char *MAR_RECEIVE_ERROR = "ReceiveError";
const char *MAR_RECEIVE_DEVICE_ID = "ReceiveDeviceID";

@interface Marigold ()
- (void)setWrapperName:(NSString *)wrapperName andVersion:(NSString *)wrapperVersion;
@end

static MarigoldWrapper * _sharedInstance = nil;
static dispatch_once_t onceSharedPredicate = 0;

@interface MarigoldWrapper ()
/*
 * We need to hold these blocks to make sure they are not released 
 * by ARC before they're executed and the scope variable are destroyed. 
 * Seems to be unique to the Unity runtime and Objective-C++.
 */
@property (nonatomic, copy) void (^errorBlock)(NSError *error);
@property (nonatomic, copy) void (^deviceIDBlock)(NSString *deviceID, NSError *error);
@property (nonatomic, strong) Marigold *marigold;

+ (MarigoldWrapper *)shared;

@end

@implementation MarigoldWrapper

# pragma mark - C Methods

void _start() {
    [[MarigoldWrapper shared] setWrapperDetails];
}

# pragma mark Location

void _updateLocation(double lat, double lon) {
    [[MarigoldWrapper shared] updateLocationWithLatitude:lat longitude:lon];
}

#pragma mark Device ID 

void _deviceID() {
    [[MarigoldWrapper shared] deviceID];
}

# pragma mark Registration

void _logRegistrationEvent (const char *userID) {
    NSString *userIdString = nil;
    if (userID) {
        userIdString = [NSString stringWithUTF8String:userID];
    }
    [[MarigoldWrapper shared] logRegistrationEventWithUserId:userIdString];
}

# pragma mark - In App Notifications

void _setInAppNotificationsEnabled(bool enabled) {
    [[MarigoldWrapper shared] setInAppNotificationsEnabled:enabled];
}

# pragma mark - Geo IP tracking

void _setGeoIpTrackingEnabled (bool enabled) {
    [[MarigoldWrapper shared] setGeoIpTrackingEnabled:enabled];
}

# pragma mark Notification Permissions

void _requestNotificationPermission() {
    [[MarigoldWrapper shared] requestNotificationPermission];
}

void _syncNotificationSettings() {
    [[MarigoldWrapper shared] syncNotificationSettings];
}

# pragma mark - Obj-C Methods

# pragma mark Init

+ (MarigoldWrapper *)shared {
    dispatch_once(&onceSharedPredicate, ^{
        if (!_sharedInstance) {
            _sharedInstance = [[self alloc] init];
        }
    });
    
    return _sharedInstance;
}

- (id) init {
    self = [super init];
    if (self) {
        self.marigold = [Marigold new];
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
            NSString *idString = deviceID;
            if (!idString) {
                idString = @"";
            }
            UnitySendMessage(MAR_MARIGOLD, MAR_RECEIVE_DEVICE_ID, [idString UTF8String]);
        }
    };
}

#pragma mark Wrapper

- (void)setWrapperDetails {
    [self.marigold setWrapperName:@"Unity" andVersion:@"2.1.1"];
}

# pragma mark Location

- (void)updateLocationWithLatitude:(double)lat longitude:(double)lon {
    CLLocation *loc = [[CLLocation alloc] initWithLatitude:lat longitude:lon];
    [self.marigold updateLocation:loc];
}

#pragma mark Device ID

- (void)deviceID {
    [self.marigold deviceID:self.deviceIDBlock];
}

# pragma mark Registration

- (void)logRegistrationEventWithUserId:(NSString *)userID {
    [self.marigold logRegistrationEvent:userID];
}

# pragma mark - In App Notifications

- (void)setInAppNotificationsEnabled:(bool)enabled {
    [self.marigold setInAppNotificationsEnabled:enabled];
}

# pragma mark - Geo IP tracking

- (void)setGeoIpTrackingEnabled:(bool)enabled {
    [self.marigold setGeoIPTrackingEnabled:enabled withResponse:self.errorBlock];
}

#pragma mark Notification Permission

- (void)requestNotificationPermission {
    UNAuthorizationOptions options = UNAuthorizationOptionAlert | UNAuthorizationOptionBadge | UNAuthorizationOptionSound;
    [[UNUserNotificationCenter currentNotificationCenter] requestAuthorizationWithOptions:options completionHandler:^(BOOL granted, NSError * _Nullable error) {}];

    [self dispatchOnMainQueue:^{
        if(![[UIApplication sharedApplication] isRegisteredForRemoteNotifications]) {
            [[UIApplication sharedApplication] registerForRemoteNotifications];
        }
    }];
}

- (void)syncNotificationSettings {
    [self.marigold syncNotificationSettings];
}

- (void)dispatchOnMainQueue:(dispatch_block_t) block {
    if (!block) return;
    
    if ([NSThread isMainThread]) {
        block();
    } else {
        [[NSOperationQueue mainQueue] addOperationWithBlock:block];
    }
}

@end
