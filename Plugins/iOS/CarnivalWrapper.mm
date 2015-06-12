#import "CarnivalWrapper.h"
#import <Foundation/Foundation.h>

@interface CarnivalWrapper ()
/*
 * We need to hold these blocks to make sure they are not released 
 * by ARC before they're executed and the scope variable are destroyed. 
 * Seems to be unique to the Unity runtime and Objective-C++.
 */
@property (nonatomic, copy) void (^tagReturnBlock)(NSArray *tags, NSError *error);
@property (nonatomic, copy) void (^tagSetBlock)(NSArray *tags, NSError *error);
@property (nonatomic, copy) void (^stringAttributeSetBlock)(NSError *error);
@property (nonatomic, copy) void (^booleanAttributeSetBlock)(NSError *error);
@property (nonatomic, copy) void (^floatAttributeSetBlock)(NSError *error);
@property (nonatomic, copy) void (^integerAttributeSetBlock)(NSError *error);
@property (nonatomic, copy) void (^dateAttributeSetBlock)(NSError *error);
@property (nonatomic, copy) void (^unsetAttributeBlock)(NSError *error);

@property (nonatomic, strong) UINavigationController *navVC;
@end
@implementation CarnivalWrapper

# pragma mark - C Methods

# pragma mark Init

/*
 * Use of carnivalInstance is to effectively call self inside C++ methods, which you can't do.
 */
void initCarnival () {
    if (!carnivalInstance) {
        [[CarnivalWrapper alloc] init];
    }
}

# pragma mark Engine

void _startEngine(char *apiKey) {
    printf("We got here\n:");
    [Carnival startEngine:[NSString stringWithUTF8String:apiKey]];
}

# pragma mark Tags

void _setTags(char *tagString) {
    initCarnival();
    [carnivalInstance setTags:[[NSString stringWithUTF8String:tagString] componentsSeparatedByString:@","]];
    
}

void _getTags() {
    initCarnival();
    [carnivalInstance getTags];
}

# pragma mark Message Stream

void _showMessageStream() {
    initCarnival();
    [carnivalInstance showMesssageStream];
}

# pragma mark Location

void _updateLocation(double lat, double lon) {
    CLLocation *loc = [[CLLocation alloc] initWithLatitude:lat longitude:lon];
    [Carnival updateLocation:loc];
}

# pragma mark Custom Events

void _logEvent(const char *event) {
    [Carnival logEvent:[NSString stringWithUTF8String:event]];
}

# pragma mark Custom Attributes

void _setString(const char *string, const char *key) {
    initCarnival();
    [carnivalInstance setString:[NSString stringWithUTF8String:string] forKey:[NSString stringWithUTF8String:key]];
}

void _setBool(bool boolValue, const char *key) {
    initCarnival();
    [carnivalInstance setBoolean:boolValue forKey:[NSString stringWithUTF8String:key]];
}

void _setDate(int64_t secondsSince1970, const char *key) {
    initCarnival();
    [carnivalInstance setDate:[NSDate dateWithTimeIntervalSince1970:secondsSince1970] forKey:[NSString stringWithUTF8String:key]];
}

void _setFloat(float floatValue, const char *key) {
    initCarnival();
    [carnivalInstance setFloat:floatValue forKey:[NSString stringWithUTF8String:key]];
}

void _setInteger(int intValue, const char *key) {
    initCarnival();
    [carnivalInstance setInteger:intValue forKey:[NSString stringWithUTF8String:key]];
}

void _removeAttribute(const char *key) {
    initCarnival();
    [carnivalInstance unsetValueForKey:[NSString stringWithUTF8String:key]];
}


# pragma mark - Obj-C Methods

# pragma mark Init

- (id) init {
    self = [super init];
    if (self) {
        carnivalInstance = self;
    }
    return self;
}

# pragma mark Tags
- (void)getTags {
    self.tagReturnBlock = ^(NSArray *tags, NSError *error) {
        if (tags) {
            UnitySendMessage("Carnival", "ReceiveTags", [[tags componentsJoinedByString:@","] UTF8String]);
        }
        if (error) {
            UnitySendMessage("Carnival", "ReceiveError", [[error localizedDescription] UTF8String]);
        }
    };
    
    [Carnival getTagsInBackgroundWithResponse:self.tagReturnBlock];
}

- (void)setTags:(NSArray *)tags {
    self.tagSetBlock = ^(NSArray *tags, NSError *error) {
        if (error) {
            UnitySendMessage("Carnival", "ReceiveError", [[error localizedDescription] UTF8String]);
        }
    };
    
    [Carnival setTagsInBackground:tags withResponse:self.tagSetBlock];
}

# pragma mark Stream

- (void)showMesssageStream {
    CarnivalStreamViewController *streamVC = [[CarnivalStreamViewController alloc] init];
    self.navVC = [[UINavigationController alloc] initWithRootViewController:streamVC];
    
    UIBarButtonItem *closeItem = [[UIBarButtonItem alloc] initWithImage:[UIImage imageNamed:@"CarnivalResources.bundle/cp_close_button.png"]  style:UIBarButtonItemStylePlain target:self action:@selector(closeButtonPressed:)];
    
    [closeItem setTintColor:[UIColor blackColor]];
    
    [streamVC.navigationItem setRightBarButtonItem:closeItem];
    
    [UnityGetGLViewController() presentViewController:self.navVC animated:YES completion:nil];
}

- (void)closeButtonPressed:(UIButton *)button {
    [self.navVC dismissViewControllerAnimated:YES completion:NULL];
}

# pragma mark Custom Attributes

- (void)setString:(NSString *)value forKey:(NSString *)key {
    self.stringAttributeSetBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage("Carnival", "ReceiveError", [[error localizedDescription] UTF8String]);
        }
    };
    [Carnival setString:value forKey:key withResponse:self.stringAttributeSetBlock];
}

- (void)setBoolean:(BOOL)value forKey:(NSString *)key {
    self.booleanAttributeSetBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage("Carnival", "ReceiveError", [[error localizedDescription] UTF8String]);
        }
    };
    [Carnival setBool:value forKey:key withResponse:self.booleanAttributeSetBlock];
}

- (void)setDate:(NSDate *)value forKey:(NSString *)key {
    self.dateAttributeSetBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage("Carnival", "ReceiveError", [[error localizedDescription] UTF8String]);
        }
    };
    [Carnival setDate:value forKey:key withResponse:self.dateAttributeSetBlock];
}

- (void)setInteger:(NSInteger)value forKey:(NSString *)key {
    self.integerAttributeSetBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage("Carnival", "ReceiveError", [[error localizedDescription] UTF8String]);
        }
    };
    [Carnival setInteger:value forKey:key withResponse:self.integerAttributeSetBlock];
}

- (void)setFloat:(CGFloat)value forKey:(NSString *)key {
    self.floatAttributeSetBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage("Carnival", "ReceiveError", [[error localizedDescription] UTF8String]);
        }
    };
    [Carnival setFloat:value forKey:key withResponse:self.floatAttributeSetBlock];
}

- (void)unsetValueForKey:(NSString *)key {
    self.unsetAttributeBlock = ^(NSError *error) {
        if (error) {
            UnitySendMessage("Carnival", "ReceiveError", [[error localizedDescription] UTF8String]);
        }
    };
    [Carnival removeAttributeWithKey:key withResponse:self.unsetAttributeBlock];
}



@end
