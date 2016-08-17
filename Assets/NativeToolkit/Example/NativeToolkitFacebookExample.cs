using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class NativeToolkitFacebookExample : MonoBehaviour {

	public Texture2D testImage;
	public Image profilePic;
	public Text userName, Id;

	string imagePath;
	Sprite defaultImage;

	void Awake()
	{
		NativeToolkit.FacebookInit(FacebookInited);
		defaultImage = profilePic.sprite;
	}


	//=============================================================================
	// Button handlers
	//=============================================================================

	public void OnFacebookLoginPress()
	{
		NativeToolkit.FacebookLogin(FacebookLoggedIn);
	}

	public void OnFacebookPostToWallPress()
	{
		NativeToolkit.FacebookPostToWall("My post", "Check this out guys!", "I can post to Facebook from my Unity App!", "",
		                                 "http://unity3d.com", FacebookPostedToWall);
	}

	public void OnFacebookUploadImagePress()
	{
		NativeToolkit.FacebookUploadImage(testImage, FacebookImageUploaded);
	}

	public void OnFacebookShareWithFriendsPress()
	{
		NativeToolkit.FacebookShareWithFriends("My App", "Hey people, come check out my app!", true, null, FacebookSharedWithFriends);
	}

	public void OnFacebookGetProfilePicPress()
	{
		NativeToolkit.FacebookGetProfilePic(FacebookProfilePicRetrieved);
	}

	public void OnFacebookGetUserDetailsPress()
	{
		NativeToolkit.FacebookGetUserDetails(FacebookUserDetailsRetrieved);
	}

	public void OnFacebookLogoutPress()
	{
		NativeToolkit.FacebookLogout();
		userName.text = "User Name";
		Id.text = "Id";
		profilePic.sprite = defaultImage;
	}


	//=============================================================================
	// Callbacks - handle returned data for each function
	//=============================================================================

	void FacebookInited()
	{
		Debug.Log ("facebook inited");
	}

	void FacebookLoggedIn(Dictionary<string, object> result)
	{
		Debug.Log ("logged in user " + result["user_id"].ToString());
		Id.text = "Id: " + result["user_id"].ToString();
	}

	void FacebookPostedToWall(Dictionary<string, object> result)
	{
		Debug.Log ("post to wall complete");

		foreach(KeyValuePair<string, object> entry in result)
			Debug.Log (entry.Key + " : " + entry.Value);
	}

	void FacebookImageUploaded(Dictionary<string, object> result)
	{
		Debug.Log ("image uploaded");

		foreach(KeyValuePair<string, object> entry in result)
			Debug.Log (entry.Key + " : " + entry.Value);
	}

	void FacebookSharedWithFriends(Dictionary<string, object> result)
	{
		foreach(KeyValuePair<string, object> entry in result)
			Debug.Log (entry.Key + " : " + entry.Value);
	}

	void FacebookProfilePicRetrieved(Texture2D image)
	{
		Debug.Log ("got profile pic");
		profilePic.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(.5f, .5f));
	}

	void FacebookUserDetailsRetrieved(Dictionary<string, object> result)
	{
		Debug.Log ("user details retrieved : ");

		foreach(KeyValuePair<string, object> entry in result)
			Debug.Log (entry.Key + " : " + entry.Value);

		userName.text = result["first_name"].ToString();
	}
}