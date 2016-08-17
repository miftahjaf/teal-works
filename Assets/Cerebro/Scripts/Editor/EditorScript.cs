using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using MaterialUI;
using System.IO;

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
using UnityEditor;
#endif

namespace Cerebro
{
	public class EditorScript : MonoBehaviour
	{
		public Font theFont;

		#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
		[MenuItem ("Tools/Clear PlayerPrefs")]
		private static void NewMenuOption ()
		{
			PlayerPrefs.DeleteAll ();
			DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
			FileInfo[] info = dir.GetFiles("*.txt");
			for (int i = 0; i < info.Length; i++) {
				File.Delete (info [i].FullName);
			}
			FileInfo[] infoImages = dir.GetFiles("*.jpg");
			for (int i = 0; i < infoImages.Length; i++) {
				File.Delete (infoImages [i].FullName);
			}
		}

		[MenuItem ("Assets/Replace Fonts")]
		private static void ReplaceFonts ()
		{
			Font fnt = Resources.Load<Text> ("Fonts/SampleText").font;
			for (var i = 0; i < Selection.objects.Length; i++) {
				GameObject gameObject = Selection.gameObjects [i];
				ChangeFont (gameObject, fnt);
			}
		}

		private static void ChangeFont (GameObject go, Font fnt)
		{
			if (go.GetComponent<Text> () != null) {
				go.GetComponent<Text> ().font = fnt;
			}
			for (var i = 0; i < go.transform.childCount; i++) {
				ChangeFont (go.transform.GetChild (i).gameObject, fnt);
			}
		}

		[MenuItem ("Assets/Replace ProgressCircle")]
		private static void ReplaceProgressCircle ()
		{
			for (var i = 0; i < Selection.objects.Length; i++) {
				GameObject gameObject = Selection.gameObjects [i];
				CheckProgressCircle (gameObject,gameObject);
			}
		}

		private static void CheckProgressCircle (GameObject main, GameObject go)
		{
			if (go.name == "ProgressCircle") {
				if (go.transform.Find ("CircleMask").Find ("Icon").GetComponent<VectorImage> () != null) {
					CerebroHelper.DebugLog ("Found in " + main.name);
				}
			}
			for (var i = 0; i < go.transform.childCount; i++) {
				CheckProgressCircle (main,go.transform.GetChild (i).gameObject);
			}
		}

		[MenuItem ("Assets/Change Panel")]
		private static void ChangePanel ()
		{
			GameObject Canvas = GameObject.Find ("Canvas").gameObject;
			for (var i = 0; i < Selection.objects.Length; i++) {
				Object prefab = PrefabUtility.InstantiatePrefab (Selection.gameObjects [i]);
				GameObject gameObject = prefab as GameObject;
				gameObject.transform.SetParent (Canvas.transform);
				gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);

				if (gameObject.transform.Find ("Panel")) {
					Image panel = gameObject.transform.Find ("Panel").GetComponent<Image> ();
					panel.color = new Color (1f, 1f, 1f, 1f);
					panel.sprite = Resources.Load <Sprite> ("Images/Keyboard_bg-1");
				}
				if (gameObject.transform.Find ("numPad")) {
					if (gameObject.transform.Find ("numPad").Find ("BG")) {
						DestroyImmediate (gameObject.transform.Find ("numPad").Find ("BG").gameObject);
					}
				}
				PrefabUtility.ReplacePrefab (gameObject, PrefabUtility.GetPrefabParent (prefab));
				DestroyImmediate (gameObject);
			}
		}

		private static void CheckFontSize (GameObject go)
		{
			if (go.GetComponent<TEXDraw> () != null) {
				go.GetComponent<TEXDraw> ().size = 30;
			}
			for (var i = 0; i < go.transform.childCount; i++) {
				CheckFontSize (go.transform.GetChild (i).gameObject);
			}
		}

