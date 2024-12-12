#import "EngageBySailthruWrapper.h"
#import <Marigold/Marigold.h>

const char *MAR_ST_ENGAGE_BY_ST = "EngageBySailthru";
const char *MAR_ST_RECEIVE_ERROR = "ReceiveError";
const char *MAR_ST_RECEIVE_PROFILE_VARS = "ReceiveProfileVars";
const char *MAR_ST_RECEIVE_UNWRAPPED_LINK = "ReceiveUnwrappedLink";

@interface MARPurchase ()

- (nullable instancetype)initWithDictionary:(nonnull NSDictionary *)dictionary;

@end

static EngageBySailthruWrapper * _sharedInstance = nil;
static dispatch_once_t onceSharedPredicate = 0;

@interface EngageBySailthruWrapper ()
/*
 * We need to hold these blocks to make sure they are not released
 * by ARC before they're executed and the scope variable are destroyed.
 * Seems to be unique to the Unity runtime and Objective-C++.
 */
@property (nonatomic, copy) void (^errorBlock)(NSError *error);
@property (nonatomic, copy) void (^profileVarsBlock)(NSDictionary<NSString *, id> * _Nullable, NSError * _Nullable);

+ (EngageBySailthruWrapper *)shared;

@end

@implementation EngageBySailthruWrapper

# pragma mark - C Methods

# pragma mark Page Tracking

void _trackPageview (const char *url, const char *tags) {
    NSString *tagsString = nil;
    if (tags) {
        tagsString = [NSString stringWithUTF8String:tags];
    }
    [[EngageBySailthruWrapper shared] trackPageview:[NSString stringWithUTF8String:url] withTags:tagsString];

}

void _trackImpression (const char *sectionId, const char *urls) {
    NSString *urlsString = nil;
    if (urls) {
        urlsString = [NSString stringWithUTF8String:urls];
    }
    [[EngageBySailthruWrapper shared] trackImpression:[NSString stringWithUTF8String:sectionId] withUrls:urlsString];
}

void _trackClick (const char *sectionId, const char *url) {
    [[EngageBySailthruWrapper shared] trackClick:[NSString stringWithUTF8String:sectionId] withUrl:[NSString stringWithUTF8String:url]];
}

# pragma mark User Details

void _setUserId(const char *userID) {
    NSString *idString = nil;
    if (userID) {
        idString = [NSString stringWithUTF8String:userID];
    }
    [[EngageBySailthruWrapper shared] setUserId:idString];
}

void _setUserEmail(const char *userEmail) {
    NSString *emailString = nil;
    if (userEmail) {
        emailString = [NSString stringWithUTF8String:userEmail];
    }
    [[EngageBySailthruWrapper shared] setUserEmail:emailString];
}

# pragma mark Custom Events

void _logEvent(const char *event, const char *varsString) {
    NSString *vars = nil;
    if (varsString) {
        vars = [NSString stringWithUTF8String:varsString];
    }
    [[EngageBySailthruWrapper shared] logEvent:[NSString stringWithUTF8String:event] withVars:vars];
}

void _clearEvents() {
    [[EngageBySailthruWrapper shared] clearEvents];
}

# pragma mark Profile Vars

void _setProfileVars (const char *varsString) {
    [[EngageBySailthruWrapper shared] setProfileVars:[NSString stringWithUTF8String:varsString]];
}

void _getProfileVars () {
    [[EngageBySailthruWrapper shared] getProfileVars];
}

# pragma mark Sailthru Links

void _handleSailthruLink (const char *linkString) {
    [[EngageBySailthruWrapper shared] handleSailthruLink:[NSString stringWithUTF8String:linkString]];
}

# pragma mark Purchases

void _logPurchase(const char *purchaseString) {
    [[EngageBySailthruWrapper shared] logPurchase:[NSString stringWithUTF8String:purchaseString]];
}

void _logAbandonedCart (const char *purchaseString) {
    [[EngageBySailthruWrapper shared] logAbandonedCart:[NSString stringWithUTF8String:purchaseString]];
}

# pragma mark Device Attributes

void _setAttributes(const char *attributesString) {
    [[EngageBySailthruWrapper shared] setAttributes:[NSString stringWithUTF8String:attributesString]];
}

void _removeAttribute(const char *key) {
    [[EngageBySailthruWrapper shared] removeAttribute:[NSString stringWithUTF8String:key]];
}

void _clearAttributes() {
    [[EngageBySailthruWrapper shared] clearAttributes];
}


# pragma mark - Obj-C Methods

+ (EngageBySailthruWrapper *)shared {
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
        [self setupCallbacks];
    }
    return self;
}

