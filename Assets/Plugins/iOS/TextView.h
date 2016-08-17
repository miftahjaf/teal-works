#ifdef __cplusplus
extern "C" {
#endif
    
#import <MobileCoreServices/UTCoreTypes.h>
#import <UIKit/UIKit.h>
    
@interface CerebroTextViewController: UIViewController <UITextViewDelegate>
-(void) AddTextView:(NSUInteger)charLimit withText:(const char*)text;
@end
    
    void _AddTextView(float x, float y, float width, float height,float charLimit, const char* text);
    void _RemoveTextView();
#ifdef __cplusplus
}
#endif