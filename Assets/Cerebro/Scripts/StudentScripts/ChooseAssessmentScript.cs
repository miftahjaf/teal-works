﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DigitalRubyShared;

namespace Cerebro
{
	public class ChooseAssessmentScript : CerebroTestScript
	{
		private List<PracticeItems> assessments;

		[SerializeField]
		private GameObject upArrow;

		[SerializeField]
		private GameObject downArrow;
		[SerializeField]
		private Text testMode;

		private bool testModeActive = false;

		private float totalListContentHeight;
		private float listContentHeight;
		private float listHeight;

		private AssessmentSelector list;
		private bool isAnimating = false;
		private bool isInitialised = false;

		public FingersScript FingerScript;
		private TapGestureRecognizer tapGesture;
	    private TapGestureRecognizer doubleTapGesture;
		private List<string> checkTaps = new List<string> ();
		// Use this for initialization
		void Start ()
		{
			Initialise ();
		}

		private GestureTouch FirstTouch (ICollection<GestureTouch> touches)
		{
			foreach (GestureTouch t in touches) {
				return t;
			}
			return new GestureTouch ();
		}

		private void TapGestureCallback (GestureRecognizer gesture, ICollection<GestureTouch> touches)
		{
			if (gesture.State == GestureRecognizerState.Ended) {
				GestureTouch t = FirstTouch (touches);
				if (t.IsValid ()) {
					checkTaps.Clear ();
				}
			}
		}

		private void CreateTapGesture ()
		{
			tapGesture = new TapGestureRecognizer ();
			tapGesture.Updated += TapGestureCallback;
			FingerScript.AddGesture (tapGesture);
		}

		private void DoubleTapGestureCallback (GestureRecognizer gesture, ICollection<GestureTouch> touches)
		{
			if (gesture.State == GestureRecognizerState.Ended) {
				GestureTouch t = FirstTouch (touches);
				if (t.IsValid ()) {
					if (checkTaps.Count % 2 == 0) {
						if (t.Y > Screen.height - 30) {
							checkTaps.Add (t.X.ToString ());
						} else {
							checkTaps.Clear ();
						}
					} else if (checkTaps.Count % 2 == 1) {
						if (t.Y > Screen.height - 50) {
							checkTaps.Add (t.X.ToString ());
						} else {
							checkTaps.Clear ();
						}
					}
					if (checkTaps.Count >= 2 && CerebroHelper.isTestUser()) {
						if (testModeActive) {
							testModeActive = false;
							testMode.text = "";
						} else {
							testModeActive = true;
							testMode.text = "Test Mode On";
						}
						checkTaps.Clear ();
					}
				}
			}
		}

		private void CreateDoubleTapGesture ()
		{
			doubleTapGesture = new TapGestureRecognizer ();
			doubleTapGesture.NumberOfTapsRequired = 2;
			doubleTapGesture.Updated += DoubleTapGestureCallback;
			FingerScript.AddGesture (doubleTapGesture);
		}

		public void Initialise ()
		{
			if (isInitialised) {
				return;
			}

			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Practice);

			assessments = new List<PracticeItems> ();
			List<PracticeItems> ValidPracticeItems = LaunchList.instance.GetGradePracticeItems ();
			foreach (var obj in ValidPracticeItems) {
				string prefabName = obj.PracticeItemName.Replace (" ", "");
				Object prefab = null;
				try {
					prefab = Resources.Load ("Assessments/" + prefabName);
				} catch (UnityException e) {            
					CerebroHelper.DebugLog (e);
				}
				if (prefab != null) {
					assessments.Add (obj);
				}
				prefab = null;
			}
				
		    CreateTapGesture ();
			CreateDoubleTapGesture ();

			tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;

			isInitialised = true;
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);

			LaunchList.instance.PracticeRegeneration ();

			list = transform.Find ("AssessmentSelector").GetComponent<AssessmentSelector> ();
			list.Initialize (assessments);
			list.AssessmentSelected -= List_AssessmentSelected; //if already done
			list.AssessmentSelected += List_AssessmentSelected;

