using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class FBUploadImage : MonoBehaviour {

	public InputField messageField;
	public Image uploadImage;

	Action<Dictionary<string, object>> callback;
	Texture2D image;
	GameObject eventSystem;
	TouchScreenKeyboard keyboard;

	void Awake()
	{
		List<GameObject> eventSystems = new List<GameObject>();

		foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
			if(obj.GetComponent<EventSystem>())
				eventSystems.Add(obj);
	
		if(eventSystems.Count == 0)
		{
			eventSystem = new GameObject();
			eventSystem.name = "EventSystem";
			eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<TouchInputModule>();
			eventSystem.AddComponent<StandaloneInputModule>();
		}

		callback = FBWrapper.Instance.uploadCallback;
		image = FBWrapper.Instance.uploadImage;

		if(image != null)
		{
			uploadImage.sprite = Sprite.Create(image, 
			                                   new Rect(0, 0, image.width, image.height), 
			                                   new Vector2(.5f, .5f));
		}

		keyboard = TouchScreenKeyboard.Open ("");
	}

	public void OnOkPress()
	{
		FBWrapper.Instance.UploadImage(callback, image, messageField.text);
		Close ();
	}

	public void OnCancelPress()
	{
		Close ();
	}

	void Close()
	{
		if(eventSystem != null) Destroy(eventSystem);
		Destroy(gameObject);
	}

	void Update()
	{
		if(!keyboard.done)
		{
			messageField.text = keyboard.text;
		}
	}
}