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

NSDate *date1 = [NSDate now];
NSDate *date2 = [[NSDate now] dateByAddingTimeInterval:1234];

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

- (void)testSetAttributes {
    NSString *attributesJson = [self createAttributesJsonString];
    _setAttributes([attributesJson UTF8String]);
    OCMVerify([self.mockEngageBySt setAttributes:[OCMArg checkWithSelector:@selector(checkAttributes:) onObject:self] withCompletion:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testSetAttributesWithInvalidJson {
    _setAttributes(invalidJson);
    OCMVerify(never(), [self.mockEngageBySt setAttributes:[OCMArg any] withCompletion:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testRemoveAttribute {
    _removeAttribute(testString);
    OCMVerify([self.mockEngageBySt removeAttributeWithKey:@"howdy" withCompletion:[EngageBySailthruWrapper shared].errorBlock]);
}

- (void)testClearAttributes {
    _clearAttributes();
    OCMVerify([self.mockEngageBySt clearAttributesWithCompletion:[EngageBySailthruWrapper shared].errorBlock]);
}



#pragma mark Helpers

- (void)checkJsonError {
    [[UnitySender shared] checkMessageEqualsWithObject:@"EngageBySailthru" method:@"ReceiveError" message:@"The data couldn’t be read because it isn’t in the correct format."];
}

- (BOOL)checkPurchase:(MARPurchase *)purchase {
    XCTAssertEqual([purchase.purchaseItems count], 1);
    MARPurchaseItem *purchaseItem = [purchase.purchaseItems firstObject];
    XCTAssertEqual([purchaseItem.quantity intValue], 2);
    XCTAssertEqualObjects(purchaseItem.title, @"item name");
    XCTAssertEqual([purchaseItem.price intValue], 1234);
    XCTAssertEqualObjects(purchaseItem.purchaseItemID, @"2345");
    XCTAssertEqualObjects(purchaseItem.URL, [NSURL URLWithString:@"https://www.sailthru.com"]);
    return YES;
}

- (NSString *)createAttributesJsonString {
    NSDictionary *attributes = @{
        @"mergeRule": @1,
        @"attributes": @{
            @"stringAttr": @{
                @"type": @"string",
                @"value": @"testme"
            },
            @"stringsAttr": @{
                @"type": @"stringArray",
                @"value": @[ @"testme1", @"testme2" ]
            },
            @"integerAttr": @{
                @"type": @"integer",
                @"value": @45
            },
            @"integersAttr": @{
                @"type": @"integerArray",
                @"value": @[ @23, @34 ]
            },
            @"floatAttr": @{
                @"type": @"float",
                @"value": @(1.23)
            },
            @"floatsAttr": @{
                @"type": @"floatArray",
                @"value": @[ @(2.34f), @(3.45f) ]
            },
            @"booleanAttr": @{
                @"type": @"boolean",
                @"value": @YES
            },
            @"dateAttr": @{
                @"type": @"date",
                @"value": [self millisecondLongStringForDate:date1]
            },
            @"datesAttr": @{
                @"type": @"dateArray",
                @"value": @[ [self millisecondLongStringForDate:date1], [self millisecondLongStringForDate:date2] ]
            }
        }
    };
    NSError *error;
    NSData *serialisedAttributes = [NSJSONSerialization dataWithJSONObject:attributes options:0 error:&error];
    if (error) {
        throw error;
    }
    return [[NSString alloc] initWithData:serialisedAttributes encoding:NSUTF8StringEncoding];
}

- (NSString *)millisecondLongStringForDate:(NSDate *)date {
    return [NSString stringWithFormat:@"%ld", ((long)[date timeIntervalSince1970] * 1000)];
}

- (BOOL)checkAttributes:(MARAttributes *)attributes {
    XCTAssertEqualObjects([attributes getString:@"stringAttr"], @"testme");
    NSArray *strings = [attributes getStrings:@"stringsAttr"];
    XCTAssertEqualObjects(strings[0], @"testme1");
    XCTAssertEqualObjects(strings[1], @"testme2");
    XCTAssertEqual([attributes getInteger:@"integerAttr" defaultValue:0], 45);
    NSArray *integers = [attributes getIntegers:@"integersAttr"];
    XCTAssertEqual([integers[0] intValue], 23);
    XCTAssertEqual([integers[1] intValue], 34);
    XCTAssertEqualWithAccuracy([attributes getFloat:@"floatAttr" defaultValue:0], 1.23, 0.01);
    NSArray *floats = [attributes getFloats:@"floatsAttr"];
    XCTAssertEqualWithAccuracy([floats[0] floatValue], 2.34, 0.01);
    XCTAssertEqualWithAccuracy([floats[1] floatValue], 3.45, 0.01);
    XCTAssertTrue([attributes getBool:@"booleanAttr" defaultValue:NO]);
    NSDate *date = [attributes getDate:@"dateAttr"];
    XCTAssertEqual(floor([date timeIntervalSince1970]), floor([date1 timeIntervalSince1970]));
    NSArray *dates = [attributes getDates:@"datesAttr"];
    XCTAssertEqual(floor([dates[0] timeIntervalSince1970]), floor([date1 timeIntervalSince1970]));
    XCTAssertEqual(floor([dates[1] timeIntervalSince1970]), floor([date2 timeIntervalSince1970]));
    return YES;
}

@end
