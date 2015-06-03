#import "CarnivalWrapper.h"
#import <Foundation/Foundation.h>
@implementation CarnivalWrapper

void start_engine(char *apiKey) {
    printf("We got here\n:");
    [Carnival startEngine:[NSString stringWithUTF8String:apiKey]];
}

void set_tags(char *tagString){
    NSString *nsTagString = [NSString stringWithUTF8String:tagString];
    [Carnival setTagsInBackground:[nsTagString componentsSeparatedByString:@","]];
}

void get_tags(const char *GameObjectName,const char *TagCallback,const char *ErrorCallback){
    [Carnival getTagsInBackgroundWithResponse:^(NSArray *tags, NSError *error) {
        if (tags) {
             UnitySendMessage(GameObjectName, TagCallback, [[tags componentsJoinedByString:@","] UTF8String]);
        }
        if (error) {
            UnitySendMessage(GameObjectName, ErrorCallback, [[error localizedDescription] UTF8String]);
        }
    }];
}

void show_message_stream() {
    
    CarnivalStreamViewController *streamVC = [[CarnivalStreamViewController alloc] init];
    UINavigationController *navVC = [[UINavigationController alloc] initWithRootViewController:streamVC];
    
    //UIBarButtonItem *closeItem = [[UIBarButtonItem alloc] initWithTitle:@"Close" style:UIBarButtonItemStylePlain target:self action:@selector(closeButtonPressed:)];
    //[streamVC.navigationItem setRightBarButtonItem:closeItem];

    [UnityGetGLViewController() presentViewController:navVC animated:YES completion:nil];
}

//- (void)closeButtonPressed:(UIButton *)button {
//    [self dismissViewControllerAnimated:YES completion:NULL];
//}

void update_location(double lat, double lon) {
    CLLocation *loc = [[CLLocation alloc] initWithLatitude:lat longitude:lon];
    [Carnival updateLocation:loc];
}

@end
