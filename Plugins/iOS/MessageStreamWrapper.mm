#import "MessageStreamWrapper.h"
#import <Marigold/Marigold.h>

const char *MAR_STREAM_MESSAGE_STREAM = "MessageStream";
const char *MAR_STREAM_RECEIVE_ERROR = "ReceiveError";
const char *MAR_STREAM_RECEIVE_UNREAD_COUNT = "ReceiveUnreadCount";
const char *MAR_STREAM_RECEIVE_MESSAGES = "ReceiveMessagesJSONData";

static MessageStreamWrapper * _sharedInstance = nil;
static dispatch_once_t onceSharedPredicate = 0;

@interface MARMessage ()

- (nullable instancetype)initWithDictionary:(nonnull NSDictionary *)dictionary;
- (nonnull NSDictionary *)dictionary;

@end

@interface MessageStreamWrapper ()

@property (nonatomic, copy) void (^errorBlock)(NSError *error);
@property (nonatomic, copy) void (^unreadCountBlock)(NSUInteger unreadCount, NSError *error);
@property (nonatomic, copy) void (^messagesBlock)(NSArray *messages, NSError *error);
@property (nonatomic, strong) MARMessageStream *messageStream;

+ (MessageStreamWrapper *)shared;

@end

@implementation MessageStreamWrapper

# pragma mark - C Methods

# pragma mark - Unread Count

void _unreadCount() {
    [[MessageStreamWrapper shared] unreadCount];
}

# pragma mark Messages

void _messages () {
    [[MessageStreamWrapper shared] messages];
}

# pragma mark Message Detail

void _showMessageDetail(const char *messageJSON) {
    [[MessageStreamWrapper shared] showMessageDetailForMessage:[NSString stringWithUTF8String:messageJSON]];
}

void _dismissMessageDetail() {
    [[MessageStreamWrapper shared] dismissMessageDetail];
}

# pragma mark Register Impressions

void _registerImpression(const char *messageJSON, int impressionType) {
    [[MessageStreamWrapper shared] registerImpressionForMessage:[NSString stringWithUTF8String:messageJSON] impressionType:impressionType];
}

# pragma mark Remove

void _removeMessage(const char *messageJSON) {
    [[MessageStreamWrapper shared] removeMessage:[NSString stringWithUTF8String:messageJSON]];
}

void _clearMessages() {
    [[MessageStreamWrapper shared] clearMessages];
}

# pragma mark Mark As Read

void _markMessageAsRead(const char *messageJSON) {
    [[MessageStreamWrapper shared] markMessageAsRead:[NSString stringWithUTF8String:messageJSON]];
}

void _markMessagesAsRead(const char *messagesJSON) {
    [[MessageStreamWrapper shared] markMessagesAsRead:[NSString stringWithUTF8String:messagesJSON]];
}

# pragma mark - Obj-C Methods

# pragma mark Init

+ (MessageStreamWrapper *)shared {
    dispatch_once(&onceSharedPredicate, ^{
        if (!_sharedInstance) {
            _sharedInstance = [[self alloc] init];
        }
    });
    
    return _sharedInstance;
}

- (id) init {
    self = [super init];
    if (self) {
        self.messageStream = [MARMessageStream new];
        [self setupCallbacks];
    }
    return self;
}

- (void)setupCallbacks {
    self.errorBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage(MAR_STREAM_MESSAGE_STREAM, MAR_STREAM_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        }
    };
    
    self.unreadCountBlock = ^(NSUInteger unreadCount, NSError *error) {
        if (error) {
            UnitySendMessage(MAR_STREAM_MESSAGE_STREAM, MAR_STREAM_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
            return;
        }
        UnitySendMessage(MAR_STREAM_MESSAGE_STREAM, MAR_STREAM_RECEIVE_UNREAD_COUNT, [[NSString stringWithFormat:@"%lu", (unsigned long)unreadCount] UTF8String]);
    };
    
    __weak __typeof__(self) weakSelf = self;
    self.messagesBlock = ^(NSArray *messages, NSError *error) {
        if (error) {
            UnitySendMessage(MAR_STREAM_MESSAGE_STREAM, MAR_STREAM_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
            return;
        }
        
        NSArray *messagesArray = messages;
        if (!messagesArray) {
            messagesArray = @[];
        }
        __weak __typeof__(self) strongSelf = weakSelf;
        if (strongSelf) {
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:[strongSelf arrayOfMessageDictionariesFromMessageArray:messagesArray] options:0 error:nil];
            NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            UnitySendMessage(MAR_STREAM_MESSAGE_STREAM, MAR_STREAM_RECEIVE_MESSAGES, [jsonString UTF8String]);
        }
    };
}

