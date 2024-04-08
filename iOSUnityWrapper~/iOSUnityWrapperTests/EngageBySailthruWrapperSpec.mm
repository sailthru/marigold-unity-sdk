//
//  EngageBySailthruWrapperSpec.m
//  iOSUnityWrapperTests
//
//  Created by Ian Stewart on 26/02/24.
//

#import <XCTest/XCTest.h>
#import <OCMock/OCMock.h>
#import <Marigold/Marigold.h>
#import "EngageBySailthruWrapper+Private.h"
#import "UnityMessage.h"

const char *url = "https://www.sailthru.com";
const char *tagsJson = "[\"tag1\", \"tag2\", \"tag3\"]";
const char *invalidJson = "[}";
const char *testString = "howdy";
const char *urlsJson = "[\"https://www.sailthru.com\", \"https://www.sailthru.com/something\"]";
const char *varsJson = "{\"hi\":\"there\"}";
const char *stLinkString = "https://link.varickandvandam.com/click/5afcdea395a7a1540e04604d/aHR0cHM6Ly92YXJpY2thbmR2YW5kYW0uY29tL3Byb2R1Y3RzLzEwODgzNTg/5a7cbd790aea1153738b60f3B81a4ee8d";
const char *stLinkUnwrappedString = "https://varickandvandam.com/products/1088358";
const char *purchaseJson = "{\"items\":[{\"qty\":2,\"title\":\"item name\",\"price\":1234,\"id\":\"2345\",\"url\":\"https://www.sailthru.com\"}]}";

@interface EngageBySailthruWrapperSpec : XCTestCase

@property (strong, nonatomic) EngageBySailthruWrapper *spyEngageStWrapper;
@property (strong, nonatomic) EngageBySailthru *mockEngageBySt;
@property (strong, nonatomic) NSError *error;

@end

@implementation EngageBySailthruWrapperSpec

- (void)setUp {
    id wrapperClassMock = OCMClassMock([EngageBySailthruWrapper class]);
    self.spyEngageStWrapper = OCMPartialMock([EngageBySailthruWrapper shared]);
    [OCMStub(ClassMethod([wrapperClassMock shared])) andReturn:self.spyEngageStWrapper];
    
    self.mockEngageBySt = OCMClassMock([EngageBySailthru class]);
    [OCMStub([self.spyEngageStWrapper engageBySailthru]) andReturn:self.mockEngageBySt];
    
    self.error = [NSError errorWithDomain:@"test error message" code:1 userInfo:nil];
}

- (void)tearDown {
    [[UnitySender shared].messages removeAllObjects];
    [(id)self.spyEngageStWrapper stopMocking];
}

