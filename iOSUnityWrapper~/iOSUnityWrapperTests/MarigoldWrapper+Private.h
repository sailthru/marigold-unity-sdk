#ifndef MarigoldWrapper_Private_h
#define MarigoldWrapper_Private_h

@interface MarigoldWrapper()

@property (nonatomic, copy) void (^errorBlock)(NSError *error);
@property (nonatomic, copy) void (^deviceIDBlock)(NSString *deviceID, NSError *error);
@property (nonatomic, strong) Marigold *marigold;

+ (MarigoldWrapper *)shared;

@end

#endif /* MarigoldWrapper_Private_h */
