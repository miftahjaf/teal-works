using UnityEngine;
using System.Collections;

public class TestiOS : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("We are here");
		FrameworkBridge.initializeDelegate();
		FrameworkBridge.hello ();
		FrameworkBridge.message ("Hello from Unity!");
		FrameworkBridge.askDelegateForNumber();
	}
}
