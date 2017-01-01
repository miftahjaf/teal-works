using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Cerebro
{
	public class AssessmentObject : MonoBehaviour 
	{
		private Text TitleText;
		private Image[] ScoreImages;
		private Color EnableColor, DisableColor;

		public void InitializeAssessment(string titleText, int score)
		{
			TitleText = transform.FindChild ("Title").GetComponent<Text> ();
			ScoreImages = new Image[6];
			for (int i = 0; i < 6; i++) {
				ScoreImages [i] = transform.FindChild ("Score" + (i+1)).GetComponent<Image> ();
			}
			EnableColor = CerebroHelper.HexToRGB ("72737C");
			DisableColor = CerebroHelper.HexToRGB ("CECECE");

			TitleText.text = titleText;
			for (int i = 0; i < 6; i++) {
				if (i < score) {
					ScoreImages [i].color = EnableColor;
					ScoreImages [i].transform.FindChild ("Text").GetComponent<Text> ().color = new Color (0.9f, 0.9f, 0.9f, 1f);
				} else {
					ScoreImages [i].color = DisableColor;
					ScoreImages [i].transform.FindChild ("Text").GetComponent<Text> ().color = new Color (0.1f, 0.1f, 0.1f, 1f);
				}
			}
		}

	}
}
