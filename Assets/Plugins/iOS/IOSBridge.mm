//
//  iOSBridge.m
//  TestUnityPlugin
//
//  Created by Shalin Shodhan on 09/06/16.
//  Copyright © 2016 Sagar Patel. All rights reserved.
//
#import "iOSBridge.h"


void _TestMessage(const char* message)
{
    NSLog(@"Message from framework ");
    NSLog(@"%s",message);
}