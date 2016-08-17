//
//  TextView.mm
//  TestUnityPlugin
//
//  Created by Shalin Shodhan on 09/06/16.
//  Copyright © 2016 Sagar Patel. All rights reserved.
//

#import "FeedbackTextView.h"

@implementation CerebroFeedbackTextViewController

UITextView* feedbacktextView;

-(void) AddTextView
{
    if(feedbacktextView == NULL) {
        feedbacktextView = [[UITextView alloc] initWithFrame:self.view.bounds];
    } else {
        [feedbacktextView setFrame:self.view.bounds];
    }
    if ([feedbacktextView respondsToSelector:@selector(inputAssistantItem)])
    {
        UITextInputAssistantItem *inputAssistantItem = [feedbacktextView inputAssistantItem];
        inputAssistantItem.leadingBarButtonGroups = @[];
        inputAssistantItem.trailingBarButtonGroups = @[];
    }
    feedbacktextView.autocorrectionType = UITextAutocorrectionTypeNo;
    
    feedbacktextView.contentInset = UIEdgeInsetsMake(4,4,0,0);
    feedbacktextView.font = [UIFont fontWithName:@"GothamSSm-Book" size:19];
    
    feedbacktextView.text = @"Enter your feedback here";
    feedbacktextView.textColor = [UIColor lightGrayColor];
    
    feedbacktextView.delegate = self;
    [self.view addSubview:feedbacktextView];
    [feedbacktextView becomeFirstResponder];
}
-(void) textViewDidBeginEditing:(UITextView *)textView {
    if([textView.text isEqual: @"Enter your feedback here"]) {
        textView.textColor = [UIColor blackColor];
        textView.text = @"";
    }
    [self.view setFrame:CGRectMake(self.view.frame.origin.x, self.view.frame.origin.y, self.view.frame.size.width, 260)];
    [feedbacktextView setFrame:self.view.bounds];
    
    UnitySendMessage("Feedback", "KeyboardShow", "");
}

-(void) textViewDidEndEditing:(UITextView *)textView {
    if([textView.text isEqual: @""]) {
        textView.textColor = [UIColor lightGrayColor];
        textView.text = @"Enter your feedback here";
    }
    [self.view setFrame:CGRectMake(self.view.frame.origin.x, self.view.frame.origin.y, self.view.frame.size.width, 590)];
    [textView setFrame:self.view.bounds];
    
    UnitySendMessage("Feedback", "KeyboardHide", "");
}

- (void)textViewDidChange:(UITextView *)textView
{
    NSUInteger textLength = textView.text.length;
    if(textLength > 250) {
        textView.text = [textView.text substringToIndex:250];
    }
    const char *stringAsChar = [textView.text cStringUsingEncoding:[NSString defaultCStringEncoding]];
    
    UnitySendMessage("Feedback", "GetTextFieldString", stringAsChar);
}

@end

static CerebroFeedbackTextViewController *vc = NULL;

void _AddFeedbackTextView(float x, float y, float width, float height) {
    UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    
    if(vc == NULL) {
        vc = [[CerebroFeedbackTextViewController alloc]init];
    }
    [vc.view setFrame:CGRectMake(x,y, width, height)];
    vc.view.backgroundColor = [UIColor clearColor];
    
    [rootVC addChildViewController:vc];
    [rootVC.view addSubview:vc.view];
    [vc didMoveToParentViewController:rootVC];
    
    [vc AddTextView];
}

void _RemoveFeedbackTextView() {
    if(vc != NULL) {
        [vc removeFromParentViewController];
        [vc.view removeFromSuperview];
    }
}
