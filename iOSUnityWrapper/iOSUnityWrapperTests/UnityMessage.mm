//
//  UnityMessage.m
//  iOSUnityWrapperTests
//
//  Created by Ian Stewart on 26/02/24.
//

#import "UnityMessage.h"

@implementation UnityMessage

- (instancetype)initWithObject:(NSString *)object method:(NSString *)method message:(NSString *)message {
    self = [super init];
    if (self) {
        self.object = object;
        self.method = method;
        self.message = message;
    }
    return self;
}

@end
