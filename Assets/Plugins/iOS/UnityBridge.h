#import "TestClass.h"
#ifdef __cplusplus
extern "C" {
#endif
    
    void framework_hello();
    void framework_message(const char* message);
    void framework_trigger_delegate();
    
    typedef void (*DelegateCallbackFunction)(int number);
    void framework_setDelegate(DelegateCallbackFunction callback);
    void framework_sendMessage(int message);
    
#ifdef __cplusplus
}
#endif