- (void)testTrackPageview {
    _trackPageview(url, nil);
    NSURL *expectedUrl = [NSURL URLWithString:[NSString stringWithUTF8String:url]];
    OCMVerify([self.mockEngageBySt trackPageviewWithUrl:expectedUrl andResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testTrackPageviewWithTags {
    _trackPageview(url, tagsJson);
    NSURL *expectedUrl = [NSURL URLWithString:[NSString stringWithUTF8String:url]];
    NSArray *expectedTags = @[@"tag1", @"tag2", @"tag3"];
    OCMVerify([self.mockEngageBySt trackPageviewWithUrl:expectedUrl andTags:expectedTags andResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testTrackPageviewWithTagsError {
    _trackPageview(url, invalidJson);
    OCMVerify(never(), [self.mockEngageBySt trackPageviewWithUrl:[OCMArg any] andResponse:[OCMArg any]]);
    OCMVerify(never(), [self.mockEngageBySt trackPageviewWithUrl:[OCMArg any] andTags:[OCMArg any] andResponse:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testTrackImpression {
    _trackImpression(testString, nil);
    NSString *expectedSection = @"howdy";
    OCMVerify([self.mockEngageBySt trackImpressionWithSection:expectedSection andResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testTrackPImpressionWithUrls {
    _trackImpression(testString, urlsJson);
    NSString *expectedSection = @"howdy";
    NSArray *expectedUrls = @[
        [NSURL URLWithString:@"https://www.sailthru.com"],
        [NSURL URLWithString:@"https://www.sailthru.com/something"]
    ];
    OCMVerify([self.mockEngageBySt trackImpressionWithSection:expectedSection andUrls:expectedUrls andResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testTrackImpressionWithUrlsError {
    _trackImpression(testString, invalidJson);
    OCMVerify(never(), [self.mockEngageBySt trackImpressionWithSection:[OCMArg any] andResponse:[OCMArg any]]);
    OCMVerify(never(), [self.mockEngageBySt trackImpressionWithSection:[OCMArg any] andUrls:[OCMArg any] andResponse:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testTrackClick {
    _trackClick(testString, url);
    NSString *expectedSection = @"howdy";
    NSURL *expectedUrl = [NSURL URLWithString:[NSString stringWithUTF8String:url]];
    OCMVerify([self.mockEngageBySt trackClickWithSection:expectedSection andUrl:expectedUrl andResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testSetUserId {
    _setUserId(testString);
    NSString *expectedUserId = [NSString stringWithUTF8String:testString];
    OCMVerify([self.mockEngageBySt setUserId:expectedUserId withResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testSetUserEmail {
    _setUserEmail(testString);
    NSString *expectedUserEmail = [NSString stringWithUTF8String:testString];
    OCMVerify([self.mockEngageBySt setUserEmail:expectedUserEmail withResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testLogEvent {
    _logEvent(testString, nil);
    NSString *expectedName = [NSString stringWithUTF8String:testString];
    OCMVerify([self.mockEngageBySt logEvent:expectedName withVars:nil]);
}

- (void)testLogEventWithVars {
    _logEvent(testString, varsJson);
    NSString *expectedName = [NSString stringWithUTF8String:testString];
    NSDictionary *expectedVars = @{@"hi":@"there"};
    OCMVerify([self.mockEngageBySt logEvent:expectedName withVars:expectedVars]);
}

- (void)testLogEventWithInvalidVars {
    _logEvent(testString, invalidJson);
    OCMVerify(never(), [self.mockEngageBySt logEvent:[OCMArg any] withVars:[OCMArg any]]);
    
    [self checkJsonError];
}

- (void)testClearEvents {
    _clearEvents();
    OCMVerify([self.mockEngageBySt clearEventsWithResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testSetProfileVars {
    _setProfileVars(varsJson);
    NSDictionary *expectedVars = @{@"hi":@"there"};
    OCMVerify([self.mockEngageBySt setProfileVars:expectedVars withResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testSetProfileVarsWithInvalidVars {
    _setProfileVars(invalidJson);
    OCMVerify(never(), [self.mockEngageBySt setProfileVars:[OCMArg any] withResponse:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testGetProfileVars {
    _getProfileVars();
    OCMVerify([self.mockEngageBySt getProfileVarsWithResponse:[EngageBySailthruWrapper shared].profileVarsBlock]);
}

- (void)testHandleSailthruLink {
    NSString *unwrappedLinkString = [NSString stringWithUTF8String:stLinkUnwrappedString];
    NSURL *unwrappedLink = [NSURL URLWithString:unwrappedLinkString];
    [OCMStub([self.mockEngageBySt handleSailthruLink:[OCMArg any]]) andReturn:unwrappedLink];
    _handleSailthruLink(stLinkString);
    NSURL *expectedUrl = [NSURL URLWithString:[NSString stringWithUTF8String:stLinkString]];
    OCMVerify([self.mockEngageBySt handleSailthruLink:expectedUrl]);
    [[UnitySender shared] checkMessageEqualsWithObject:@"EngageBySailthru" method:@"ReceiveUnwrappedLink" message:unwrappedLinkString];
}

- (void)testHandleSailthruLinkInvalidLink {
    _handleSailthruLink(testString);
    OCMVerify([self.mockEngageBySt handleSailthruLink:[OCMArg any]]);
    [[UnitySender shared] checkMessageEqualsWithObject:@"EngageBySailthru" method:@"ReceiveError" message:@"Provided link is not in a valid format"];
}

- (void)testLogPurchase {
    _logPurchase(purchaseJson);
    OCMVerify([self.mockEngageBySt logPurchase:[OCMArg checkWithSelector:@selector(checkPurchase:) onObject:self] withResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testLogPurchaseWithInvalidJson {
    _logPurchase(invalidJson);
    OCMVerify(never(), [self.mockEngageBySt logPurchase:[OCMArg any] withResponse:[OCMArg any]]);
}

- (void)testLogAbandonedCart {
    _logAbandonedCart(purchaseJson);
    OCMVerify([self.mockEngageBySt logAbandonedCart:[OCMArg checkWithSelector:@selector(checkPurchase:) onObject:self] withResponse:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testErrorBlockWithNil {
    [EngageBySailthruWrapper shared].errorBlock(nil);
    XCTAssertEqual(0, [[UnitySender shared].messages count]);
}

- (void)testErrorBlockWithError {
    [EngageBySailthruWrapper shared].errorBlock(self.error);
    [[UnitySender shared] checkMessageContainsWithObject:@"EngageBySailthru" method:@"ReceiveError" message:@"test error message"];
}

- (void)testProfileVarsBlockWithNil {
    [EngageBySailthruWrapper shared].profileVarsBlock(nil, nil);
    [[UnitySender shared] checkMessageEqualsWithObject:@"EngageBySailthru" method:@"ReceiveProfileVars" message:@"{}"];
}

- (void)testProfileVarsBlockWithVars {
    NSDictionary *expectedVars = @{@"hi":@"there"};
    [EngageBySailthruWrapper shared].profileVarsBlock(expectedVars, nil);
    [[UnitySender shared] checkMessageEqualsWithObject:@"EngageBySailthru" method:@"ReceiveProfileVars" message:@"{\"hi\":\"there\"}"];
}

- (void)testProfileVarsBlockWithError {
    [EngageBySailthruWrapper shared].profileVarsBlock(nil, self.error);
    [[UnitySender shared] checkMessageContainsWithObject:@"EngageBySailthru" method:@"ReceiveError" message:@"test error message"];
}



#pragma mark Helpers

- (void)checkJsonError {
    [[UnitySender shared] checkMessageEqualsWithObject:@"EngageBySailthru" method:@"ReceiveError" message:@"The data couldn’t be read because it isn’t in the correct format."];
}

- (BOOL)checkPurchase:(MARPurchase *)purchase {
    if ([purchase.purchaseItems count] != 1) {
        return NO;
    }
    MARPurchaseItem *purchaseItem = [purchase.purchaseItems firstObject];
    if (![purchaseItem.quantity isEqualToNumber:@2]) {
        return NO;
    }
    if (![purchaseItem.title isEqualToString:@"item name"]) {
        return NO;
    }
    if (![purchaseItem.price isEqualToNumber:@1234]) {
        return NO;
    }
    if (![purchaseItem.purchaseItemID isEqualToString:@"2345"]) {
        return NO;
    }
    if (![purchaseItem.URL isEqual:[NSURL URLWithString:@"https://www.sailthru.com"]]) {
        return NO;
    }
    return YES;
}

@end
