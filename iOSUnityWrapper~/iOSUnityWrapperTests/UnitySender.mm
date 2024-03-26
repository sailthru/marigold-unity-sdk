//
//  UnitySender.m
//  iOSUnityWrapper
//
//  Created by Ian Stewart on 21/02/24.
//

#import "UnitySender.h"
#import "UnityMessage.h"
#import <XCTest/XCTest.h>

void UnitySendMessage(const char* obj, const char* method, const char* msg) {
    UnityMessage *message = [[UnityMessage alloc]
                             initWithObject:[NSString stringWithUTF8String:obj]
                             method:[NSString stringWithUTF8String:method]
                             message:[NSString stringWithUTF8String:msg]];
    [[UnitySender shared].messages addObject:message];
}

static dispatch_once_t onceToken;
static UnitySender *_sharedInstance;

@implementation UnitySender

+ (UnitySender *)shared {
    dispatch_once(&onceToken, ^ {
        _sharedInstance = [UnitySender new];
    });
    
    return _sharedInstance;
}

- (instancetype)init {
    self = [super init];
    if (self) {
        self.messages = [NSMutableArray new];
    }
    return self;
}

- (void)checkMessageEqualsWithObject:(NSString *)objectString method:(NSString *)methodString message:(NSString *)messageString {
    XCTAssertEqual(1, [self.messages count]);
    UnityMessage *message = [self.messages firstObject];
    XCTAssertTrue([message.object isEqualToString:objectString]);
    XCTAssertTrue([message.method isEqualToString:methodString]);
    XCTAssertTrue([message.message isEqualToString:messageString]);
}

- (void)checkMessageContainsWithObject:(NSString *)objectString method:(NSString *)methodString message:(NSString *)messageString {
    XCTAssertEqual(1, [self.messages count]);
    UnityMessage *message = [self.messages firstObject];
    XCTAssertTrue([message.object isEqualToString:objectString]);
    XCTAssertTrue([message.method isEqualToString:methodString]);
    XCTAssertTrue([message.message containsString:messageString]);
}

@end