# pragma mark - Unread Count

- (void)unreadCount {
    [self.messageStream unreadCount:self.unreadCountBlock];
}

# pragma mark Messages

- (void)messages {
    [self.messageStream messages:[MessageStreamWrapper shared].messagesBlock];
}

# pragma mark Message Detail

- (void)showMessageDetailForMessage:(NSString *)messageJSON {
    MARMessage *message = [self messageFromJSON:messageJSON];
    if (message) {
        [self.messageStream presentMessageDetailForMessage:message];
    }
}

- (void)dismissMessageDetail {
    [[MessageStreamWrapper shared].messageStream dismissMessageDetail];
}

# pragma mark Register Impressions

- (void)registerImpressionForMessage:(NSString *)messageJSON impressionType:(int)impressionType {
    MARMessage *message = [self messageFromJSON:messageJSON];
    if (!message) {
        return;
    }
    
    MARImpressionType type;
    switch (impressionType) {
        case 0:
            type = MARImpressionTypeInAppNotificationView;
            break;
        case 1:
            type = MARImpressionTypeStreamView;
            break;
        case 2:
            type = MARImpressionTypeDetailView;
            break;
        default:
            UnitySendMessage(MAR_STREAM_MESSAGE_STREAM, MAR_STREAM_RECEIVE_ERROR, "Unable to determine Impression Type");
            return;
    }
    [self.messageStream registerImpressionWithType:type forMessage:message];
}

# pragma mark Remove

- (void)removeMessage:(NSString *)messageJSON {
    MARMessage *message = [self messageFromJSON:messageJSON];
    if (message) {
        [self.messageStream removeMessage:message withResponse:self.errorBlock];
    }
}

- (void)clearMessages {
    [self.messageStream clearMessagesWithResponse:self.errorBlock];
}

# pragma mark Mark As Read

- (void)markMessageAsRead:(NSString *)messageJSON {
    MARMessage *message = [[MessageStreamWrapper shared] messageFromJSON:messageJSON];
    if (message) {
        [[MessageStreamWrapper shared].messageStream markMessageAsRead:message withResponse:[MessageStreamWrapper shared].errorBlock];
    }
}

- (void)markMessagesAsRead:(NSString *)messagesJSON {
    NSArray *messages = [[MessageStreamWrapper shared] messagesFromJSON:messagesJSON];
    if (messages) {
        [[MessageStreamWrapper shared].messageStream markMessagesAsRead:messages withResponse:[MessageStreamWrapper shared].errorBlock];
    }
}

# pragma mark - Helper Methods

- (NSArray *)arrayOfMessageDictionariesFromMessageArray:(NSArray *)messageArray {
    NSMutableArray *messageDictionaries = [NSMutableArray array];
    for (MARMessage *message in messageArray) {
        [messageDictionaries addObject:[message dictionary]];
    }
    return messageDictionaries;
}

- (MARMessage *)messageFromJSON:(NSString *)JSONString {
    NSData *data = [JSONString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *deserializeError = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers error:&deserializeError];
    
    if (deserializeError) {
        UnitySendMessage(MAR_STREAM_MESSAGE_STREAM, MAR_STREAM_RECEIVE_ERROR, [[deserializeError localizedDescription] UTF8String]);
        return nil;
    }
    return [[MARMessage alloc] initWithDictionary:dict];
}

- (NSArray *)messagesFromJSON:(NSString *)JSONArrayString {
    NSData *data = [JSONArrayString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *deserializeError = nil;
    NSArray *array = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers error:&deserializeError];
    
    if (deserializeError) {
        UnitySendMessage(MAR_STREAM_MESSAGE_STREAM, MAR_STREAM_RECEIVE_ERROR, [[deserializeError localizedDescription] UTF8String]);
        return nil;
    }
    
    NSMutableArray *messageArray = [NSMutableArray new];
    for (NSDictionary *messageDict in array) {
        [messageArray addObject:[[MARMessage alloc] initWithDictionary:messageDict]];
    }
    return messageArray;
}

@end