		[MenuItem ("Assets/Edit AssessmentPrefabs")]
		private static void EditAssessmentPrefabs ()
		{
			Font fntBook = Resources.Load<Text> ("Fonts/SampleTextBook").font;
			Font fntLight = Resources.Load<Text> ("Fonts/SampleTextLight").font;
			GameObject Canvas = GameObject.Find ("Canvas").gameObject;
			for (var i = 0; i < Selection.objects.Length; i++) {
				Object prefab = PrefabUtility.InstantiatePrefab (Selection.gameObjects [i]);
				GameObject gameObject = prefab as GameObject;
				gameObject.transform.SetParent (Canvas.transform);
				gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);

				if (gameObject.transform.Find ("Question") != null) {
					GameObject Question = gameObject.transform.Find ("Question").gameObject;

					Question.GetComponent<RectTransform> ().localPosition = new Vector3 (50f, 660f, 1f);
					Question.GetComponent<RectTransform> ().sizeDelta = new Vector2 (924f, 200f);

					if (Question.GetComponent<Text> () != null) {
						Question.GetComponent<Text> ().alignment = TextAnchor.UpperCenter;
						Question.GetComponent<Text> ().font = fntLight;
						Question.GetComponent<Text> ().fontSize = 22;
						Question.GetComponent<Text> ().resizeTextForBestFit = false;
						Question.GetComponent<Text> ().color = CerebroHelper.HexToRGB("2D2D38");
					} else if(Question.GetComponent<TEXDraw> () != null) {
						Question.GetComponent<TEXDraw> ().size = 22;
						Question.GetComponent<TEXDraw> ().color = CerebroHelper.HexToRGB("2D2D38");
						Question.GetComponent<TEXDraw> ().alignment = new Vector2 (0.5f, 0f);
					}
				}

				GameObject inputPanel = gameObject.transform.Find ("InputPanel").gameObject;
				if (inputPanel.transform.Find ("subQuestion") != null) {
					GameObject subQuestion = inputPanel.transform.Find ("subQuestion").gameObject;

					subQuestion.GetComponent<RectTransform> ().localPosition = new Vector3 (-462f, -106f, 1f);
					subQuestion.GetComponent<RectTransform> ().sizeDelta = new Vector2 (924f, 150f);

					if (subQuestion.GetComponent<Text> () != null) {
						subQuestion.GetComponent<Text> ().alignment = TextAnchor.UpperCenter;
						subQuestion.GetComponent<Text> ().font = fntBook;
						subQuestion.GetComponent<Text> ().fontSize = 22;
						subQuestion.GetComponent<Text> ().resizeTextForBestFit = false;
						subQuestion.GetComponent<Text> ().color = CerebroHelper.HexToRGB ("2D2D38");
					} else if(subQuestion.GetComponent<TEXDraw> () != null) {
						subQuestion.GetComponent<TEXDraw> ().size = 22;
						subQuestion.GetComponent<TEXDraw> ().color = CerebroHelper.HexToRGB("2D2D38");
						subQuestion.GetComponent<TEXDraw> ().alignment = new Vector2 (0.5f, 0f);
					}

				}
				PrefabUtility.ReplacePrefab (gameObject, PrefabUtility.GetPrefabParent (prefab));
				DestroyImmediate (gameObject);
			}
		}

		[MenuItem ("Assets/Edit Numpad")]
		private static void EditNumPad ()
		{
			GameObject Canvas = GameObject.Find ("Canvas").gameObject;
			GameObject Decimal6 = GameObject.Find ("Decimals6").gameObject;
			GameObject MasterNumPad = Decimal6.transform.Find ("numPad").gameObject;
			GameObject MasterBG = MasterNumPad.transform.Find ("BG").gameObject;
			GameObject MasterBtn = MasterNumPad.transform.Find ("PanelLayer").GetChild (0).gameObject;
			GameObject MasterBGImage = MasterBtn.transform.Find ("Image").gameObject;

			for (var i = 0; i < Selection.objects.Length; i++) {
				Object prefab = PrefabUtility.InstantiatePrefab (Selection.gameObjects [i]);
				GameObject gameObject = prefab as GameObject;
				gameObject.transform.SetParent (Canvas.transform);
				gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);

				GameObject numPad = gameObject.transform.Find ("numPad").gameObject;
				GameObject newbg = GameObject.Instantiate (MasterBG);
				newbg.name = "BG";
				newbg.transform.SetParent (numPad.transform, false);
				newbg.transform.SetAsFirstSibling ();
				newbg.transform.localScale = new Vector3 (1f, 1f, 1f);
				newbg.GetComponent<RectTransform> ().anchorMax = MasterBG.GetComponent<RectTransform> ().anchorMax;
				newbg.GetComponent<RectTransform> ().anchorMin = MasterBG.GetComponent<RectTransform> ().anchorMin;
				newbg.GetComponent<RectTransform> ().pivot = MasterBG.GetComponent<RectTransform> ().pivot;
				newbg.GetComponent<RectTransform> ().sizeDelta = MasterBG.GetComponent<RectTransform> ().sizeDelta;
				newbg.GetComponent<RectTransform> ().localPosition = MasterBG.GetComponent<RectTransform> ().localPosition;
				if (numPad.transform.Find ("Shadow") != null) {
					DestroyImmediate (numPad.transform.Find ("Shadow").gameObject);
				}

				GameObject PanelLayer = numPad.transform.Find ("PanelLayer").gameObject;
				DestroyImmediate (PanelLayer.GetComponent<Image> ());
				DestroyImmediate (PanelLayer.GetComponent<SpriteSwapper> ());

				for (var j = 0; j < PanelLayer.transform.childCount; j++) {
					GameObject btn = PanelLayer.transform.GetChild (j).gameObject;
					btn.GetComponent<RectTransform> ().sizeDelta = new Vector2 (90f, 72f);

					DestroyImmediate (btn.transform.Find ("Image").gameObject);

					if (btn.transform.Find ("Text").GetComponent<Text> () != null) {
						btn.transform.Find ("Text").GetComponent<Text> ().text = btn.transform.Find ("Text").GetComponent<Text> ().text.Trim (" " [0]);
						btn.transform.Find ("Text").GetComponent<Text> ().fontSize = 22;
						btn.transform.Find ("Text").GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
						btn.transform.Find ("Text").GetComponent<Text> ().color = CerebroHelper.HexToRGB ("FFFFFF");
					}

					GameObject newbgimage = GameObject.Instantiate (MasterBGImage);
					newbgimage.name = "Image";
					newbgimage.transform.SetParent (btn.transform, false);
					newbgimage.transform.SetAsFirstSibling ();
					newbgimage.transform.localScale = new Vector3 (1f, 1f, 1f);
					if (btn.name == "Submit") {
						newbgimage.GetComponent<ProceduralImage> ().color = CerebroHelper.HexToRGB ("29CDB1");
					}
					if (btn.name == "Cancel") {
						newbgimage.GetComponent<ProceduralImage> ().color = CerebroHelper.HexToRGB ("FF9633");
					}
					newbgimage.GetComponent<RectTransform> ().anchorMax = MasterBGImage.GetComponent<RectTransform> ().anchorMax;
					newbgimage.GetComponent<RectTransform> ().anchorMin = MasterBGImage.GetComponent<RectTransform> ().anchorMin;
					newbgimage.GetComponent<RectTransform> ().pivot = MasterBGImage.GetComponent<RectTransform> ().pivot;
					newbgimage.GetComponent<RectTransform> ().sizeDelta = MasterBGImage.GetComponent<RectTransform> ().sizeDelta;
					newbgimage.GetComponent<RectTransform> ().localPosition = MasterBGImage.GetComponent<RectTransform> ().localPosition;
				}

				PrefabUtility.ReplacePrefab (gameObject, PrefabUtility.GetPrefabParent (prefab));
				DestroyImmediate (gameObject);
			}
		}

		[MenuItem ("Assets/Edit Buttons")]
		private static void EditButtons ()
		{
			GameObject Canvas = GameObject.Find ("Canvas").gameObject;
			for (var i = 0; i < Selection.objects.Length; i++) {
				Object prefab = PrefabUtility.InstantiatePrefab (Selection.gameObjects [i]);
				GameObject gameObject = prefab as GameObject;
				gameObject.transform.SetParent (Canvas.transform);
				gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
				GameObject continuebutton = gameObject.transform.Find ("ContinueButton").gameObject;
				GameObject flagbutton = gameObject.transform.Find ("FlagButton").gameObject;
				GameObject solutionBtn = gameObject.transform.Find ("SolutionButton").gameObject;
				
				flagbutton.GetComponent<RectTransform> ().sizeDelta = new Vector2 (160f, 56f);
				continuebutton.GetComponent<RectTransform> ().sizeDelta = new Vector2 (160f, 56f);
				solutionBtn.GetComponent<RectTransform> ().sizeDelta = new Vector2 (160f, 56f);

				flagbutton.transform.Find ("Text").GetComponent<Text> ().fontSize = 18;
				continuebutton.transform.Find ("Text").GetComponent<Text> ().fontSize = 18;
				solutionBtn.transform.Find ("Text").GetComponent<Text> ().fontSize = 18;

				flagbutton.transform.Find ("Text").GetComponent<Text> ().color = CerebroHelper.HexToRGB ("29CDB1");
				continuebutton.transform.Find ("Text").GetComponent<Text> ().color = CerebroHelper.HexToRGB ("29CDB1");
				solutionBtn.transform.Find ("Text").GetComponent<Text> ().color = CerebroHelper.HexToRGB ("29CDB1");

				continuebutton.transform.localPosition = new Vector3 (continuebutton.transform.localPosition.x, 154.6f, continuebutton.transform.localPosition.z);
				solutionBtn.transform.localPosition = new Vector3 (solutionBtn.transform.localPosition.x, 71.6f, solutionBtn.transform.localPosition.z);
				flagbutton.transform.localPosition = new Vector3 (flagbutton.transform.localPosition.x, 240.6f, flagbutton.transform.localPosition.z);

				DestroyImmediate (solutionBtn.transform.Find ("Image").gameObject.GetComponent<Image> ());
				DestroyImmediate (solutionBtn.transform.Find ("Image").gameObject.GetComponent<CanvasGroup> ());

				solutionBtn.transform.Find ("Image").gameObject.AddComponent<ProceduralImage> ();
				solutionBtn.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().color = CerebroHelper.HexToRGB ("29CDB1");
				solutionBtn.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().BorderWidth = 1.0f;
				solutionBtn.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().ModifierType = typeof(RoundModifier);

				DestroyImmediate (flagbutton.transform.Find ("Image").gameObject.GetComponent<Image> ());
				DestroyImmediate (flagbutton.transform.Find ("Image").gameObject.GetComponent<CanvasGroup> ());

				flagbutton.transform.Find ("Image").gameObject.AddComponent<ProceduralImage> ();
				flagbutton.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().color = CerebroHelper.HexToRGB ("29CDB1");
				flagbutton.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().BorderWidth = 1.0f;
				flagbutton.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().ModifierType = typeof(RoundModifier);

				DestroyImmediate (continuebutton.transform.Find ("Image").gameObject.GetComponent<Image> ());
				DestroyImmediate (continuebutton.transform.Find ("Image").gameObject.GetComponent<CanvasGroup> ());

				continuebutton.transform.Find ("Image").gameObject.AddComponent<ProceduralImage> ();
				continuebutton.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().color = CerebroHelper.HexToRGB ("29CDB1");
				continuebutton.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().BorderWidth = 1.0f;
				continuebutton.transform.Find ("Image").gameObject.GetComponent<ProceduralImage> ().ModifierType = typeof(RoundModifier);

				PrefabUtility.ReplacePrefab (gameObject, PrefabUtility.GetPrefabParent (prefab));
				DestroyImmediate (gameObject);
			}
		}

		[MenuItem ("Assets/EditSolutionText")]
		private static void EditSolutionText ()
		{
			GameObject Canvas = GameObject.Find ("Canvas").gameObject;
			for (var i = 0; i < Selection.objects.Length; i++) {
				GameObject gameObject = Selection.objects [i] as GameObject;
				GameObject solutionBtn = gameObject.transform.Find ("SolutionButton").gameObject;
				print (solutionBtn.transform.Find ("Text").GetComponent<Text> ().text);
			}
		}

		[MenuItem ("Assets/Check Assessment Prefabs")]
		private static void CheckBaseAssessmentValues ()
		{
			List<string> Problems = new List<string> ();
			for (var i = 0; i < Selection.objects.Length; i++) {
				string name = Selection.gameObjects [i].name;
				if (Selection.gameObjects [i].GetComponent<BaseAssessment> () == null) {
					Problems.Add (name);
				} else {
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().testMode) {
						Problems.Add (name);
					}
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().isRevisitedQuestion) {
						Problems.Add (name);
					}
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().isDestroyed) {
						Problems.Add (name);
					}
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().QuestionText == null && Selection.gameObjects [i].GetComponent<BaseAssessment> ().QuestionLatext == null) {
						Problems.Add (name);
					}
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().numPad == null) {
						Problems.Add (name);
					}
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().inputPanel == null) {
						Problems.Add (name);
					}
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().ContinueBtn == null) {
						Problems.Add (name);
					}
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().FlagButton == null) {
						Problems.Add (name);
					}
					if (Selection.gameObjects [i].GetComponent<BaseAssessment> ().SolutionButton == null) {
						Problems.Add (name);
					}
				}
			}
				
			if (Problems.Count == 0) {
				EditorUtility.DisplayDialog ("Congrats!", "All good!", "Okay");
			} else {
				string problem = "";
				foreach (string item in Problems) {
					problem = problem + item + "\n";
				}
				EditorUtility.DisplayDialog ("Problems in the following prefabs", problem, "Okay");
			}
		}
		#endif

	}
}
