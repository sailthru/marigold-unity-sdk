#ifndef MessageStreamWrapper_Private_h
#define MessageStreamWrapper_Private_h

@interface MessageStreamWrapper()

@property (nonatomic, copy) void (^errorBlock)(NSError *error);
@property (nonatomic, copy) void (^messagesBlock)(NSArray *messages, NSError *error);
@property (nonatomic, copy) void (^unreadCountBlock)(NSUInteger unreadCount, NSError *error);
@property (nonatomic, strong) MARMessageStream *messageStream;

+ (MessageStreamWrapper *)shared;

@end

#endif /* MessageStreamWrapper_Private_h */
