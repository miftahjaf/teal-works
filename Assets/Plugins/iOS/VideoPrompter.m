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
@property(nonatomic,weak) AVCaptureConnection* connection;
@property(nonatomic,strong) UILabel *counterLabel;
@end

@implementation RecordVideoViewController

#define DEGREES_TO_RADIANS(x) (M_PI * (x) / 180.0)

-(void) recordAndPlay : (BOOL*) IsLandscapeLeft
{
    [[CameraEngine engine] startup : IsLandscapeLeft];
    
    _preview = [[CameraEngine engine] getPreviewLayer];
    [_preview removeFromSuperlayer];
    _preview.frame = self.view.bounds;
    [self.view setTransform:CGAffineTransformMakeRotation(DEGREES_TO_RADIANS(180))];
    if(IsLandscapeLeft)
        [[_preview connection] setVideoOrientation:AVCaptureVideoOrientationLandscapeLeft];
    else
        [[_preview connection] setVideoOrientation:AVCaptureVideoOrientationLandscapeRight];
    
    _counter = 0;
    [self.view.layer addSublayer:_preview];
}

-(void) generateThumbImage : (NSString *)filepath
{
    NSLog(@"into ");
    
    NSURL *url = [NSURL fileURLWithPath:filepath];
    
    AVAsset *asset = [AVAsset assetWithURL:url];
    AVAssetImageGenerator *imageGenerator = [[AVAssetImageGenerator alloc]initWithAsset:asset];
    CMTime time = [asset duration];
    time.value = 0;
    CGImageRef imageRef = [imageGenerator copyCGImageAtTime:time actualTime:NULL error:NULL];
    UIImage *thumbnail = [UIImage imageWithCGImage:imageRef];
    CGImageRelease(imageRef);  // CGImageRef won't be released by ARC
    
    NSArray * paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString * basePath = ([paths count] > 0) ? [paths objectAtIndex:0] : nil;
    
    NSData * binaryImageData = UIImagePNGRepresentation(thumbnail);
    
    [binaryImageData writeToFile:[basePath stringByAppendingPathComponent:@"vid.jpg"] atomically:YES];
    
    const char* localOutputPath = [[basePath stringByAppendingPathComponent:@"vid.jpg"] cStringUsingEncoding:NSASCIIStringEncoding];
    NSLog(@"out %s", localOutputPath);
    UnitySendMessage("VerbalizeLandingPage", "GotThumbnailImage", localOutputPath);
}

-(void) startRecording : (BOOL*) IsLandscapeLeft
{
    NSLog(@"Stop clicked");
    float width = 186;
    float height = 145;
    
    UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    if(IsLandscapeLeft)
        [self.view setFrame:CGRectMake(rootVC.view.frame.size.width - width - 24, 25, width, height)];
    else
        [self.view setFrame:CGRectMake(24, rootVC.view.frame.size.height - height - 25, width, height)];
    
    
    
    //[self.view setTransform:CGAffineTransformMakeRotation(DEGREES_TO_RADIANS(180))];
    
    _counter = 1;
    _preview.frame = self.view.bounds;
    
    [[CameraEngine engine] startCapture];
    
}

