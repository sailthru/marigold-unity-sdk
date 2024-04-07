//
//  MessageStreamWrapperSpec.m
//  iOSUnityWrapperTests
//
//  Created by Ian Stewart on 22/02/24.
//

#import <XCTest/XCTest.h>
#import <OCMock/OCMock.h>
#import <Marigold/Marigold.h>
#import "MessageStreamWrapper.h"
#import "MessageStreamWrapper+Private.h"
#import "UnityMessage.h"

const char *messageJson = "{\"id\":\"12345\",\"title\":\"testmsg\"}";
const char *messagesJson = "[{\"id\":\"12345\",\"title\":\"testmsg\"},{\"id\":\"other\",\"title\":\"msg2\"}]";
const char *invalidMessageJson = "{]";

@interface MessageStreamWrapper ()
- (MARMessage *)messageFromJSON:(NSString *)JSONString;
- (NSArray *)messagesFromJSON:(NSString *)JSONArrayString;
@end

@interface MARMessage ()
- (nullable instancetype)initWithDictionary:(nonnull NSDictionary *)dictionary;
@end

@interface MessageStreamWrapperSpec : XCTestCase

@property (strong, nonatomic) MARMessageStream *mockMessageStream;
@property (strong, nonatomic) NSError *error;

@end

@implementation MessageStreamWrapperSpec

- (void)setUp {
    self.mockMessageStream = OCMClassMock([MARMessageStream class]);
    self.error = [NSError errorWithDomain:@"test error message" code:1 userInfo:nil];
    [MessageStreamWrapper shared].messageStream = self.mockMessageStream;
}

- (void)tearDown {
    [[UnitySender shared].messages removeAllObjects];
}

- (void)testUnreadCount {
    _unreadCount();
    OCMVerify([self.mockMessageStream unreadCount:[MessageStreamWrapper shared].unreadCountBlock]);
}

- (void)testMessages {
    _messages();
    OCMVerify([self.mockMessageStream messages:[MessageStreamWrapper shared].messagesBlock]);
}

- (void)testShowMessageDetail {
    _showMessageDetail(messageJson);
    OCMVerify([self.mockMessageStream presentMessageDetailForMessage:[OCMArg checkWithSelector:@selector(checkMessage:) onObject:self]]);
}

