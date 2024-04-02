#ifndef EngageBySailthruWrapper_Private_h
#define EngageBySailthruWrapper_Private_h

#import "EngageBySailthruWrapper.h"

@interface EngageBySailthruWrapper ()

@property (nonatomic, copy) void (^errorBlock)(NSError *error);
@property (nonatomic, copy) void (^profileVarsBlock)(NSDictionary<NSString *, id> * _Nullable, NSError * _Nullable);

+ (EngageBySailthruWrapper *)shared;
- (EngageBySailthru *)engageBySailthru;

@end

#endif /* EngageBySailthruWrapper_Private_h */
