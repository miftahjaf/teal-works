//
//  TextView.mm
//  TestUnityPlugin
//
//  Created by Shalin Shodhan on 09/06/16.
//  Copyright © 2016 Sagar Patel. All rights reserved.
//

#import "TextView.h"

@implementation CerebroTextViewController

UITextView* textView;

-(void) AddTextView:(NSUInteger)charLimit withText:(const char*)text
{
    NSString* initText = [NSString stringWithUTF8String:text];
    characterLimit = charLimit;
    if(textView == NULL) {
        textView = [[UITextView alloc] initWithFrame:self.view.bounds];
    } else {
        [textView setFrame:self.view.bounds];
    }
    if ([textView respondsToSelector:@selector(inputAssistantItem)])
    {
        UITextInputAssistantItem *inputAssistantItem = [textView inputAssistantItem];
        inputAssistantItem.leadingBarButtonGroups = @[];
        inputAssistantItem.trailingBarButtonGroups = @[];
    }
    textView.autocorrectionType = UITextAutocorrectionTypeNo;
    
    textView.contentInset = UIEdgeInsetsMake(-4,-4,0,0);
    textView.font = [UIFont fontWithName:@"GothamSSm-Book" size:19];
    if([initText isEqual: @""]) {
        textView.text = @"Enter your answer here";
        textView.textColor = [UIColor lightGrayColor];
    } else {
        textView.text = initText;
        textView.textColor = [UIColor blackColor];
    }
    textView.delegate = self;
    [self.view addSubview:textView];
    [textView becomeFirstResponder];
}
-(void) textViewDidBeginEditing:(UITextView *)textView {
    if([textView.text isEqual: @"Enter your answer here"]) {
        textView.textColor = [UIColor blackColor];
        textView.text = @"";
    }
    [self.view setFrame:CGRectMake(self.view.frame.origin.x, self.view.frame.origin.y, self.view.frame.size.width, 260)];
    [textView setFrame:self.view.bounds];
    
    UnitySendMessage("WritersCorner", "KeyboardShow", "");
}

-(void) textViewDidEndEditing:(UITextView *)textView {
    if([textView.text isEqual: @""]) {
        textView.textColor = [UIColor lightGrayColor];
        textView.text = @"Enter your answer here";
    }
    [self.view setFrame:CGRectMake(self.view.frame.origin.x, self.view.frame.origin.y, self.view.frame.size.width, 600)];
    [textView setFrame:self.view.bounds];
    
    UnitySendMessage("WritersCorner", "KeyboardHide", "");
}

- (void)textViewDidChange:(UITextView *)textView
{
    NSUInteger textLength = textView.text.length;
    if(textLength > characterLimit) {
        textView.text = [textView.text substringToIndex:characterLimit];
    }
    const char *stringAsChar = [textView.text cStringUsingEncoding:[NSString defaultCStringEncoding]];
    
    UnitySendMessage("WritersCorner", "GetTextFieldString", stringAsChar);
}

- (BOOL)textView:(UITextView *)textView shouldChangeTextInRange:(NSRange)range replacementText:(NSString *)text
{
    if ([textView isFirstResponder])
    {
        if ([[[textView textInputMode] primaryLanguage] isEqualToString:@"emoji"] || ![[textView textInputMode] primaryLanguage])
        {
            return NO;
        }
    }
    return YES;
}

@end

static CerebroTextViewController *vc = NULL;

void _AddTextView(float x, float y, float width, float height,float charLimit,const char* text) {
    UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    
    if(vc == NULL) {
        vc = [[CerebroTextViewController alloc]init];
    }
    [vc.view setFrame:CGRectMake(x,y, width, height)];
    vc.view.backgroundColor = [UIColor clearColor];
    
    [rootVC addChildViewController:vc];
    [rootVC.view addSubview:vc.view];
    [vc didMoveToParentViewController:rootVC];
    
    [vc AddTextView:charLimit withText:text];
}

void _RemoveTextView() {
    if(vc != NULL) {
        [vc removeFromParentViewController];
        [vc.view removeFromSuperview];
    }
}
