#import "EngageBySailthruWrapper.h"
#import <Marigold/Marigold.h>
#import <Foundation/Foundation.h>

const char *MAR_ST_MARIGOLD = "Marigold";
const char *MAR_ST_ENGAGE_BY_ST = "EngageBySailthru";
const char *MAR_ST_RECEIVE_ERROR = "ReceiveError";
const char *MAR_ST_RECEIVE_PROFILE_VARS = "ReceiveProfileVars";
const char *MAR_ST_RECEIVE_UNWRAPPED_LINK = "ReceiveUnwrappedLink";

@interface MARPurchase ()

- (nullable instancetype)initWithDictionary:(nonnull NSDictionary *)dictionary;

@end

@interface EngageBySailthruWrapper ()
/*
 * We need to hold these blocks to make sure they are not released 
 * by ARC before they're executed and the scope variable are destroyed. 
 * Seems to be unique to the Unity runtime and Objective-C++.
 */
@property (nonatomic, copy) void (^errorBlock)(NSError *error);
@property (nonatomic, copy) void (^profileVarsBlock)(NSDictionary<NSString *, id> * _Nullable, NSError * _Nullable);
@end

@implementation EngageBySailthruWrapper

# pragma mark - C Methods

/*
 * Use of engageBySailthruInstance is to effectively call self inside C++ methods, which you can't do.
 */
void initEngageBySailthru () {
    if (!engageBySailthruInstance) {
        engageBySailthruInstance = [[EngageBySailthruWrapper alloc] init];
    }
}

# pragma mark Page Tracking

void _trackPageview (const char *url, const char *tags) {
    initEngageBySailthru();
    [engageBySailthruInstance trackPageview:[NSString stringWithUTF8String:url] withTags:nullableString(tags)];

}

void _trackImpression (const char *sectionId, const char *urls) {
    initEngageBySailthru();
    [engageBySailthruInstance trackImpression:[NSString stringWithUTF8String:sectionId] withUrls:nullableString(urls)];
}

void _trackClick (const char *sectionId, const char *url) {
    initEngageBySailthru();
    [engageBySailthruInstance trackClick:[NSString stringWithUTF8String:sectionId] withUrl:[NSString stringWithUTF8String:url]];
}

# pragma mark User Details

void _setUserId(const char *userID) {
    initEngageBySailthru();
    [engageBySailthruInstance setUserId:nullableString(userID)];
}

void _setUserEmail(const char *userEmail) {
    initEngageBySailthru();
    [engageBySailthruInstance setUserEmail:nullableString(userEmail)];
}

# pragma mark Cusom Events

void _logEvent(const char *event, const char *varsString) {
    initEngageBySailthru();
    [engageBySailthruInstance logEvent:[NSString stringWithUTF8String:event] withVars:nullableString(varsString)];
}

# pragma mark Profile Vars

void _setProfileVars (const char *varsString) {
    initEngageBySailthru();
    [engageBySailthruInstance setProfileVars:[NSString stringWithUTF8String:varsString]];
}

void _getProfileVars () {
    initEngageBySailthru();
    [engageBySailthruInstance getProfileVars];
}

# pragma mark Sailthru Links

void _handleSailthruLink (const char *linkString) {
    initEngageBySailthru();
    [engageBySailthruInstance handleSailthruLink:[NSString stringWithUTF8String:linkString]];
}

# pragma mark Purchases

void _logPurchase(const char *purchaseString) {
    initEngageBySailthru();
    [engageBySailthruInstance logPurchase:[NSString stringWithUTF8String:purchaseString]];
}

void _logAbandonedCart (const char *purchaseString) {
    initEngageBySailthru();
    [engageBySailthruInstance logAbandonedCart:[NSString stringWithUTF8String:purchaseString]];
}

NSString * nullableString(const char *nullableString) {
    if (!nullableString) {
        return nil;
    }
    return [NSString stringWithUTF8String:nullableString];
}


# pragma mark - Obj-C Methods

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
            UnitySendMessage(MAR_ST_MARIGOLD, MAR_ST_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        }
    };
    
    self.profileVarsBlock = ^(NSDictionary<NSString *,id> *vars, NSError *error) {
        if (error) {
            UnitySendMessage(MAR_ST_MARIGOLD, MAR_ST_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
            return;
        }
        
        NSError *varsError;
        NSData *varsData = [NSJSONSerialization dataWithJSONObject:(id)vars options:NSJSONWritingPrettyPrinted error:&varsError];
        if(varsError) {
            UnitySendMessage(MAR_ST_MARIGOLD, MAR_ST_RECEIVE_ERROR, [[varsError localizedDescription] UTF8String]);
            return;
        }
        if (!varsData) {
            return;
        }
        NSString *varsString = [NSString stringWithUTF8String:(const char *)[varsData bytes]];
        UnitySendMessage(MAR_ST_ENGAGE_BY_ST, MAR_ST_RECEIVE_PROFILE_VARS, [varsString UTF8String]);
    };
}

- (EngageBySailthru *)engageBySailthru {
    NSError *error;
    EngageBySailthru *engageBySailthru = [[EngageBySailthru alloc] initWithError:&error];
    if (error) {
        UnitySendMessage(MAR_ST_MARIGOLD, MAR_ST_RECEIVE_ERROR, [[error localizedDescription] UTF8String]);
        return nil;
    }
    return engageBySailthru;
}

# pragma mark Page Tracking

- (void)trackPageview:(NSString *)urlString withTags:(NSString *)tagsString {
    NSURL *url = [NSURL URLWithString:urlString];
    NSArray *tags = nil;
    if (!url) {
        UnitySendMessage(MAR_ST_MARIGOLD, MAR_ST_RECEIVE_ERROR, "Invalid URL provided");
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
    NSArray *urls = nil;
    if (urlsString) {
        urls = [self convertStringToJson:urlsString];
        if (!urls) {
            return;
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
        UnitySendMessage(MAR_ST_MARIGOLD, MAR_ST_RECEIVE_ERROR, "Invalid URL provided");
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
        UnitySendMessage(MAR_ST_MARIGOLD, MAR_ST_RECEIVE_ERROR, "Provided link is not in a valid format");
    }
}

# pragma mark Purchases

- (void)logPurchase:(NSString *)purchaseString {
    MARPurchase *purchase = [self convertStringToPurchase:purchaseString];
    [[self engageBySailthru] logPurchase:purchase withResponse:self.errorBlock];
}

- (void)logAbandonedCart:(NSString *)purchaseString {
    MARPurchase *purchase = [self convertStringToPurchase:purchaseString];
    [[self engageBySailthru] logAbandonedCart:purchase withResponse:self.errorBlock];
}

# pragma mark Helper Methods

- (id)convertStringToJson:(NSString *)jsonString {
    NSError *jsonError;
    NSDictionary *json = [NSJSONSerialization JSONObjectWithData:[jsonString dataUsingEncoding:NSUTF8StringEncoding] options:kNilOptions error:&jsonError];
    if (jsonError) {
        UnitySendMessage(MAR_ST_MARIGOLD, MAR_ST_RECEIVE_ERROR, [[jsonError localizedDescription] UTF8String]);
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

@end
