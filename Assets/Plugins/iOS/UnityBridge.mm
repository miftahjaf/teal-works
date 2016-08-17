//
//  UnityBridge.m
//  TestUnityPlugin
//
//  Created by Shalin Shodhan on 09/06/16.
//  Copyright © 2016 Shalin Shodhan. All rights reserved.
//

#import "UnityBridge.h"

DelegateCallbackFunction delegate = NULL;
@interface UnityBridge : NSObject<TestDelegate>
@end
static UnityBridge *__delegate = nil;
void framework_hello() {
    [TestClass displayFrameworkHello];
}
void framework_message(const char* message) {
    [TestClass displayFrameworkString:[NSString stringWithUTF8String:message]];
}
void framework_trigger_delegate() {
    [TestClass sendNumberToDelegate];
}
void framework_setDelegate(DelegateCallbackFunction callback) {
    if (!__delegate) {
        __delegate = [[UnityBridge alloc] init];
    }
    [TestClass setDelegate:__delegate];
    
    delegate = callback;
}
@implementation UnityBridge
-(void)newNumberAvailable:(int)number {
    if (delegate != NULL) {
        delegate(number);
    }
}
@end