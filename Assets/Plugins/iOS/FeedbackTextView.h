#ifdef __cplusplus
extern "C" {
#endif
    
#import <MobileCoreServices/UTCoreTypes.h>
#import <UIKit/UIKit.h>
    
    @interface CerebroFeedbackTextViewController: UIViewController <UITextViewDelegate>
-(void) AddTextView;
@end
    
    void _AddFeedbackTextView(float x, float y, float width, float height);
    void _RemoveFeedbackTextView();
#ifdef __cplusplus
}
#endif