- (void)testShowMessageDetailInvalidMessage {
    _showMessageDetail(invalidMessageJson);
    OCMVerify(never(), [self.mockMessageStream presentMessageDetailForMessage:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testDismissMessageDetail {
    _dismissMessageDetail();
    OCMVerify([self.mockMessageStream dismissMessageDetail]);
}

- (void)testRegisterImpressionInApp {
    _registerImpression(messageJson, 0);
    OCMVerify([self.mockMessageStream registerImpressionWithType:MARImpressionTypeInAppNotificationView forMessage:[OCMArg checkWithSelector:@selector(checkMessage:) onObject:self]]);
}

- (void)testRegisterImpressionStream {
    _registerImpression(messageJson, 1);
    OCMVerify([self.mockMessageStream registerImpressionWithType:MARImpressionTypeStreamView forMessage:[OCMArg checkWithSelector:@selector(checkMessage:) onObject:self]]);
}

- (void)testRegisterImpressionDetail {
    _registerImpression(messageJson, 2);
    OCMVerify([self.mockMessageStream registerImpressionWithType:MARImpressionTypeDetailView forMessage:[OCMArg checkWithSelector:@selector(checkMessage:) onObject:self]]);
}

- (void)testRegisterImpressionInvalid {
    _registerImpression(messageJson, 3);
    OCMVerify(never(), [self.mockMessageStream registerImpressionWithType:MARImpressionTypeInAppNotificationView forMessage:[OCMArg any]]);
    OCMVerify(never(), [self.mockMessageStream registerImpressionWithType:MARImpressionTypeStreamView forMessage:[OCMArg any]]);
    OCMVerify(never(), [self.mockMessageStream registerImpressionWithType:MARImpressionTypeDetailView forMessage:[OCMArg any]]);
    [[UnitySender shared] checkMessageContainsWithObject:@"MessageStream" method:@"ReceiveError" message:@"Unable to determine Impression Type"];
}

- (void)testRegisterMessageImpressionInvalidMessage {
    _registerImpression(invalidMessageJson, 0);
    OCMVerify(never(), [self.mockMessageStream registerImpressionWithType:MARImpressionTypeInAppNotificationView forMessage:[OCMArg any]]);
    OCMVerify(never(), [self.mockMessageStream registerImpressionWithType:MARImpressionTypeStreamView forMessage:[OCMArg any]]);
    OCMVerify(never(), [self.mockMessageStream registerImpressionWithType:MARImpressionTypeDetailView forMessage:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testRemoveMessage {
    _removeMessage(messageJson);
    OCMVerify([self.mockMessageStream removeMessage:[OCMArg checkWithSelector:@selector(checkMessage:) onObject:self] withResponse:[MessageStreamWrapper shared].errorBlock]);
}

- (void)testRemoveMessageInvalidMessage {
    _removeMessage(invalidMessageJson);
    OCMVerify(never(), [self.mockMessageStream removeMessage:[OCMArg any] withResponse:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testClearMessages {
    _clearMessages();
    OCMVerify([self.mockMessageStream clearMessagesWithResponse:[MessageStreamWrapper shared].errorBlock]);
}

- (void)testMarkMessageAsRead {
    _markMessageAsRead(messageJson);
    OCMVerify([self.mockMessageStream markMessageAsRead:[OCMArg checkWithSelector:@selector(checkMessage:) onObject:self] withResponse:[MessageStreamWrapper shared].errorBlock]);
}

- (void)testMarkMessageAsReadInvalidMessage {
    _markMessageAsRead(invalidMessageJson);
    OCMVerify(never(), [self.mockMessageStream markMessageAsRead:[OCMArg any] withResponse:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testMarkMessagesAsRead {
    _markMessagesAsRead(messagesJson);
    OCMVerify([self.mockMessageStream markMessagesAsRead:[OCMArg checkWithSelector:@selector(checkMessages:) onObject:self] withResponse:[MessageStreamWrapper shared].errorBlock]);
}

- (void)testMarkMessagesAsReadInvalidMessage {
    _markMessagesAsRead(invalidMessageJson);
    OCMVerify(never(), [self.mockMessageStream markMessagesAsRead:[OCMArg any] withResponse:[OCMArg any]]);
    [self checkJsonError];
}

- (void)testErrorBlockWithNil {
    [MessageStreamWrapper shared].errorBlock(nil);
    XCTAssertEqual(0, [[UnitySender shared].messages count]);
}

- (void)testErrorBlockWithError {
    [MessageStreamWrapper shared].errorBlock(self.error);
    [[UnitySender shared] checkMessageContainsWithObject:@"MessageStream" method:@"ReceiveError" message:@"test error message"];
}

- (void)testUnreadCountBlock {
    [MessageStreamWrapper shared].unreadCountBlock(5, nil);
    [[UnitySender shared] checkMessageEqualsWithObject:@"MessageStream" method:@"ReceiveUnreadCount" message:@"5"];
}

- (void)testUnreadCountBlockWithError {
    [MessageStreamWrapper shared].unreadCountBlock(0, self.error);
    [[UnitySender shared] checkMessageContainsWithObject:@"MessageStream" method:@"ReceiveError" message:@"test error message"];
}

- (void)testMessagesBlockWithNil {
    [MessageStreamWrapper shared].messagesBlock(nil, nil);
    [[UnitySender shared] checkMessageEqualsWithObject:@"MessageStream" method:@"ReceiveMessagesJSONData" message:@"[]"];
}

- (void)testMessagesBlockWithMessages {
    NSArray *messages = [self createMessagesWithJson:[NSString stringWithUTF8String:messagesJson]];
    [MessageStreamWrapper shared].messagesBlock(messages, nil);
    XCTAssertEqual(1, [[UnitySender shared].messages count]);
    UnityMessage *message = [[UnitySender shared].messages firstObject];
    XCTAssertTrue([message.object isEqualToString:@"MessageStream"]);
    XCTAssertTrue([message.method isEqualToString:@"ReceiveMessagesJSONData"]);
    XCTAssertTrue([message.message containsString:@"\"id\":\"12345\""]);
    XCTAssertTrue([message.message containsString:@"\"title\":\"testmsg\""]);
    XCTAssertTrue([message.message containsString:@"\"id\":\"other\""]);
    XCTAssertTrue([message.message containsString:@"\"title\":\"msg2\""]);
}

- (void)testMessagesBlockWithError {
    [MessageStreamWrapper shared].unreadCountBlock(0, self.error);
    [[UnitySender shared] checkMessageContainsWithObject:@"MessageStream" method:@"ReceiveError" message:@"test error message"];
}

// TODO - test JSON converters

# pragma mark Helpers

- (void)checkJsonError {
    [[UnitySender shared] checkMessageEqualsWithObject:@"MessageStream" method:@"ReceiveError" message:@"The data couldn’t be read because it isn’t in the correct format."];
}

- (BOOL)checkMessage:(MARMessage *)message {
    if (![message.messageID isEqualToString:@"12345"]) {
        return NO;
    }
    if (![message.title isEqualToString:@"testmsg"]) {
        return NO;
    }
    return YES;
}

- (BOOL)checkMessages:(NSArray *)messages {
    if ([messages count] != 2) {
        return NO;
    }
    
    MARMessage *message1 = messages[0];
    if (![message1.messageID isEqualToString:@"12345"]) {
        return NO;
    }
    if (![message1.title isEqualToString:@"testmsg"]) {
        return NO;
    }
    
    MARMessage *message2 = messages[1];
    if (![message2.messageID isEqualToString:@"other"]) {
        return NO;
    }
    if (![message2.title isEqualToString:@"msg2"]) {
        return NO;
    }
    return YES;
}

- (NSArray *)createMessagesWithJson:(NSString *)messgesJson {
    NSMutableArray *messages = [NSMutableArray new];
    NSArray *json = [NSJSONSerialization JSONObjectWithData:[messgesJson dataUsingEncoding:NSUTF8StringEncoding] options:0 error:nil];
    for (NSDictionary *messageDict in json) {
        [messages addObject:[[MARMessage alloc] initWithDictionary:messageDict]];
    }
    return messages;
}

@end