			upArrow.SetActive (false);
			if (assessments.Count <= 3) {
				downArrow.SetActive (false);
			}
		}


		void List_AssessmentSelected (object sender, System.EventArgs e)
		{
			string practiceId = (e as AssessmentEventArgs).practiceId;
			string KCID = (e as AssessmentEventArgs).KCID;
			string practiceName = LaunchList.instance.mPracticeItems [practiceId].PracticeItemName;
			string prefabName = practiceName.Replace (" ", "");
			 
		    OpenAssessment ("Assessments/" + prefabName, practiceName, null,practiceId,KCID);
		}

		public void DownArrowPressed ()
		{
			var currentValue = (list.scrollElement.value);
			var singleHeight = (listContentHeight) / (totalListContentHeight - listHeight);
			var increment = singleHeight * 3;
			var offset = (currentValue / singleHeight);
			if (offset % 1 > 0.01 && offset % 1 < 0.99) {
				var val = (Mathf.FloorToInt (currentValue / singleHeight)) - 2;
				Go.to (list.scrollElement, 0.2f, new GoTweenConfig ().floatProp ("value", val * singleHeight, false));
			} else {
				Go.to (list.scrollElement, 0.3f, new GoTweenConfig ().floatProp ("value", -increment, true));
			}
		}

		public void UpArrowPressed ()
		{
			var currentValue = (list.scrollElement.value);
			var singleHeight = (listContentHeight) / (totalListContentHeight - listHeight);
			var increment = singleHeight * 3;
			var offset = (currentValue / singleHeight);
			if (offset % 1 > 0.01 && offset % 1 < 0.99) {
				var val = (Mathf.FloorToInt (currentValue / singleHeight)) + 3;
				Go.to (list.scrollElement, 0.2f, new GoTweenConfig ().floatProp ("value", val * singleHeight, false));
			} else {
				Go.to (list.scrollElement, 0.3f, new GoTweenConfig ().floatProp ("value", increment, true));
			}
		}

		public void onScrollValueChanged ()
		{
			if (list == null) {
				return;
			}
			if (list.scrollElement.value <= 0.001) {
//				upArrow.SetActive (true);
				downArrow.SetActive (false);
			} else if (list.scrollElement.value >= 0.99) {
				upArrow.SetActive (false);
//				downArrow.SetActive (true);
			} else {
//				upArrow.SetActive (true);
//				downArrow.SetActive (true);
			}
		}

		public void BackOnScreen ()
		{
			list.RefreshActiveCellViews ();
			if (isAnimating) {
				return;
			}
//			CheckForRegeneration ();
			//list.enhanceScroller.ReloadData();
//			list.RefreshCellData ();
			//StudentScript ss = GameObject.FindGameObjectWithTag ("StudentView").GetComponent<StudentScript> ();
			//ss.ShowHeading ();
		}

		private void OpenAssessment (string type, string title, Mission mission,string practiceId="",string KCId ="")
		{
			
			GameObject assessmentgameobject = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Assessments, gameObject.transform.parent.transform);
			assessmentgameobject.transform.SetAsLastSibling ();

			if (!string.IsNullOrEmpty (KCId))
			{
				assessmentgameobject.GetComponent<AssessmentScript> ().Initialize (gameObject, practiceId, KCId, testModeActive);
			} 
			else if (mission == null) 
			{
				assessmentgameobject.GetComponent<AssessmentScript> ().Initialize (type, title, gameObject, testModeActive, practiceId, KCId, practiceId);
			}
			else
			{
				assessmentgameobject.GetComponent<AssessmentScript> ().Initialize (gameObject, mission);
			}

			gameObject.SetActive (false);
		}

		public void BackPressed ()
		{
			//StudentScript ss = GameObject.FindGameObjectWithTag ("StudentView").GetComponent<StudentScript> ();
			//ss.Initialise ();
			WelcomeScript.instance.ShowScreen (false);
			Destroy (gameObject);
		}

		public override void ForceOpenScreen (string[] screen, int index, Mission mission)
		{
			Initialise ();
			string nextScreen = null;
			if (index < screen.Length - 1) {
				nextScreen = screen [index + 1];
			}
			OpenScreen (screen [index], mission);
			if (nextScreen != null) {
				// Go one step deeper here.
			}
		}
		public override void ForceOpenScreen (string[] screen, int index, MissionItemData missionItemData)
		{
		}

		public override string[] GetOptions ()
		{
			return new string[]{ };
		}

		public void OpenScreen (string assessmentName, Mission mission)
		{
			
			string prefabName = assessmentName.Replace (" ", "");
			OpenAssessment ("Assessments/" + prefabName, assessmentName, mission);
		}

	}
}