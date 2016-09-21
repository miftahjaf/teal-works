using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Cerebro
{

	public class CustomizeAvatar : MonoBehaviour 
	{
		public GOTSplashScreen CurrSplashScreen;
		public GameObject BoyButton, GirlButton, ProgressCircle;

		[HideInInspector]
		public int CurrBodyId, CurrHairID, CurrHeadId;
		[HideInInspector]
		public bool IsBoy, IsTransitionOn;
		[HideInInspector]
		public string CurrGroupID;

		private GameObject GenericPopup;

		void Start()
		{
			string BabaId = PlayerPrefs.GetString (PlayerPrefKeys.BabaID, "111");
			InitializeAvatar (BabaId);
		}

		void InitializeAvatar(string BabaId)
		{
			CurrHairID = int.Parse (BabaId [0].ToString());
			CurrHeadId = int.Parse (BabaId [1].ToString());
			CurrBodyId = int.Parse (BabaId [2].ToString());
			//			LaunchList.instance.mCurrentGame.GroupID = PlayerPrefs.GetString (PlayerPrefKeys.GOTGameTeamID, "1");
			if (CurrBodyId > 4) {
				IsBoy = false;
				CurrBodyId -= 4;
				CurrHairID -= 4;
				CurrHeadId -= 4;
			} else {
				IsBoy = true;
			}

			DisableAllComponents ();
			if (IsBoy) {
				transform.FindChild ("boy_body").gameObject.SetActive (true);
				transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId);
				transform.FindChild ("boy_head").gameObject.SetActive (true);
				transform.FindChild ("boy_head").GetComponent<AvatarComponentList>().Initialize(CurrHeadId);
				transform.FindChild ("boy_hair").gameObject.SetActive (true);
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().Initialize(CurrHairID);
			} else {
				transform.FindChild ("girl_body").gameObject.SetActive (true);
				transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId);
				transform.FindChild ("girl_head").gameObject.SetActive (true);
				transform.FindChild ("girl_head").GetComponent<AvatarComponentList>().Initialize(CurrHeadId);
				transform.FindChild ("girl_hair_back").gameObject.SetActive (true);
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().Initialize(CurrHairID);
				transform.FindChild ("girl_hair_front").gameObject.SetActive (true);
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().Initialize(CurrHairID);
			}
		}

		void DisableAllComponents()
		{
			transform.FindChild ("boy_body").gameObject.SetActive (false);
			transform.FindChild ("boy_head").gameObject.SetActive (false);
			transform.FindChild ("boy_hair").gameObject.SetActive (false);
			transform.FindChild ("girl_body").gameObject.SetActive (false);
			transform.FindChild ("girl_head").gameObject.SetActive (false);
			transform.FindChild ("girl_hair_back").gameObject.SetActive (false);
			transform.FindChild ("girl_hair_front").gameObject.SetActive (false);
		}

		public void BoyButtonPressed()
		{
			string BabaId = "111";
			InitializeAvatar (BabaId);
//			BoyButton.transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1, 1, 1, 1);
//			BoyButton.transform.FindChild ("Text").GetComponent<Image> ().color = new Color (1, 1, 1, 1);
//			GirlButton.transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1, 1, 1, 0.5f);
//			GirlButton.transform.FindChild ("Text").GetComponent<Image> ().color = new Color (1, 1, 1, 0.5f);
		}

		public void GirlButtonPressed()
		{
			string BabaId = "555";
			InitializeAvatar (BabaId);
//			BoyButton.transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1, 1, 1, 0.5f);
//			BoyButton.transform.FindChild ("Text").GetComponent<Image> ().color = new Color (1, 1, 1, 0.5f);
//			GirlButton.transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1, 1, 1, 1);
//			GirlButton.transform.FindChild ("Text").GetComponent<Image> ().color = new Color (1, 1, 1, 1);
		}

		public void SaveButtonPressed()
		{
			string BabaId = "";
			BabaId += CurrHairID;
			BabaId += CurrHeadId;
			BabaId += CurrBodyId;
			ProgressCircle.SetActive (true);
			HTTPRequestHelper.instance.SendAvatarSet (BabaId, OnSaveResponse);
		}

		public void OnSaveResponse(bool IsSuccess)
		{
			ProgressCircle.SetActive (false);
			if (IsSuccess) {
				string BabaId = "";
				BabaId += CurrHairID;
				BabaId += CurrHeadId;
				BabaId += CurrBodyId;
				PlayerPrefs.SetString (PlayerPrefKeys.BabaID, BabaId);
				CurrSplashScreen.CloseButtonClicked ();
			} else {
				Start ();
				GenericPopup = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.GenericPopup, transform.parent);
				GenericPopup.transform.SetAsLastSibling ();
				GenericPopup.GetComponent<GenericPopup> ().Initialise ("No Internet Connection. Try back later.", 1, false, ClosePopup);
			}
		}

		public void ClosePopup()
		{
			Destroy (GenericPopup);
			CurrSplashScreen.CloseButtonClicked ();
		}

		public void BodyNextPressed()
		{
			if (CurrBodyId == 4 || IsTransitionOn) {
				return;
			}

			CurrBodyId++;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().NextButtonPressed();
			} else {
				transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().NextButtonPressed();
			}
		}

		public void BodyPreviousPressed()
		{
			if (CurrBodyId == 1 || IsTransitionOn) {
				return;
			}

			CurrBodyId--;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().PreviousButtonPressed();
			} else {
				transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().PreviousButtonPressed();
			}
		}

		public void HeadNextPressed()
		{
			if (CurrHeadId == 4 || IsTransitionOn) {
				return;
			}

			CurrHeadId++;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_head").GetComponent<AvatarComponentList>().NextButtonPressed();
			} else {
				transform.FindChild ("girl_head").GetComponent<AvatarComponentList>().NextButtonPressed();
			}
		}

		public void HeadPreviousPressed()
		{
			if (CurrHeadId == 1 || IsTransitionOn) {
				return;
			}

			CurrHeadId--;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_head").GetComponent<AvatarComponentList>().PreviousButtonPressed();
			} else {
				transform.FindChild ("girl_head").GetComponent<AvatarComponentList>().PreviousButtonPressed();
			}
		}

		public void HairNextPressed()
		{
			if (CurrHairID == 4 || IsTransitionOn) {
				return;
			}

			CurrHairID++;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().NextButtonPressed();
			} else {
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().NextButtonPressed();
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().NextButtonPressed();
			}
		}

		public void HairPreviousPressed()
		{
			if (CurrHairID == 1 || IsTransitionOn) {
				return;
			}

			CurrHairID--;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().PreviousButtonPressed();
			} else {
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().PreviousButtonPressed();
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().PreviousButtonPressed();
			}
		}

		public void ChooseUnit(string groupID, int hairID = 1, int headID = 1, int bodyID = 1)
		{
			var hairString = "baba_hair_1";
			var hairString2 = "";
			var bodyString = "baba_body_1";
			var faceString = "baba_head_1";

			if (hairID == 1) {
				hairString = "baba_hair_1";
			} else if (hairID == 2) {
				hairString = "baba_hair_2";
			} else if (hairID == 3) {
				hairString = "baba_hair_3";
			} else if (hairID == 4) {
				hairString = "baba_hair_4";
			} else if (hairID == 5) {
				hairString = "baba_hair_5_front";
				hairString2 = "baba_hair_5_back";
			} else if (hairID == 6) {
				hairString = "baba_hair_6_front";
			} else if (hairID == 7) {
				hairString = "baba_hair_7_front";
				hairString2 = "baba_hair_7_back";
			} else if (hairID == 8) {
				hairString = "baba_hair_8_front";
				hairString2 = "baba_hair_8_back";
			}

			bodyString = "baba_body_" + bodyID.ToString();
			faceString = "baba_head_" + headID.ToString();

			var body = transform.Find (bodyString);
			var hair = transform.Find (hairString);
			Transform hair2 = null;
			SpriteRenderer hair2Rndr = null;
			if (hairString != "") {
				hair2 = transform.Find (hairString2);
			}
			var face = transform.Find (faceString);

			face.gameObject.SetActive (true);
			body.gameObject.SetActive (true);
			hair.gameObject.SetActive (true);
			if(hair2)
			{
				hair2.gameObject.SetActive (true);
				hair2Rndr = hair2.GetComponent<SpriteRenderer> ();
			}
			var bodyRndr = body.GetComponent<SpriteRenderer> ();
			var hairRndr =  hair.GetComponent<SpriteRenderer> ();

			var teamColor = new Color (0, 0, 0);
			groupID = LaunchList.instance.mCurrentGame.GroupID;
			if (groupID == GroupMapping.Group1) {
				teamColor = new Color (0.99f, 0.39f, 0.15f);
			} else if (groupID == GroupMapping.Group2) {
				teamColor = new Color (0.05f, 0.9f, 0.9f);
			} else if (groupID == GroupMapping.Group3) {
				teamColor = new Color (0.62f, 0.62f, 0.62f);
			} else if (groupID == GroupMapping.Group4) {
				teamColor = new Color (0.39f, 0.62f, 0.92f);
			}
			hair2Rndr.color = teamColor;
			bodyRndr.color = teamColor;
			hairRndr.color = teamColor;
		}
	}
}