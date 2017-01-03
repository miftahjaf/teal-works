#ifdef __cplusplus
extern "C" {
#endif
    
#import <MobileCoreServices/UTCoreTypes.h>
#import <UIKit/UIKit.h>
    
@interface CerebroAlertViewController: UIViewController <UITextViewDelegate>
{
//    NSUInteger characterLimit;
}
@end
    
void _ShowNativeAlert(const char* title, const char* message);

#ifdef __cplusplus
}
#endif
