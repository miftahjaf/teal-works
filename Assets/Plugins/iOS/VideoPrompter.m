//
//  VideoPrompter.m
//  Unity-iPhone
//
//  Created by Sagar Patel on 18/07/16.
//
//

#import "VideoPrompter.h"
#import "CameraEngine.h"

@interface RecordVideoViewController()
@property(nonatomic) int counter;
@property(nonatomic,weak) AVCaptureVideoPreviewLayer* preview;
@property(nonatomic,strong) UILabel *counterLabel;
@end

@implementation RecordVideoViewController

#define DEGREES_TO_RADIANS(x) (M_PI * (x) / 180.0)

-(void) recordAndPlay
{
    [[CameraEngine engine] startup];
    
    _preview = [[CameraEngine engine] getPreviewLayer];
    [_preview removeFromSuperlayer];
    _preview.frame = self.view.bounds;
    [self.view setTransform:CGAffineTransformMakeRotation(DEGREES_TO_RADIANS(180))];
    [[_preview connection] setVideoOrientation:AVCaptureVideoOrientationLandscapeLeft];
    
    [self.view.layer addSublayer:_preview];
}

-(void) startRecording
{
    NSLog(@"Stop clicked");
    float width = 186;
    float height = 145;
    
    UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    [self.view setFrame:CGRectMake(rootVC.view.frame.size.width - width - 24, 25, width, height)];

    
    
    //[self.view setTransform:CGAffineTransformMakeRotation(DEGREES_TO_RADIANS(180))];
    

    _preview.frame = self.view.bounds;
    
    [[CameraEngine engine] startCapture];

}

-(void) stopButtonClicked
{
    NSLog(@"Stop clicked");
    [[CameraEngine engine] stopCapture : YES];
    [self backButtonClicked];
}

-(void) pauseButtonClicked
{
    NSLog(@"Pause clicked");
    [[CameraEngine engine] pauseCapture];
}

-(void) resumeButtonClicked
{
    NSLog(@"Resume clicked");
    [[CameraEngine engine] resumeCapture];
}

-(void) startButtonClicked :(UIButton*) sender
{
    [NSTimer scheduledTimerWithTimeInterval:1.0
                                     target:self
                                   selector:@selector(onTick:)
                                   userInfo:nil
                                    repeats:NO];
    
    UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    float width = 100;
    float height = 50;
    _counterLabel = [[UILabel alloc] initWithFrame:CGRectMake((rootVC.view.frame.size.width - width) / 2, (rootVC.view.frame.size.height - height) / 2, width, height)];
    
    [_counterLabel setTextColor:[UIColor blackColor]];
    [_counterLabel setBackgroundColor:[UIColor clearColor]];
    NSString *counterString = [NSString stringWithFormat:@"%d", _counter];
    _counterLabel.text = counterString;
    [_counterLabel setFont:[UIFont fontWithName: @"Trebuchet MS" size: 50.0f]];
    [self.view addSubview:_counterLabel];
}

-(void) backButtonClicked
{
    [[CameraEngine engine] stopCapture : NO];
    [self willMoveToParentViewController:nil];
    [self.view removeFromSuperview];
    [self removeFromParentViewController];
}

-(void) deleteLocalVideo : (const char*) path
{
    NSString* localPath = [NSString stringWithUTF8String:path];
    [[NSFileManager defaultManager] removeItemAtPath:localPath error:nil];
}

@end

static RecordVideoViewController *r = NULL;

void _BackButton(const char* message)
{
    NSLog(@"Native: Back button clicked");
    [r backButtonClicked];
}

void _StartButton(const char* message)
{
    NSLog(@"Native: Back button clicked");
    [r startRecording];
}

void _StopButton(const char* message)
{
    NSLog(@"Native: Back button clicked");
    [r stopButtonClicked];
}

void _PauseButton(const char* message)
{
    NSLog(@"Native: Back button clicked");
    [r pauseButtonClicked];
}

void _ResumeButton(const char* message)
{
    NSLog(@"Native: Back button clicked");
    [r resumeButtonClicked];
}

void _DeleteLocalVideo(const char* path)
{
    NSLog(@"Native: Deleting local video");
    [r deleteLocalVideo:path];
}

void _StartPreview(const char* message)
{
    NSLog(@"Message from framework ");
    NSLog(@"%s",message);
    
    UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    
    float width = 800;
    float height = 600;
    
    NSLog(@"%@ ", r);
    if(r == NULL) {
        r = [[RecordVideoViewController alloc]init];
    }
    [r.view setFrame:CGRectMake(78, 84, width, height)];
    r.view.backgroundColor  = [UIColor clearColor];
    
    [rootVC addChildViewController:r];
    [rootVC.view addSubview:r.view];
    [r didMoveToParentViewController:rootVC];
    [r recordAndPlay];
}