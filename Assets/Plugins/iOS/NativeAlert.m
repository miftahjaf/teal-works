//
//  NativeAlert.m
//  Unity-iPhone
//
//  Created by Teal on 03/01/17.
//
//

#import "NativeAlert.h"

static CerebroAlertViewController *vc = NULL;

@interface CerebroAlertViewController()

@end

@implementation CerebroAlertViewController

@end

void _ShowNativeAlert(const char* title, const char* message)
{
    if(vc == NULL) {
        vc = [[CerebroAlertViewController alloc]init];
    }
    
    NSString* titleNS = [NSString stringWithUTF8String:title];
    NSString* messageNS = [NSString stringWithUTF8String:message];
    
    UIAlertController * alert=   [UIAlertController
                                  alertControllerWithTitle:titleNS
                                  message:messageNS
                                  preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction* ok = [UIAlertAction
                         actionWithTitle:@"OK"
                         style:UIAlertActionStyleDefault
                         handler:^(UIAlertAction * action)
                         {
                             [alert dismissViewControllerAnimated:YES completion:nil];
                             
                         }];
    [alert addAction:ok];
    
    [vc presentViewController:alert animated:YES completion:nil];
}
