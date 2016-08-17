#import <Foundation/Foundation.h>
@protocol TestDelegate <NSObject>
- (void)newNumberAvailable:(int)number;
@end
@interface TestClass : NSObject
+ (void)displayFrameworkHello;
+ (void)displayFrameworkString:(NSString *)string;
+ (void)sendNumberToDelegate;
+(void)setDelegate:(id<TestDelegate>)delegate;
@end