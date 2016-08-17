using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
public class FrameworkBridge: MonoBehaviour {
	#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void framework_hello();

	[DllImport("__Internal")]
	private static extern void framework_message(string message);

	[DllImport("__Internal")] 
	private static extern void framework_trigger_delegate();

	[DllImport("__Internal")]
	private static extern void framework_setDelegate(DelegateMessage callback);

	private delegate void DelegateMessage(int number);

	[MonoPInvokeCallback(typeof(DelegateMessage))] 
	private static void delegateMessageReceived(int number) {
		Debug.Log("Message received: " + number);
	}
	#endif

	public static void hello() {
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			framework_hello();
		}
		#endif
	}

	public static void message(string message) {
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			framework_message(message);
		}
		#endif
	}

	public static void askDelegateForNumber() {
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			framework_trigger_delegate();
		}
		#endif
	}

	public static void initializeDelegate() {
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			framework_setDelegate(delegateMessageReceived);
		}
		#endif
	}
}