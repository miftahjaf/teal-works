using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Cerebro
{
	public class ProfileScript : MonoBehaviour 
	{

		[SerializeField]
		private GameObject ProfilePic;
		[SerializeField]
		private GameObject NameText;
		[SerializeField]
		private GameObject CoinsText;
		[SerializeField]
		private GameObject GradeText;
		[SerializeField]
		private GameObject QuestionsAttempted;
		[SerializeField]
		private GameObject QuestionsCorrect;
		[SerializeField]
		private GameObject VideosWatched;
		[SerializeField]
		private Text VersionNumber;

		string profileImageFile = "";
		bool imageChanged = false;

		void Start()
		{
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Profile);

			VersionNumber.text = VersionHelper.GetVersionNumber ();
			profileImageFile = Application.persistentDataPath + "/ProfileImage.jpg";
			fetchAndSetImage (profileImageFile);

			NameText.GetComponent<Text> ().text = PlayerPrefs.GetString (PlayerPrefKeys.nameKey);
			GradeText.GetComponent<Text> ().text = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey) + " " + PlayerPrefs.GetString (PlayerPrefKeys.SectionKey);
		}

		public void Initialise () {
			NativeToolkit.OnImagePicked += ImagePicked;
			NativeToolkit.OnCameraShotComplete += CameraShotComplete;

			List<string> watchedVideos = WelcomeScript.instance.GetWatchedVideosJSON ();
			VideosWatched.GetComponent<Text> ().text = watchedVideos.Count.ToString ();

			int totalAttempts = 0;
			int totalCorrect = 0;
			foreach (var item in PracticeData.mPracticeData) {
				totalAttempts += item.Value.totalAttempts;
				totalCorrect += item.Value.totalCorrect;
			}
			QuestionsAttempted.GetComponent<Text> ().text = totalAttempts.ToString ();
			QuestionsCorrect.GetComponent<Text> ().text = totalCorrect.ToString ();

			float totalCoins = LaunchList.instance.mCurrentStudent.Coins;
			CoinsText.GetComponent<Text> ().text = totalCoins.ToString ();
		}

		void fetchAndSetImage(string path)
		{
			Texture2D tex = FetchImageFromFile (path);

			if (tex != null) {
				SetImage (tex);
			} else {
				string url = PlayerPrefs.GetString (PlayerPrefKeys.ProfilePicKey, "");
				CerebroHelper.DebugLog (url);
				if (url != "") {
					StartCoroutine (getProfilePicFromServer (url));
				}
			}
		}

		void SetImage(Texture2D tex)
		{
			float holderWidth = 256f;
			float holderHeight = 256f;

			float holderAspectRatio = holderWidth / holderHeight;
			float imageAspectRatio = (float)tex.width / (float)tex.height;

			float scaleRatio = 1;
			if (holderAspectRatio >= imageAspectRatio) {			// scale to match width
				float actualImageWidth = tex.width * (holderHeight / tex.height);
				scaleRatio = holderWidth / actualImageWidth;
			} else {
				float actualImageHeight = tex.height * (holderWidth / tex.width);
				scaleRatio = holderHeight / actualImageHeight;
			}

			if (scaleRatio < 1.0f) {
				scaleRatio = 1.0f;
			}

			Sprite oldSprite = ProfilePic.GetComponent<Image> ().sprite;
			if (oldSprite.name != "profile-pic") {
				Destroy (oldSprite.texture);
				Destroy (oldSprite);
			}

			CerebroHelper.DebugLog ("tex size = " + tex.width + "," + tex.height);
			CerebroHelper.DebugLog ("Holder Size = " + holderWidth + "," + holderHeight);
			CerebroHelper.DebugLog ("SCALE RATIO = " + scaleRatio);

			var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
			ProfilePic.GetComponent<Image> ().sprite = newsprite;
			ProfilePic.GetComponent<RectTransform> ().localScale = new Vector3 (scaleRatio, scaleRatio, 1f);
			StoreImageToFile (tex, profileImageFile);
		}

		IEnumerator getProfilePicFromServer(string url)
		{
			CerebroHelper.DebugLog ("Getting Image");
			WWW remoteImage = new WWW (url);
			yield return remoteImage;
			CerebroHelper.DebugLog ("Got Image");
			if (remoteImage.error == null) {
				imageChanged = true;
				SetImage (remoteImage.texture as Texture2D);
			}
		}

		public void OnBackButtonPressed()
		{
			NativeToolkit.OnImagePicked -= ImagePicked;
			NativeToolkit.OnCameraShotComplete -= CameraShotComplete;
			WelcomeScript.instance.ShowScreen (false, null, imageChanged);
			this.transform.SetAsFirstSibling ();
			this.gameObject.SetActive (false);
		}

		public void OnPickImagePress ()
		{
			NativeToolkit.PickImage ();
		}

		public void OnCameraPress ()
		{
			NativeToolkit.TakeCameraShot ();
		}

		void ImagePicked (Texture2D img, string path)
		{
			ResizeImage (img);

			imageChanged = true;
			SetImage (img);
			string FileName = PlayerPrefs.GetString (PlayerPrefKeys.IDKey) + System.DateTime.Now.ToString("yyyyMMddHHmmss");
			HTTPRequestHelper.instance.uploadProfilePic (FileName, img);
		}

		void CameraShotComplete (Texture2D img, string path)
		{
			ResizeImage (img);

			imageChanged = true;
			SetImage (img);
			string FileName = PlayerPrefs.GetString (PlayerPrefKeys.IDKey) + System.DateTime.Now.ToString("yyyyMMddHHmmss");
			HTTPRequestHelper.instance.uploadProfilePic (FileName, img);
		}

		void ResizeImage(Texture2D img) {
			if (img.width > 512) {
				int height = Mathf.FloorToInt ((img.height * 512f) / img.width);
				TextureScale.Bilinear (img, 512, height);
			}
		}

		private void StoreImageToFile (Texture2D tex, string fileName)
		{
			var bytes = tex.EncodeToJPG ();
			File.WriteAllBytes (fileName, bytes);
		}

		private Texture2D FetchImageFromFile (string fileName)
		{
			byte[] fileData;
			Texture2D tex = null;
			if (File.Exists (fileName)) {
				fileData = File.ReadAllBytes (fileName);
				tex = new Texture2D (1, 1);
				tex.LoadImage (fileData);
			}
			return tex;
		}

		public void DestroyTexture() {
			Sprite oldSprite = ProfilePic.GetComponent<Image> ().sprite;
			if (oldSprite.name != "profile-pic") {
				Destroy (oldSprite.texture);
				Destroy (oldSprite);
			}
		}
	}
}
