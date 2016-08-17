using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionComplete : MonoBehaviour {

	public Text message;

	private string[] pool1 = new string[]{"Just Missed it","Expert Marksman","You just killed it","Hello Mr. Robinhood Patel"};
	private string[] pool2 = new string[]{"King Bruce was lying in a cave .. Observing the persevering spider .. It changed his life","Hard work always pays … always.", "Cerebro is very intelligent .. It knows exactly what you should do next", "Practice makes a (wo)man perfect !!!","Try and Fail .. But don’t Fail to Try" };
	private string[] pool3 = new string[]{"Cerebro Khush hua :D :D","With great accuracy comes great responsibility .. The responsibility to ace the next mission","Well done Mr. Einstein"};

	void Start() {
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
		GetComponent<RectTransform> ().position = new Vector3 (0f, 0f);
	}

	public void Initialise(float accuracy) {
		string msg = "";
		if (accuracy == 100) {
			message.text = pool3 [Random.Range (0, pool3.Length)];
		} else if (accuracy > 75) {
			message.text = pool1 [Random.Range (0, pool1.Length)];
		} else {
			message.text = pool2 [Random.Range (0, pool2.Length)];
		}
	}
	public void BackPressed() {
		Destroy (gameObject);
	}
}