- (void)setupCallbacks {
    self.errorBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        }
    };
    
    self.profileVarsBlock = ^(NSDictionary<NSString *,id> *vars, NSError *error) {
        if (error) {
            UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
            return;
        }
        NSDictionary *profileVars = vars;
        if (!profileVars) {
            profileVars = @{};
        }
        
        NSError *varsError;
        NSData *varsData = [NSJSONSerialization dataWithJSONObject:(id)profileVars options:0 error:&varsError];
        if(varsError) {
            UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_ERROR, [[varsError localizedDescription] UTF8String]);
            return;
        }
        if (!varsData) {
            return;
        }
        NSString *varsString = [[NSString alloc] initWithData:varsData encoding:NSUTF8StringEncoding];
        UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_PROFILE_VARS, [varsString UTF8String]);
    };
}

- (EngageBySailthru *)engageBySailthru {
    NSError *error;
    EngageBySailthru *engageBySailthru = [[EngageBySailthru alloc] initWithError:&error];
    if (error) {
        UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        return nil;
    }
    return engageBySailthru;
}

# pragma mark Page Tracking

- (void)trackPageview:(NSString *)urlString withTags:(NSString *)tagsString {
    NSURL *url = [NSURL URLWithString:urlString];
    NSArray *tags = nil;
    if (!url) {
        UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_ERROR, "Invalid URL provided");
        return;
    }
    if (tagsString) {
        tags = [self convertStringToJson:tagsString];
        if (!tags) {
            return;
        }
    }
    
    if (tags) {
        [[self engageBySailthru] trackPageviewWithUrl:url andTags:tags andResponse:self.errorBlock];
    } else {
        [[self engageBySailthru] trackPageviewWithUrl:url andResponse:self.errorBlock];
    }
    
}

- (void)trackImpression:(NSString *)sectionId withUrls:(NSString *)urlsString {
    NSMutableArray *urls = nil;
    if (urlsString) {
        NSArray *urlStrings = [self convertStringToJson:urlsString];
        if (!urlStrings) {
            return;
        }
        urls = [NSMutableArray new];
        for (NSString *urlString in urlStrings) {
            [urls addObject:[NSURL URLWithString:urlString]];
        }
    }
    if (urls) {
        [[self engageBySailthru] trackImpressionWithSection:sectionId andUrls:urls andResponse:self.errorBlock];
    } else {
        [[self engageBySailthru] trackImpressionWithSection:sectionId andResponse:self.errorBlock];
    }
    
}

- (void) trackClick:(NSString *)sectionId withUrl:(NSString *)urlString {
    NSURL *url = [NSURL URLWithString:urlString];
    if (!url) {
        UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_ERROR, "Invalid URL provided");
        return;
    }
    [[self engageBySailthru] trackClickWithSection:sectionId andUrl:url andResponse:self.errorBlock];
}

# pragma mark User Details

-(void)setUserId:(NSString *)userID {
    [[self engageBySailthru] setUserId:userID withResponse:self.errorBlock];
}

-(void)setUserEmail:(NSString *)userEmail {
    [[self engageBySailthru] setUserEmail:userEmail withResponse:self.errorBlock];
}

# pragma mark Custom Events

- (void)logEvent:(NSString *)event withVars:(NSString *)varsString {
    NSDictionary *vars = nil;
    if (varsString) {
        vars = [self convertStringToJson:varsString];
        if (!vars) {
            return;
        }
    }
    [[self engageBySailthru] logEvent:event withVars:vars];
}

- (void)clearEvents {
    [[self engageBySailthru] clearEventsWithResponse:self.errorBlock];
}

# pragma mark Profile Vars

- (void)setProfileVars:(NSString *)varsString {
    NSDictionary *vars = [self convertStringToJson:varsString];
    if (!vars) {
        return;
    }
    [[self engageBySailthru] setProfileVars:vars withResponse:self.errorBlock];
}

- (void)getProfileVars {
    [[self engageBySailthru] getProfileVarsWithResponse:self.profileVarsBlock];
}

# pragma mark Sailthru Links

- (void)handleSailthruLink:(NSString *)linkString {
    NSURL *link = [NSURL URLWithString:linkString];
    NSURL *unwrapped = [[self engageBySailthru] handleSailthruLink:link];
    if (unwrapped) {
        UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_UNWRAPPED_LINK, [[unwrapped absoluteString] UTF8String]);
    } else {
        UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_ERROR, "Provided link is not in a valid format");
    }
}

# pragma mark Purchases

- (void)logPurchase:(NSString *)purchaseString {
    MARPurchase *purchase = [self convertStringToPurchase:purchaseString];
    if (!purchase) {
        return;
    }
    [[self engageBySailthru] logPurchase:purchase withResponse:self.errorBlock];
}

