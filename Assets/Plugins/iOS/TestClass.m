#import "TestClass.h"

@implementation TestClass
id __delegate = nil;
+ (void)displayFrameworkHello {
    NSLog(@"Hello from framework");
}
+ (void)displayFrameworkString:(NSString *)string
{
    NSLog(@"Message from framework: %@", string);
}
+ (void)sendNumberToDelegate
{
    if (__delegate && [__delegate respondsToSelector:@selector(newNumberAvailable:)]) {
        [__delegate newNumberAvailable:10];
    }
}
+(void)setDelegate:(id<TestDelegate>)delegate {
    __delegate = delegate;
}
@end