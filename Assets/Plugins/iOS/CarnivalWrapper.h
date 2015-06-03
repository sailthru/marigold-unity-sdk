#import <Carnival/Carnival.h>

@interface CarnivalWrapper : NSObject
{
}

extern "C" void start_engine(char *apiKey);
extern "C" void set_tags(char *tagString);
extern "C" void get_tags(const char *GameObjectName,const char *TagCallback,const char *ErrorCallback);
extern "C" void show_message_stream();
extern "C" void update_location(double lat, double lon);
@end