-(void) rotationChanged : (BOOL*) IsLandscapeLeft
{
    if(_counter == 1)
    {
        float width = 186;
        float height = 145;
        
        UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
        if(IsLandscapeLeft)
            [self.view setFrame:CGRectMake(rootVC.view.frame.size.width - width - 24, 25, width, height)];
        else
            [self.view setFrame:CGRectMake(24, rootVC.view.frame.size.height - height - 25, width, height)];
        
        _connection = [[CameraEngine engine] getVideoConnection];
        if(IsLandscapeLeft)
        {
            [_connection setVideoOrientation:AVCaptureVideoOrientationLandscapeLeft];
        }
        else
        {
            [_connection setVideoOrientation:AVCaptureVideoOrientationLandscapeRight];
        }
    }
    _preview = [[CameraEngine engine] getPreviewLayer];
    if(IsLandscapeLeft)
        [[_preview connection] setVideoOrientation:AVCaptureVideoOrientationLandscapeLeft];
    else
        [[_preview connection] setVideoOrientation:AVCaptureVideoOrientationLandscapeRight];
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
    BOOL IsLandscapeLeft = true;
    
    if(strcmp(message, "true") != 0)
    {
        IsLandscapeLeft = false;
    }
    
    [r startRecording : IsLandscapeLeft];
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

void _GetTempDirectory(const char* path)
{
    NSLog(@"Native: Getting temp directory");
    NSString* tempPath = NSTemporaryDirectory();
    const char* localPath = [tempPath cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage("LaunchList", "GotTempDirectory", localPath);
}

void _GetThumbnail(const char* path)
{
    NSLog(@"Native: Getting Thumbnail");
}

void _RotationChanged(const char* message)
{
    NSLog(@"Message from framework ");
    NSLog(@"%s",message);
    BOOL IsLandscapeLeft = true;
    
    if(strcmp(message, "true") != 0)
    {
        IsLandscapeLeft = false;
    }
    
    UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    
    float width = 800;
    float height = 600;
    
    if(r == NULL) {
        r = [[RecordVideoViewController alloc]init];
    }
    if(IsLandscapeLeft)
        [r.view setFrame:CGRectMake(78, 84, width, height)];
    else
        [r.view setFrame:CGRectMake(rootVC.view.frame.size.width - width - 78, 84, width, height)];
    
    [r rotationChanged:IsLandscapeLeft];
}

void _StartPreview(const char* message)
{
    NSLog(@"Message from framework ");
    NSLog(@"%s",message);
    BOOL IsLandscapeLeft = true;
    __block BOOL IsPermissionGranted = true;
    
    
    if(strcmp(message, "true") != 0)
    {
        IsLandscapeLeft = false;
    }
    
    UIViewController* rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    
    float width = 800;
    float height = 600;
    
    NSLog(@"%@ ", r);
    if(r == NULL) {
        r = [[RecordVideoViewController alloc]init];
    }
    if(IsLandscapeLeft)
        [r.view setFrame:CGRectMake(78, 84, width, height)];
    else
        [r.view setFrame:CGRectMake(rootVC.view.frame.size.width - width - 78, 84, width, height)];
    r.view.backgroundColor  = [UIColor clearColor];
    
    [rootVC addChildViewController:r];
    [rootVC.view addSubview:r.view];
    [r didMoveToParentViewController:rootVC];
    
    [[AVAudioSession sharedInstance] requestRecordPermission:^(BOOL granted) {
        if (granted) {
            NSLog(@"enabled microphone");
        }
        else {
            NSLog(@"disabled microphone");
            UIAlertController * alert=   [UIAlertController
                                          alertControllerWithTitle:@"Microphone Access Denied"
                                          message:@"You must allow microphone access in Settings > Privacy > Microphone"
                                          preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction* ok = [UIAlertAction
                                 actionWithTitle:@"OK"
                                 style:UIAlertActionStyleDefault
                                 handler:^(UIAlertAction * action)
                                 {
                                     [alert dismissViewControllerAnimated:YES completion:nil];
                                     
                                 }];
            [alert addAction:ok];
            
            [r presentViewController:alert animated:YES completion:nil];
            IsPermissionGranted = false;
            UnitySendMessage("Verbalize", "BackPressedOkButton", "");
            return;
        }
    }];
    
    AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
    
    if(status == AVAuthorizationStatusAuthorized) { // authorized
        NSLog(@"camera authorized");
    } else {
        NSLog(@"disabled camera");
        
        if ([AVCaptureDevice respondsToSelector:@selector(requestAccessForMediaType: completionHandler:)]) {
            [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
                NSLog(@"DENIED");
                if (granted) {
                    NSLog(@"granted");
                } else {
                    // Permission has been denied.
                    NSLog(@"permission denied");
                }
            }];
        }
        
        UIAlertController * alert=   [UIAlertController
                                      alertControllerWithTitle:@"Camera Access Denied"
                                      message:@"You must allow camera access in Settings > Privacy > Camera"
                                      preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction* ok = [UIAlertAction
                             actionWithTitle:@"OK"
                             style:UIAlertActionStyleDefault
                             handler:^(UIAlertAction * action)
                             {
                                 [alert dismissViewControllerAnimated:YES completion:nil];
                                 
                             }];
        [alert addAction:ok];
        
        [r presentViewController:alert animated:YES completion:nil];
        IsPermissionGranted = false;
        UnitySendMessage("Verbalize", "BackPressedOkButton", "");
        return;
    }

    if(IsPermissionGranted)
    {
        [r recordAndPlay : IsLandscapeLeft];
    }
    
}