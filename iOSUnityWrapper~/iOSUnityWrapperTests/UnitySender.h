//
//  UnitySender.h
//  iOSUnityWrapper
//
//  Created by Ian Stewart on 21/02/24.
//

#include <stdint.h>
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface UnitySender : NSObject {
}

extern "C" void UnitySendMessage(const char* obj, const char* method, const char* msg);

@property (nonatomic, strong) NSMutableArray *messages;

+ (UnitySender *)shared;

- (void)checkMessageEqualsWithObject:(NSString *)objectString method:(NSString *)methodString message:(NSString *)messageString;
- (void)checkMessageContainsWithObject:(NSString *)objectString method:(NSString *)methodString message:(NSString *)messageString;

@end

NS_ASSUME_NONNULL_END
