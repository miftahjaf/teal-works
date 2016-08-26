using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MathFunctions : MonoBehaviour {

	public static int GetHCF(int a, int b) {
		return b == 0 ? a : GetHCF(b, a % b);
	}

	public static int GetLCM(int a, int b, int c) {
		int lcm1 = GetLCM (a, b);
		int lcmFinal = GetLCM (lcm1, c);
		return lcmFinal;
	}

	public static int GetLCM(int a, int b) {
		int num1, num2;
		if (a > b)
		{
			num1 = a; num2 = b;
		}
		else
		{
			num1 = b; num2 = a;
		}

		for (int i = 1; i <= num2; i++)
		{
			if ((num1 * i) % num2 == 0)
			{
				return i * num1;
			}
		}
		return num2;
	}

	public static string GetMultiples(int num, int count) {
		List<string> factors = new List<string> ();
		for (int i = 1; i <= count; i++) {
			factors.Add ((num * i).ToString ());
		}
		return string.Join(",", factors.ToArray());
	}

	public static string GetFactors(int num) {
		List<string> factors = new List<string> ();
		factors.Add ("1");
		for (int i = 2; i <= num/2; i++) {
			if((num % i) == 0) {
				factors.Add (i.ToString());
			}
		}
		factors.Add (num.ToString());
		return string.Join(",", factors.ToArray());
	}

	public static bool checkFractions (string[] userAnswers, string[] correctAnswers)
	{
		float num1 = -1;
		if (float.TryParse (userAnswers [0], out num1)) {
			num1 = float.Parse (userAnswers [0]);
		} else {
			return false;
		}
		float num2 = float.Parse (correctAnswers [0]);
		float den2 = 1f;
		if (correctAnswers.Length == 2) {
			den2 = float.Parse (correctAnswers [1]);
		}

		float den1 = 1;
		if (userAnswers.Length == 2) {
			if (float.TryParse (userAnswers [1], out den1)) {
				den1 = float.Parse (userAnswers [1]);
			} else {
				return false;
			}
		}

		if (Mathf.Abs((num1 / den1) - (num2 / den2)) < 0.01) {
			return true;
		}
		return false;
	}
}
