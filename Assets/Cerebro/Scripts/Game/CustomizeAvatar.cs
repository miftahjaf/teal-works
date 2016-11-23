using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Cerebro
{

	public class CustomizeAvatar : MonoBehaviour 
	{
		public GOTSplashScreen CurrSplashScreen;
		public GameObject BoyButton, GirlButton, CoinsLabel, PurchaseButton, TotalCoinsLabel, ProgressCircle;
		public GameObject[] NextPrevButtons, TabularButtons;

		private int CurrBodyId, CurrHairID, CurrHeadId;
		private int CurrHatID, CurrGogglesID, CurrBadgeID;
		private bool IsBoy;

		[HideInInspector]
		public bool IsTransitionOn;

		private GameObject GenericPopup;
		private Tabs CurrTab;
		private Avatar CurrAvatar;

		private int[] HatsCoins, GogglesCoins, BadgesCoins;

		enum Tabs
		{
			Avatar,
			Hats,
			Goggles,
			Badges
		}

		public void Start()
		{
			HatsCoins = new int[6];
			HatsCoins [0] = 0;
			HatsCoins [1] = 200;
			HatsCoins [2] = 250;
			HatsCoins [3] = 300;
			HatsCoins [4] = 350;
			HatsCoins [5] = 400;

			GogglesCoins = new int[6];
			GogglesCoins [0] = 0;
			GogglesCoins [1] = 500;
			GogglesCoins [2] = 600;
			GogglesCoins [3] = 700;
			GogglesCoins [4] = 800;
			GogglesCoins [5] = 900;

			BadgesCoins = new int[6];
			BadgesCoins [0] = 0;
			BadgesCoins [1] = 50;
			BadgesCoins [2] = 75;
			BadgesCoins [3] = 100;
			BadgesCoins [4] = 125;
			BadgesCoins [5] = 150;

			LaunchList.instance.ReadAvatar ();
			CurrAvatar = new Avatar ();
			CurrAvatar.HairID = LaunchList.instance.mAvatar.HairID;
			CurrAvatar.HeadID = LaunchList.instance.mAvatar.HeadID;
			CurrAvatar.BodyID = LaunchList.instance.mAvatar.BodyID;
			CurrAvatar.HatID = LaunchList.instance.mAvatar.HatID;
			CurrAvatar.GogglesID = LaunchList.instance.mAvatar.GogglesID;
			CurrAvatar.BadgeID = LaunchList.instance.mAvatar.BadgeID;

			CurrAvatar.HatID = CurrAvatar.HatID > 0 ? (CurrAvatar.HatID + 1) : -1;
			CurrAvatar.GogglesID = CurrAvatar.GogglesID > 0 ? (CurrAvatar.GogglesID + 1) : -1;
			CurrAvatar.BadgeID = CurrAvatar.BadgeID > 0 ? (CurrAvatar.BadgeID + 1) : -1;

			CurrAvatar.IsBoy = LaunchList.instance.mAvatar.IsBoy;
			InitializeAvatar ();
		}

		public void InitializeAvatar()
		{
//			CurrHairID = LaunchList.instance.mAvatar.HairID;
//			CurrHeadId = LaunchList.instance.mAvatar.HeadID;
//			CurrBodyId = LaunchList.instance.mAvatar.BodyID;
//			CurrHatID = LaunchList.instance.mAvatar.HatID;
//			CurrGogglesID = LaunchList.instance.mAvatar.GogglesID;
//			CurrBadgeID = LaunchList.instance.mAvatar.BadgeID;
			CurrHairID = CurrAvatar.HairID;
			CurrHeadId = CurrAvatar.HeadID;
			CurrBodyId = CurrAvatar.BodyID;
			CurrHatID = CurrAvatar.HatID;
			CurrGogglesID = CurrAvatar.GogglesID;
			CurrBadgeID = CurrAvatar.BadgeID;
			Debug.Log("init hat "+CurrHatID+" goggle "+CurrGogglesID+" badge "+CurrBadgeID);
			//			LaunchList.instance.mCurrentGame.GroupID = PlayerPrefs.GetString (PlayerPrefKeys.GOTGameTeamID, "1");
			if (CurrBodyId > 4) {
				IsBoy = false;
				CurrBodyId -= 4;
				CurrHairID -= 4;
				CurrHeadId -= 4;
			} else {
				IsBoy = true;
			}
//			CurrHatID = CurrHatID > 0 ? (CurrHatID + 1) : -1;
//			CurrGogglesID = CurrGogglesID > 0 ? (CurrGogglesID + 1) : -1;
//			CurrBadgeID = CurrBadgeID > 0 ? (CurrBadgeID + 1) : -1;

			EnableAvatarSelection ();
			TotalCoinsLabel.GetComponent<Text> ().text = LaunchList.instance.mCurrentStudent.Coins.ToString();
		}

		void EnableBasicAvatar()
		{
			if (IsBoy) {
				transform.FindChild ("boy_body").gameObject.SetActive (true);
				transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId);
				transform.FindChild ("boy_head").gameObject.SetActive (true);
				transform.FindChild ("boy_head").GetComponent<AvatarComponentList>().Initialize(CurrHeadId);
				transform.FindChild ("boy_head").GetComponent<AvatarComponentList> ().IsMovingHidden = false;
				transform.FindChild ("boy_hair").gameObject.SetActive (true);
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().Initialize(CurrHairID);
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList> ().IsMovingHidden = false;
			} else {
				transform.FindChild ("girl_body").gameObject.SetActive (true);
				transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId);
				transform.FindChild ("girl_head").gameObject.SetActive (true);
				transform.FindChild ("girl_head").GetComponent<AvatarComponentList>().Initialize(CurrHeadId);
				transform.FindChild ("girl_hair_back").gameObject.SetActive (true);
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().Initialize(CurrHairID);
				transform.FindChild ("girl_hair_front").gameObject.SetActive (true);
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().Initialize(CurrHairID);
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList> ().IsMovingHidden = false;
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList> ().IsMovingHidden = false;
			}
		}

		void EnableHats()
		{
			if (CurrHatID <= 0)
				CurrHatID = 1;
			if (IsBoy) {
				if (CurrTab == Tabs.Hats) {
					CurrHatID = 1;
					transform.FindChild ("boy_hair").GetComponent<AvatarComponentList> ().IsMovingHidden = true;
				} else {
					transform.FindChild ("boy_hair").gameObject.SetActive (false);
				}	
				transform.FindChild ("boy_hats").gameObject.SetActive (true);
				transform.FindChild ("boy_hats").GetComponent<AvatarComponentList> ().Initialize (CurrHatID);
			} else {
				if (CurrTab == Tabs.Hats) {
					CurrHatID = 1;
					transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList> ().IsMovingHidden = true;
					transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList> ().IsMovingHidden = true;
				} else {
					transform.FindChild ("girl_hair_back").gameObject.SetActive (false);
					transform.FindChild ("girl_hair_front").gameObject.SetActive (false);
				}
				transform.FindChild ("girl_hats").gameObject.SetActive (true);
				transform.FindChild ("girl_hats").GetComponent<AvatarComponentList>().Initialize(CurrHatID);
			}
			CheckForHatPrice ();
		}

		void EnableGoggles()
		{
			if (CurrGogglesID <= 0)
				CurrGogglesID = 1;
			if (IsBoy) {
				if (CurrTab == Tabs.Goggles) {
					CurrGogglesID = 1;
					transform.FindChild ("boy_head").GetComponent<AvatarComponentList> ().IsMovingHidden = true;
					GameObject[] GoggleGms = transform.FindChild ("boy_goggles").GetComponent<AvatarComponentList>().ChildComponents;
					GameObject currHead = transform.FindChild ("boy_head_without_eyes").GetComponent<AvatarComponentList> ().ChildComponents [CurrHeadId - 1];
					for (int i = 0; i < GoggleGms.Length; i++) {
						GoggleGms [i].GetComponent<Image> ().sprite = currHead.GetComponent<Image> ().sprite;
					}
					transform.FindChild ("boy_goggles").gameObject.SetActive (true);
					transform.FindChild ("boy_goggles").GetComponent<AvatarComponentList>().Initialize(CurrGogglesID);
				} else {
					transform.FindChild ("boy_head").gameObject.SetActive (false);
					GameObject[] HeadGms = transform.FindChild ("boy_head_without_eyes").GetComponent<AvatarComponentList>().ChildComponents;
					GameObject currGoggle = transform.FindChild ("boy_goggles").GetComponent<AvatarComponentList> ().ChildComponents [CurrGogglesID - 1];
					for (int i = 0; i < HeadGms.Length; i++) {
						HeadGms [i].transform.FindChild("baba_goggles").GetComponent<Image> ().sprite = currGoggle.transform.FindChild("baba_goggles").GetComponent<Image> ().sprite;
						Rect r = currGoggle.transform.FindChild ("baba_goggles").GetComponent<RectTransform> ().rect;
						HeadGms [i].transform.FindChild ("baba_goggles").GetComponent<RectTransform> ().sizeDelta = new Vector2 (r.width, r.height);
					}
					transform.FindChild ("boy_head_without_eyes").gameObject.SetActive (true);
					transform.FindChild ("boy_head_without_eyes").GetComponent<AvatarComponentList>().Initialize(CurrHeadId);
				}
			} else {
				if (CurrTab == Tabs.Goggles) {
					CurrGogglesID = 1;
					transform.FindChild ("girl_head").GetComponent<AvatarComponentList> ().IsMovingHidden = true;
					GameObject[] GoggleGms = transform.FindChild ("girl_goggles").GetComponent<AvatarComponentList>().ChildComponents;
					GameObject currHead = transform.FindChild ("girl_head_without_eyes").GetComponent<AvatarComponentList> ().ChildComponents [CurrHeadId - 1];
					for (int i = 0; i < GoggleGms.Length; i++) {
						GoggleGms [i].GetComponent<Image> ().sprite = currHead.GetComponent<Image> ().sprite;
					}
					transform.FindChild ("girl_goggles").gameObject.SetActive (true);
					transform.FindChild ("girl_goggles").GetComponent<AvatarComponentList>().Initialize(CurrGogglesID);
				} else {
					transform.FindChild ("girl_head").gameObject.SetActive (false);
					GameObject[] HeadGms = transform.FindChild ("girl_head_without_eyes").GetComponent<AvatarComponentList>().ChildComponents;
					GameObject currGoggle = transform.FindChild ("girl_goggles").GetComponent<AvatarComponentList> ().ChildComponents [CurrGogglesID - 1];
					for (int i = 0; i < HeadGms.Length; i++) {
						HeadGms [i].transform.FindChild("baba_goggles").GetComponent<Image> ().sprite = currGoggle.transform.FindChild("baba_goggles").GetComponent<Image> ().sprite;
						Rect r = currGoggle.transform.FindChild ("baba_goggles").GetComponent<RectTransform> ().rect;
						HeadGms [i].transform.FindChild ("baba_goggles").GetComponent<RectTransform> ().sizeDelta = new Vector2 (r.width, r.height);
					}
					transform.FindChild ("girl_head_without_eyes").gameObject.SetActive (true);
					transform.FindChild ("girl_head_without_eyes").GetComponent<AvatarComponentList>().Initialize(CurrHeadId);
				}
//				transform.FindChild ("girl_head").gameObject.SetActive (false);
//				transform.FindChild ("girl_head_without_eyes").gameObject.SetActive (true);
//				transform.FindChild ("girl_head_without_eyes").GetComponent<AvatarComponentList>().Initialize(CurrHeadId);
//				transform.FindChild ("girl_goggles").gameObject.SetActive (true);
//				transform.FindChild ("girl_goggles").GetComponent<AvatarComponentList>().Initialize(CurrGogglesID);
			}
			CheckForGogglesPrice ();
		}

		void EnableBadges()
		{
			if (CurrBadgeID <= 0)
				CurrBadgeID = 1;
			if (IsBoy) {
				if (CurrTab == Tabs.Badges) {
					CurrBadgeID = 1;
					GameObject[] BadgesGms = transform.FindChild ("boy_badges").GetComponent<AvatarComponentList>().ChildComponents;
					GameObject currBodyGm = transform.FindChild ("boy_body").GetComponent<AvatarComponentList> ().ChildComponents [CurrBodyId - 1];
					for (int i = 0; i < BadgesGms.Length; i++) {
						BadgesGms [i].GetComponent<Image> ().sprite = currBodyGm.GetComponent<Image> ().sprite;
						BadgesGms [i].GetComponent<Image> ().color = currBodyGm.GetComponent<Image> ().color;
					}
					transform.FindChild ("boy_body").gameObject.SetActive (false);
					transform.FindChild ("boy_badges").gameObject.SetActive (true);
					transform.FindChild ("boy_badges").GetComponent<AvatarComponentList>().Initialize(CurrBadgeID);
				} else {
					GameObject[] BodyGms = transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().ChildComponents;
					GameObject currBadgeGm = transform.FindChild ("boy_badges").GetComponent<AvatarComponentList> ().ChildComponents [CurrBadgeID - 1];
					for (int i = 0; i < BodyGms.Length; i++) {
						BodyGms [i].transform.FindChild("baba_badges").GetComponent<Image> ().sprite = currBadgeGm.transform.FindChild("baba_badges").GetComponent<Image> ().sprite;
						BodyGms [i].transform.FindChild("baba_badges").GetComponent<Image> ().color = currBadgeGm.transform.FindChild("baba_badges").GetComponent<Image> ().color;
						Rect r = currBadgeGm.transform.FindChild ("baba_badges").GetComponent<RectTransform> ().rect;
						BodyGms [i].transform.FindChild ("baba_badges").GetComponent<RectTransform> ().sizeDelta = new Vector2 (r.width, r.height);
					}
					transform.FindChild ("boy_body").gameObject.SetActive (true);
					transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId);
				}
			} else {
				if (CurrTab == Tabs.Badges) {
					CurrBadgeID = 1;
					GameObject[] BadgesGms = transform.FindChild ("girl_badges").GetComponent<AvatarComponentList>().ChildComponents;
					GameObject currBodyGm = transform.FindChild ("girl_body").GetComponent<AvatarComponentList> ().ChildComponents [CurrBodyId - 1];
					for (int i = 0; i < BadgesGms.Length; i++) {
						BadgesGms [i].GetComponent<Image> ().sprite = currBodyGm.GetComponent<Image> ().sprite;
						BadgesGms [i].GetComponent<Image> ().color = currBodyGm.GetComponent<Image> ().color;
					}
					transform.FindChild ("girl_body").gameObject.SetActive (false);
					transform.FindChild ("girl_badges").gameObject.SetActive (true);
					transform.FindChild ("girl_badges").GetComponent<AvatarComponentList>().Initialize(CurrBadgeID);
				} else {
					GameObject[] BodyGms = transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().ChildComponents;
					GameObject currBadgeGm = transform.FindChild ("girl_badges").GetComponent<AvatarComponentList> ().ChildComponents [CurrBadgeID - 1];
					for (int i = 0; i < BodyGms.Length; i++) {
						BodyGms [i].transform.FindChild("baba_badges").GetComponent<Image> ().sprite = currBadgeGm.transform.FindChild("baba_badges").GetComponent<Image> ().sprite;
						BodyGms [i].transform.FindChild("baba_badges").GetComponent<Image> ().color = currBadgeGm.transform.FindChild("baba_badges").GetComponent<Image> ().color;
						Rect r = currBadgeGm.transform.FindChild ("baba_badges").GetComponent<RectTransform> ().rect;
						BodyGms [i].transform.FindChild ("baba_badges").GetComponent<RectTransform> ().sizeDelta = new Vector2 (r.width, r.height);
					}
					transform.FindChild ("girl_body").gameObject.SetActive (true);
					transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId);
				}
			}
			CheckForBadgesPrice ();
		}

		void DisableBadges()
		{
			if (IsBoy) {
				GameObject[] BodyGms = transform.FindChild ("boy_body").GetComponent<AvatarComponentList> ().ChildComponents;
				for (int i = 0; i < BodyGms.Length; i++) {
					BodyGms [i].transform.FindChild ("baba_badges").GetComponent<Image> ().sprite = null;
					BodyGms [i].transform.FindChild ("baba_badges").GetComponent<Image> ().color = new Color (0, 0, 0, 0);
				}
			} else {
				GameObject[] BodyGms = transform.FindChild ("girl_body").GetComponent<AvatarComponentList> ().ChildComponents;
				for (int i = 0; i < BodyGms.Length; i++) {
					BodyGms [i].transform.FindChild ("baba_badges").GetComponent<Image> ().sprite = null;
					BodyGms [i].transform.FindChild ("baba_badges").GetComponent<Image> ().color = new Color (0, 0, 0, 0);
				}
			}
		}

		void resetAvatarWithOnlyPurchasedAccessories()
		{
			DisableAllComponents ();
			EnableBasicAvatar ();
			if (!LaunchList.instance.mAvatar.PurchasedHats.Contains (CurrHatID - 1)) {
				CurrHatID = -1;
			} else {
				EnableHats ();
			}
			if (!LaunchList.instance.mAvatar.PurchasedGoggles.Contains (CurrGogglesID - 1)) {
				CurrGogglesID = -1;
			} else {
				EnableGoggles ();
			}
			if (!LaunchList.instance.mAvatar.PurchasedBadges.Contains (CurrBadgeID - 1)) {
				CurrBadgeID = -1;
				DisableBadges ();
			} else {
				EnableBadges ();
			}
			if (CurrTab == Tabs.Hats) {
				EnableHats ();
			}
			if (CurrTab == Tabs.Goggles) {
				EnableGoggles ();
			}
			if (CurrTab == Tabs.Badges) {
				EnableBadges ();
			}
		}

		public void EnableAvatarSelection()
		{
			CurrTab = Tabs.Avatar;
			ResetAllTabs ();
			TabularButtons [0].transform.FindChild ("Enable").gameObject.SetActive (true);
			resetAvatarWithOnlyPurchasedAccessories ();
			if (IsBoy) {
				if (BoyButton && GirlButton) {
					BoyButton.SetActive (false);
					GirlButton.SetActive (true);
				}
			} else {
				if (BoyButton && GirlButton) {
					BoyButton.SetActive (true);
					GirlButton.SetActive (false);
				}
			}
			ProcessNextPrevButtons ();
		}

		public void EnableHatsSelection()
		{
			CurrTab = Tabs.Hats;
			ResetAllTabs ();
			TabularButtons [1].transform.FindChild ("Enable").gameObject.SetActive (true);
			resetAvatarWithOnlyPurchasedAccessories ();
			ProcessNextPrevButtons ();
		}

		public void EnableGogglesSelection()
		{
			CurrTab = Tabs.Goggles;
			ResetAllTabs ();
			TabularButtons [2].transform.FindChild ("Enable").gameObject.SetActive (true);
			Debug.Log ("enable tab goggles");
			resetAvatarWithOnlyPurchasedAccessories ();
			ProcessNextPrevButtons ();
		}

		public void EnableBadgesSelection()
		{
			CurrTab = Tabs.Badges;
			ResetAllTabs ();
			TabularButtons [3].transform.FindChild ("Enable").gameObject.SetActive (true);
			resetAvatarWithOnlyPurchasedAccessories ();
			ProcessNextPrevButtons ();
		}

		void DisableAllComponents()
		{
			transform.FindChild ("boy_body").gameObject.SetActive (false);
			transform.FindChild ("boy_head").gameObject.SetActive (false);
			transform.FindChild ("boy_hair").gameObject.SetActive (false);
			transform.FindChild ("boy_head_without_eyes").gameObject.SetActive (false);
			transform.FindChild ("boy_goggles").gameObject.SetActive (false);
			transform.FindChild ("boy_hats").gameObject.SetActive (false);
			transform.FindChild ("boy_badges").gameObject.SetActive (false);
			transform.FindChild ("girl_body").gameObject.SetActive (false);
			transform.FindChild ("girl_head").gameObject.SetActive (false);
			transform.FindChild ("girl_hair_back").gameObject.SetActive (false);
			transform.FindChild ("girl_hair_front").gameObject.SetActive (false);
			transform.FindChild ("girl_head_without_eyes").gameObject.SetActive (false);
			transform.FindChild ("girl_goggles").gameObject.SetActive (false);
			transform.FindChild ("girl_hats").gameObject.SetActive (false);
			transform.FindChild ("girl_badges").gameObject.SetActive (false);
			if (BoyButton)
				BoyButton.SetActive (false);
			if(GirlButton)
				GirlButton.SetActive (false);
			CoinsLabel.SetActive (false);
			PurchaseButton.SetActive (false);
		}

		void ResetAllTabs()
		{
			for (int i = 0; i < TabularButtons.Length; i++) {
				TabularButtons [i].transform.FindChild ("Enable").gameObject.SetActive (false);
			}
		}

		void EnableCoinsLabel(int coins)
		{
			if (coins <= 0) {
				DisableCoinsLabel ();
				return;
			}
			CoinsLabel.SetActive (true);
			PurchaseButton.SetActive (true);
			CoinsLabel.transform.FindChild ("Value").GetComponent<Text>().text = coins.ToString();
		}

		void DisableCoinsLabel()
		{
			CoinsLabel.SetActive (false);
			PurchaseButton.SetActive (false);
		}

		public void BoyButtonPressed()
		{
			CurrAvatar.BodyID = 1;
			CurrAvatar.HairID = 1;
			CurrAvatar.HeadID = 1;
			CurrAvatar.HatID = CurrHatID;
			CurrAvatar.GogglesID = CurrGogglesID;
			CurrAvatar.BadgeID = CurrBadgeID;
			CurrAvatar.IsBoy = true;
			InitializeAvatar ();
			BoyButton.SetActive (false);
			GirlButton.SetActive (true);
		}

		public void GirlButtonPressed()
		{
			CurrAvatar.BodyID = 5;
			CurrAvatar.HairID = 5;
			CurrAvatar.HeadID = 5;
			CurrAvatar.HatID = CurrHatID;
			CurrAvatar.GogglesID = CurrGogglesID;
			CurrAvatar.BadgeID = CurrBadgeID;
			CurrAvatar.IsBoy = false;
			InitializeAvatar ();
			BoyButton.SetActive (true);
			GirlButton.SetActive (false);
		}

		public void SaveButtonPressed()
		{
			string BabaId = "";
			BabaId += CurrHairID + (IsBoy?0:4);
			BabaId += CurrHeadId + (IsBoy?0:4);
			BabaId += CurrBodyId + (IsBoy?0:4);
			ProgressCircle.SetActive (true);
			CurrHatID = CurrHatID > 0 ? (CurrHatID - 1) : -1;
			CurrGogglesID = CurrGogglesID > 0 ? (CurrGogglesID - 1) : -1;
			CurrBadgeID = CurrBadgeID > 0 ? (CurrBadgeID - 1) : -1;
			HTTPRequestHelper.instance.SendAvatarSet (BabaId, CurrHatID, CurrGogglesID, CurrBadgeID, OnSaveResponse);
		}

		public void PurchaseButtonPressed()
		{
			if (CurrTab == Tabs.Hats) {
				if (LaunchList.instance.mCurrentStudent.Coins >= HatsCoins [CurrHatID - 1]) {
					LaunchList.instance.mAvatar.PurchasedHats.Add(CurrHatID - 1);
					LaunchList.instance.SetCoins (HatsCoins [CurrHatID - 1] * -1);
					DisableCoinsLabel();
				} else {
					GenericPopup = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.GenericPopup, transform.parent);
					GenericPopup.transform.SetAsLastSibling ();
					GenericPopup.GetComponent<GenericPopup> ().Initialise ("Not enough coins.", 1, false);
				}
			} else if (CurrTab == Tabs.Goggles) {
				if (LaunchList.instance.mCurrentStudent.Coins >= GogglesCoins [CurrGogglesID - 1]) {
					LaunchList.instance.mAvatar.PurchasedGoggles.Add(CurrGogglesID - 1);
					LaunchList.instance.SetCoins (GogglesCoins [CurrGogglesID - 1] * -1);
					DisableCoinsLabel();
				} else {
					GenericPopup = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.GenericPopup, transform.parent);
					GenericPopup.transform.SetAsLastSibling ();
					GenericPopup.GetComponent<GenericPopup> ().Initialise ("Not enough coins.", 1, false);
				}
			} else if (CurrTab == Tabs.Badges) {
				if (LaunchList.instance.mCurrentStudent.Coins >= BadgesCoins [CurrBadgeID - 1]) {
					LaunchList.instance.mAvatar.PurchasedBadges.Add(CurrBadgeID - 1);
					LaunchList.instance.SetCoins (BadgesCoins [CurrBadgeID - 1] * -1);
					DisableCoinsLabel();
				} else {
					GenericPopup = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.GenericPopup, transform.parent);
					GenericPopup.transform.SetAsLastSibling ();
					GenericPopup.GetComponent<GenericPopup> ().Initialise ("Not enough coins.", 1, false);
				}
			}
			LaunchList.instance.WriteAvatar (LaunchList.instance.mAvatar);
			TotalCoinsLabel.GetComponent<Text> ().text = LaunchList.instance.mCurrentStudent.Coins.ToString();
		}

		public void OnSaveResponse(bool IsSuccess)
		{
			ProgressCircle.SetActive (false);
			if (IsSuccess) {
				string BabaId = "";
				BabaId += CurrHairID + (IsBoy?0:4);
				BabaId += CurrHeadId + (IsBoy?0:4);
				BabaId += CurrBodyId + (IsBoy?0:4);
				PlayerPrefs.SetString (PlayerPrefKeys.BabaID, BabaId);
				LaunchList.instance.mAvatar.HairID = CurrHairID + (IsBoy?0:4);
				LaunchList.instance.mAvatar.HeadID = CurrHeadId + (IsBoy?0:4);
				LaunchList.instance.mAvatar.BodyID = CurrBodyId + (IsBoy?0:4);
				LaunchList.instance.mAvatar.HatID = CurrHatID;
				LaunchList.instance.mAvatar.GogglesID = CurrGogglesID;
				LaunchList.instance.mAvatar.BadgeID = CurrBadgeID;
				LaunchList.instance.WriteAvatar (LaunchList.instance.mAvatar);
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

		public void ProcessNextPrevButtons()
		{
			if (CurrTab == Tabs.Avatar) {
				for (int i = 0; i < 6; i++) {
					NextPrevButtons [i].SetActive (true);
				}
				NextPrevButtons [0].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				NextPrevButtons [1].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				if (CurrBodyId == 4) {
					NextPrevButtons [0].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
				if (CurrBodyId == 1) {
					NextPrevButtons [1].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}

				NextPrevButtons [2].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				NextPrevButtons [3].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				if (CurrHeadId == 4) {
					NextPrevButtons [2].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
				if (CurrHeadId == 1) {
					NextPrevButtons [3].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}

				NextPrevButtons [4].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				NextPrevButtons [5].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				if ((IsBoy && CurrHairID == 3) || CurrHatID != -1) {
					NextPrevButtons [4].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
				if ((!IsBoy && CurrHairID == 4) || CurrHatID != -1) {
					NextPrevButtons [4].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
				if (CurrHairID == 1 || CurrHatID != -1) {
					NextPrevButtons [5].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
			} else if (CurrTab == Tabs.Hats) {
				for (int i = 0; i < 6; i++) {
					NextPrevButtons [i].SetActive (false);
				}
				NextPrevButtons [2].SetActive (true);
				NextPrevButtons [3].SetActive (true);
				NextPrevButtons [2].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				NextPrevButtons [3].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				if (CurrHatID == 6) {
					NextPrevButtons [2].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
				if (CurrHatID == 1) {
					NextPrevButtons [3].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
			} else if (CurrTab == Tabs.Goggles) {
				for (int i = 0; i < 6; i++) {
					NextPrevButtons [i].SetActive (false);
				}
				NextPrevButtons [2].SetActive (true);
				NextPrevButtons [3].SetActive (true);
				NextPrevButtons [2].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				NextPrevButtons [3].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				if (CurrGogglesID == 6) {
					NextPrevButtons [2].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
				if (CurrGogglesID == 1) {
					NextPrevButtons [3].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
			} else if (CurrTab == Tabs.Badges) {
				for (int i = 0; i < 6; i++) {
					NextPrevButtons [i].SetActive (false);
				}
				NextPrevButtons [2].SetActive (true);
				NextPrevButtons [3].SetActive (true);
				NextPrevButtons [2].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				NextPrevButtons [3].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				if (CurrBadgeID == 6) {
					NextPrevButtons [2].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
				if (CurrBadgeID == 1) {
					NextPrevButtons [3].transform.FindChild ("Image").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0.3f);
				}
			}
		}

		public void CenterNextButtonPressed()
		{
			if (CurrTab == Tabs.Avatar) {
				HeadNextPressed ();
			} else if (CurrTab == Tabs.Hats) {
				HatNextPressed ();
			} else if (CurrTab == Tabs.Goggles) {
				GogglesNextPressed ();
			} else if (CurrTab == Tabs.Badges) {
				BadgeNextPressed ();
			}
		}

		public void CenterPreviousButtonPressed()
		{
			if (CurrTab == Tabs.Avatar) {
				HeadPreviousPressed ();
			} else if (CurrTab == Tabs.Hats) {
				HatPreviousPressed ();
			} else if (CurrTab == Tabs.Goggles) {
				GogglesPreviousPressed ();
			} else if (CurrTab == Tabs.Badges) {
				BadgePreviousPressed ();
			}
		}

		public void BodyNextPressed()
		{
			if (CurrBodyId == 4 || IsTransitionOn) {
				return;
			}

			CurrBodyId++;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().NextButtonPressed(CurrBodyId);
			} else {
				transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().NextButtonPressed(CurrBodyId);
			}

			ProcessNextPrevButtons ();
		}

		public void BodyPreviousPressed()
		{
			if (CurrBodyId == 1 || IsTransitionOn) {
				return;
			}

			CurrBodyId--;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrBodyId);
			} else {
				transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrBodyId);
			}

			ProcessNextPrevButtons ();
		}

		public void HeadNextPressed()
		{
			if (CurrHeadId == 4 || IsTransitionOn) {
				return;
			}

			CurrHeadId++;
			IsTransitionOn = true;
			if (CurrGogglesID > 1) {
				if (IsBoy) {
					transform.FindChild ("boy_head_without_eyes").GetComponent<AvatarComponentList> ().NextButtonPressed (CurrHeadId);
				} else {
					transform.FindChild ("girl_head_without_eyes").GetComponent<AvatarComponentList> ().NextButtonPressed (CurrHeadId);
				}
			} else {
				if (IsBoy) {
					transform.FindChild ("boy_head").GetComponent<AvatarComponentList> ().NextButtonPressed (CurrHeadId);
				} else {
					transform.FindChild ("girl_head").GetComponent<AvatarComponentList> ().NextButtonPressed (CurrHeadId);
				}
			}

			ProcessNextPrevButtons ();
		}

		public void HeadPreviousPressed()
		{
			if (CurrHeadId == 1 || IsTransitionOn) {
				return;
			}

			CurrHeadId--;
			IsTransitionOn = true;
			if (CurrGogglesID > 1) {
				if (IsBoy) {
					transform.FindChild ("boy_head_without_eyes").GetComponent<AvatarComponentList> ().PreviousButtonPressed (CurrHeadId);
				} else {
					transform.FindChild ("girl_head_without_eyes").GetComponent<AvatarComponentList> ().PreviousButtonPressed (CurrHeadId);
				}
			} else {
				if (IsBoy) {
					transform.FindChild ("boy_head").GetComponent<AvatarComponentList> ().PreviousButtonPressed (CurrHeadId);
				} else {
					transform.FindChild ("girl_head").GetComponent<AvatarComponentList> ().PreviousButtonPressed (CurrHeadId);
				}
			}

			ProcessNextPrevButtons ();
		}

		public void HairNextPressed()
		{
			// if hat is enabled then return.
			if (CurrHatID != -1)
				return;
			if (IsBoy && (CurrHairID == 3 || IsTransitionOn)) {
				return;
			}
			else if (CurrHairID == 4 || IsTransitionOn) {
				return;
			}

			CurrHairID++;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHairID);
			} else {
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHairID);
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHairID);
			}

			ProcessNextPrevButtons ();
		}

		public void HairPreviousPressed()
		{
			// if hat is enabled then return.
			if (CurrHatID != -1)
				return;
			if (CurrHairID == 1 || IsTransitionOn) {
				return;
			}

			CurrHairID--;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHairID);
			} else {
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHairID);
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHairID);
			}

			ProcessNextPrevButtons ();
		}

		public void HatNextPressed()
		{
			if (CurrHatID == 6 || IsTransitionOn) {
				return;
			}

			CurrHatID++;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_hats").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHatID);
			} else {
				transform.FindChild ("girl_hats").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHatID);
			}

			if (IsBoy) {
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHairID);
			} else {
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHairID);
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHairID);
			}

			ProcessNextPrevButtons ();
			CheckForHatPrice ();
		}

		public void HatPreviousPressed()
		{
			if (CurrHatID == 1 || IsTransitionOn) {
				return;
			}

			CurrHatID--;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_hats").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHatID);
			} else {
				transform.FindChild ("girl_hats").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHatID);
			}

			if (IsBoy) {
				transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHairID);
			} else {
				transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHairID);
				transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHairID);
			}

			ProcessNextPrevButtons ();
			CheckForHatPrice ();
		}

		void CheckForHatPrice()
		{
			if (LaunchList.instance.mAvatar.PurchasedHats.Contains (CurrHatID - 1)) {
				DisableCoinsLabel ();
			} else {
				EnableCoinsLabel (HatsCoins[CurrHatID - 1]);
			}
		}

		public void GogglesNextPressed()
		{
			if (CurrGogglesID == 6 || IsTransitionOn) {
				return;
			}

			CurrGogglesID++;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_goggles").GetComponent<AvatarComponentList>().NextButtonPressed(CurrGogglesID);
			} else {
				transform.FindChild ("girl_goggles").GetComponent<AvatarComponentList>().NextButtonPressed(CurrGogglesID);
			}

			if (IsBoy) {
				transform.FindChild ("boy_head").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHeadId);
			} else {
				transform.FindChild ("girl_head").GetComponent<AvatarComponentList>().NextButtonPressed(CurrHeadId);
			}

			ProcessNextPrevButtons ();
			CheckForGogglesPrice ();
		}

		public void GogglesPreviousPressed()
		{
			if (CurrGogglesID == 1 || IsTransitionOn) {
				return;
			}

			CurrGogglesID--;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_goggles").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrGogglesID);
			} else {
				transform.FindChild ("girl_goggles").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrGogglesID);
			}

			if (IsBoy) {
				transform.FindChild ("boy_head").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHeadId);
			} else {
				transform.FindChild ("girl_head").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrHeadId);
			}

			ProcessNextPrevButtons ();
			CheckForGogglesPrice ();
		}

		void CheckForGogglesPrice()
		{
			if (LaunchList.instance.mAvatar.PurchasedGoggles.Contains (CurrGogglesID - 1)) {
				DisableCoinsLabel ();
			} else {
				EnableCoinsLabel (GogglesCoins[CurrGogglesID - 1]);
			}
		}

		public void BadgeNextPressed()
		{
			if (CurrBadgeID == 6 || IsTransitionOn) {
				return;
			}

			CurrBadgeID++;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_badges").GetComponent<AvatarComponentList>().NextButtonPressed(CurrBadgeID);
			} else {
				transform.FindChild ("girl_badges").GetComponent<AvatarComponentList>().NextButtonPressed(CurrBadgeID);
			}

			ProcessNextPrevButtons ();
			CheckForBadgesPrice ();
		}

		public void BadgePreviousPressed()
		{
			if (CurrBadgeID == 1 || IsTransitionOn) {
				return;
			}

			CurrBadgeID--;
			IsTransitionOn = true;
			if (IsBoy) {
				transform.FindChild ("boy_badges").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrBadgeID);
			} else {
				transform.FindChild ("girl_badges").GetComponent<AvatarComponentList>().PreviousButtonPressed(CurrBadgeID);
			}

			ProcessNextPrevButtons ();
			CheckForBadgesPrice ();
		}

		void CheckForBadgesPrice()
		{
			if (LaunchList.instance.mAvatar.PurchasedBadges.Contains (CurrBadgeID - 1)) {
				DisableCoinsLabel ();
			} else {
				EnableCoinsLabel (BadgesCoins[CurrBadgeID - 1]);
			}
		}
	}

	public class Avatar
	{
		public int BodyID, HairID, HeadID;
		public int HatID, GogglesID, BadgeID;
		public string ColorId;
		public bool IsBoy;
		public List<int> PurchasedHats, PurchasedGoggles, PurchasedBadges;
	}
}