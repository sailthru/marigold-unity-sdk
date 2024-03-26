//
//  UnityMessage.h
//  iOSUnityWrapperTests
//
//  Created by Ian Stewart on 26/02/24.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface UnityMessage : NSObject

@property (nonatomic, strong) NSString *object;
@property (nonatomic, strong) NSString *method;
@property (nonatomic, strong) NSString *message;

- (instancetype)init NS_UNAVAILABLE;

- (instancetype)initWithObject:(NSString *)object method:(NSString *)method message:(NSString *)message;

@end

NS_ASSUME_NONNULL_END