- (void)logAbandonedCart:(NSString *)purchaseString {
    MARPurchase *purchase = [self convertStringToPurchase:purchaseString];
    if (!purchase) {
        return;
    }
    [[self engageBySailthru] logAbandonedCart:purchase withResponse:self.errorBlock];
}

# pragma mark Device Attributes

- (void)setAttributes:(NSString *)attributesString {
    MARAttributes *attributes = [self convertStringToAttributes:attributesString];
    if (!attributes) {
        return;
    }
    [[self engageBySailthru] setAttributes:attributes withCompletion:self.errorBlock];
}

- (void)removeAttribute:(NSString *)key {
    [[self engageBySailthru] removeAttributeWithKey:key withCompletion:self.errorBlock];
}

- (void)clearAttributes {
    [[self engageBySailthru] clearAttributesWithCompletion:self.errorBlock];
}

# pragma mark Helper Methods

- (id)convertStringToJson:(NSString *)jsonString {
    NSError *jsonError;
    NSDictionary *json = [NSJSONSerialization JSONObjectWithData:[jsonString dataUsingEncoding:NSUTF8StringEncoding] options:kNilOptions error:&jsonError];
    if (jsonError) {
        UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_ERROR, [[jsonError localizedDescription] UTF8String]);
        return nil;
    }
    return json;
}

- (MARPurchase *)convertStringToPurchase:(NSString *)purchaseString {
    NSDictionary *purchaseJson = [self convertStringToJson:purchaseString];
    if (!purchaseJson) {
        return nil;
    }
    return [[MARPurchase alloc] initWithDictionary:purchaseJson];
}

- (MARAttributes *)convertStringToAttributes:(NSString *)attributesString {
    NSDictionary *attributesJson = [self convertStringToJson:attributesString];
    if (!attributesJson) {
        return nil;
    }

    MARAttributes *marAttributes = [MARAttributes new];
    NSInteger mergeRule = [[attributesJson valueForKey:@"mergeRule"] integerValue];
    [marAttributes setAttributesMergeRule:(MARAttributesMergeRule)mergeRule];

    NSDictionary *attributes = [attributesJson valueForKey:@"attributes"];
    [attributes enumerateKeysAndObjectsUsingBlock:^(NSString *  _Nonnull key, NSDictionary *  _Nonnull attribute, BOOL * _Nonnull stop) {
        NSString *type = [attribute valueForKey:@"type"];

        if ([type isEqualToString:@"string"]) {
            NSString *value = [attribute valueForKey:@"value"];
            [marAttributes setString:value forKey:key];

        } else if ([type isEqualToString:@"stringArray"]) {
            NSArray<NSString *> *value = [attribute valueForKey:@"value"];
            [marAttributes setStrings:value forKey:key];

        } else if ([type isEqualToString:@"integer"]) {
            NSNumber *value = [attribute objectForKey:@"value"];
            [marAttributes setInteger:[value integerValue] forKey:key];

        } else if ([type isEqualToString:@"integerArray"]) {
            NSArray<NSNumber *> *value = [attribute valueForKey:@"value"];
            [marAttributes setIntegers:value forKey:key];

        } else if ([type isEqualToString:@"boolean"]) {
            BOOL value = [[attribute valueForKey:@"value"] boolValue];
            [marAttributes setBool:value forKey:key];

        } else if ([type isEqualToString:@"float"]) {
            NSNumber *numberValue = [attribute objectForKey:@"value"];
            [marAttributes setFloat:[numberValue floatValue] forKey:key];

        } else if ([type isEqualToString:@"floatArray"]) {
            NSArray<NSNumber *> *value = [attribute objectForKey:@"value"];
            [marAttributes setFloats:value forKey:key];

        } else if ([type isEqualToString:@"date"]) {
            NSString *millisecondsValue = [attribute objectForKey:@"value"];
            NSNumber *value = @([millisecondsValue doubleValue] / 1000);

            if (![value isKindOfClass:[NSNumber class]]) {
                return;
            }

            NSDate *date = [NSDate dateWithTimeIntervalSince1970:[value doubleValue]];
            if (date) {
                [marAttributes setDate:date forKey:key];
            } else {
                return;
            }

        } else if ([type isEqualToString:@"dateArray"]) {
            NSArray<NSString *> *value = [attribute objectForKey:@"value"];
            NSMutableArray<NSDate *> *dates = [[NSMutableArray alloc] init];
            for (NSString *millisecondsValue in value) {
                NSNumber *secondsValue = @([millisecondsValue doubleValue] / 1000);

                if (![secondsValue isKindOfClass:[NSNumber class]]) {
                    continue;
                }

                NSDate *date = [NSDate dateWithTimeIntervalSince1970:[secondsValue doubleValue]];
                if (date) {
                    [dates addObject:date];
                }
            }

            [marAttributes setDates:dates forKey:key];
        }
    }];
    return marAttributes;
}

@end
