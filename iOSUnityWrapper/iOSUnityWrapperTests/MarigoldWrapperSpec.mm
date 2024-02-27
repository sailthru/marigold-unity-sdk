//
//  MarigoldWrapperSpec.m
//  iOSUnityWrapperTests
//
//  Created by Ian Stewart on 21/02/24.
//

#import <XCTest/XCTest.h>
#import <OCMock/OCMock.h>
#import <Marigold/Marigold.h>
#import "MarigoldWrapper.h"
#import "MarigoldWrapper+Private.h"
#import "UnityMessage.h"

@interface Marigold ()
- (void)setWrapperName:(NSString *)wrapperName andVersion:(NSString *)wrapperVersion;
@end

@interface MarigoldWrapperSpec : XCTestCase

@property (strong, nonatomic) Marigold *mockMarigold;
@property (strong, nonatomic) NSError *error;

@end

@implementation MarigoldWrapperSpec

- (void)setUp {
    self.mockMarigold = OCMClassMock([Marigold class]);
    self.error = [NSError errorWithDomain:@"test error message" code:1 userInfo:nil];
    [MarigoldWrapper shared].marigold = self.mockMarigold;
}

- (void)tearDown {
    [[UnitySender shared].messages removeAllObjects];
}

- (void)testStart {
    _start();
    OCMVerify([self.mockMarigold setWrapperName:@"Unity" andVersion:[OCMArg isNotNil]]);
}

- (void)testUpdateLocation {
    _updateLocation(1.2, 3.4);
    OCMVerify([self.mockMarigold updateLocation:[OCMArg checkWithBlock:^BOOL(CLLocation *obj) {
        if (obj.coordinate.latitude != 1.2) {
            return NO;
        }
        if (obj.coordinate.longitude != 3.4) {
            return NO;
        }
        return YES;
    }]]);
}

- (void)testDeviceID {
    _deviceID();
    OCMVerify([self.mockMarigold deviceID: [MarigoldWrapper shared].deviceIDBlock]);
}


# pragma mark Registration

- (void)testLogRegistrationEvent {
    const char *testId = "user ID";
    NSString *expectedId = @"user ID";
    _logRegistrationEvent(testId);
    OCMVerify([self.mockMarigold logRegistrationEvent:expectedId]);
}

# pragma mark - In App Notifications

- (void)testSetInAppNotificationsEnabled {
    _setInAppNotificationsEnabled(true);
    OCMVerify([self.mockMarigold setInAppNotificationsEnabled:YES]);
}

# pragma mark - Geo IP tracking

- (void)testSetGeoIpTrackingEnabled {
    _setGeoIpTrackingEnabled(true);
    OCMVerify([self.mockMarigold setGeoIPTrackingEnabled:YES withResponse:[MarigoldWrapper shared].errorBlock]);
}

- (void)testSetGeoIpTrackingDefault {
    _setGeoIpTrackingDefault(true);
    OCMVerify([self.mockMarigold setGeoIPTrackingDefault:YES]);
}

- (void)testErrorBlockWithNil {
    [MarigoldWrapper shared].errorBlock(nil);
    XCTAssertEqual(0, [[UnitySender shared].messages count]);
}

- (void)testErrorBlockWithError {
    [MarigoldWrapper shared].errorBlock(self.error);
    [[UnitySender shared] checkMessageContainsWithObject:@"Marigold" method:@"ReceiveError" message:@"test error message"];
}

- (void)testDeviceIdBlockWithNil {
    [MarigoldWrapper shared].deviceIDBlock(nil, nil);
    [[UnitySender shared] checkMessageEqualsWithObject:@"Marigold" method:@"ReceiveDeviceID" message:@""];
}

- (void)testDeviceIdBlockWithDeviceId {
    [MarigoldWrapper shared].deviceIDBlock(@"Device ID", nil);
    [[UnitySender shared] checkMessageEqualsWithObject:@"Marigold" method:@"ReceiveDeviceID" message:@"Device ID"];
}

- (void)testDeviceIdBlockWithError {
    [MarigoldWrapper shared].deviceIDBlock(nil, self.error);
    [[UnitySender shared] checkMessageContainsWithObject:@"Marigold" method:@"ReceiveError" message:@"test error message"];
}


@end
