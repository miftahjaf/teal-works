﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Cerebro {

	public class MathFunctions : MonoBehaviour {


		public static char deg = '˚';
		public static char min = '\'';
		public static char sec = '\"';

		public static int GetHCF(int a, int b) 
		{
			return b == 0 ? a : GetHCF(b, a % b);
		}

		public static int GetHCF(int a, int b, int c) 
		{
			return GetHCF (GetHCF (a, b), c);
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

		public static float GetRounded(float number, int places)
		{
			places = Mathf.Clamp (places, 1, 10);
			return (float)System.Math.Round (number, places, System.MidpointRounding.AwayFromZero);
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

		public static bool checkAlgebraicExpressions (string CorrectAnswer, string UserAnswer)  // Negi - without braces of any type
		{
			int length = UserAnswer.Length;
			string tempCorrectAnswer = string.Copy (CorrectAnswer);
			string tempUserAnswer = string.Copy (UserAnswer);

			if (tempUserAnswer [0] != '-' && tempUserAnswer [0] != '+') {
				tempUserAnswer = tempUserAnswer.Insert (0, "+");
				length++;
			}

			for (int i = 1; i < length; i++) {
				if (tempUserAnswer [i] == '{'){
					while (tempUserAnswer [i] != '}') {
						i++;						
					}
				}
				if (tempUserAnswer [i] == '-' || tempUserAnswer [i] == '+') {
					tempUserAnswer = tempUserAnswer.Insert (i, "_");
					length++;
					i++;
				}
			}
			length = tempCorrectAnswer.Length;
			if (tempCorrectAnswer [0] != '-' && tempCorrectAnswer [0] != '+') {
				tempCorrectAnswer = tempCorrectAnswer.Insert (0, "+");
				length++;
			}

			for (int i = 1; i < length; i++) {
				if (tempCorrectAnswer [i] == '{'){
					while (tempCorrectAnswer [i] != '}') {
						i++;						
					}
				}
				if (tempCorrectAnswer [i] == '-' || tempCorrectAnswer [i] == '+') {
					tempCorrectAnswer = tempCorrectAnswer.Insert (i, "_");
					length++;
					i++;
				}
			}
			var splitUserAnswer = tempUserAnswer.Split (new string[] { "_" }, System.StringSplitOptions.None);
			var spliCorrectAnswer = tempCorrectAnswer.Split (new string[] { "_" }, System.StringSplitOptions.None);

			return checkAlgebraicTerms (spliCorrectAnswer, splitUserAnswer);
		}

		public static bool checkAlgebraicTerms (string[] correctTerms, string[] userTerms)
		{
			int NumOfUserTerms = userTerms.Length;
			int NumOfCorrectTerms = correctTerms.Length;
			if (NumOfCorrectTerms != NumOfUserTerms) {
				return false;
			}

			for (int i = 0; i < NumOfCorrectTerms; i++) {
				for (int j = 0; j < NumOfUserTerms; j++) {
					if (checkSingleTerm (correctTerms [i], userTerms [j])) {
						return true;
					}
				}
			}
			return false;
		}

//		 Negi -  chesckSingleTerm checks for single term with + or - sign at the beginning of each term
//		 The term may cantain a '/' sign but not a '*' sign 
		 
		public static bool checkSingleTerm (string correctTerm, string userTerm)
		{
			if (correctTerm.Contains ("/")) {
				string[] splitUserTerm, splitCorrectTerm;
				if (userTerm.Contains ("/")) {
					splitUserTerm = userTerm.Split (new string[] { "/" }, System.StringSplitOptions.None); 
					if (splitUserTerm [0][0] != '-' && splitUserTerm [0][0] != '+') {  
						splitUserTerm [0] = splitUserTerm [0].Insert (0, "+");
					} 
					if (splitUserTerm [1][0] != '-' && splitUserTerm [1][0] != '+') { 
						splitUserTerm [1] = splitUserTerm [1].Insert (0, "+");
					} 
					if (splitUserTerm [0][0] == '-' && splitUserTerm [1][0] == '-') {
						splitUserTerm [0] = splitUserTerm [0].Remove (0, 1);
						splitUserTerm [1] = splitUserTerm [1].Remove (0, 1);
						splitUserTerm [0] = splitUserTerm [0].Insert (0, "+");
						splitUserTerm [1] = splitUserTerm [1].Insert (0, "+");
					} 
					if (splitUserTerm [0][0] == '+' && splitUserTerm [1][0] == '-'){
						splitUserTerm [0] = splitUserTerm [0].Remove (0, 1);
						splitUserTerm [1] = splitUserTerm [1].Remove (0, 1);
						splitUserTerm [0] = splitUserTerm [0].Insert (0, "-");
						splitUserTerm [1] = splitUserTerm [1].Insert (0, "+");
					}
				} else {
					return false;
				}
				splitCorrectTerm = correctTerm.Split (new string[] { "/" }, System.StringSplitOptions.None);
				if (splitCorrectTerm [0][0] != '-' && splitCorrectTerm [0][0] != '+') {  
					splitCorrectTerm [0] = splitCorrectTerm [0].Insert (0, "+");
				} 
				if (splitCorrectTerm [1][0] != '-' && splitCorrectTerm [1][0] != '+') { 
					splitCorrectTerm [1] = splitCorrectTerm [1].Insert (0, "+");
				}

				if (checkAlgebraicTerms (splitCorrectTerm, splitUserTerm)) {
					return true;
				} else {
					return false;
				}
			}

			if (correctTerm.Length != userTerm.Length) {
				return false;
			}

			List<string> splitCorrectTermList = new List<string> ();
			List<string> splitUserTermList = new List<string> ();

			List<int> splitLocationsUserTerm = userTerm.AllIndexesOf ("algebra");
			List<int> splitLocationsCorrectTerm = correctTerm.AllIndexesOf ("algebra");

			if (splitLocationsUserTerm.Count != splitLocationsCorrectTerm.Count) {  //checks whether number of variables are equal
				return false;
			}
				
			int numberOfTerms = splitLocationsCorrectTerm.Count + 1;
			for (int i = 0; i < numberOfTerms - 1; i++)
			{
				splitLocationsUserTerm [i] -= 2;
				splitLocationsCorrectTerm [i] -= 2;
			}
						
			splitCorrectTermList.Add (correctTerm.Substring (0, splitLocationsCorrectTerm [0]));
			splitUserTermList.Add (userTerm.Substring (0, splitLocationsUserTerm [0]));

			for (int i = 0; i < numberOfTerms - 2; i++) {
				splitCorrectTermList.Add (correctTerm.Substring (splitLocationsCorrectTerm [i], splitLocationsCorrectTerm [i + 1] - splitLocationsCorrectTerm [i]));
			}
			for (int i = 0; i < numberOfTerms - 2; i++) {
				splitUserTermList.Add (userTerm.Substring (splitLocationsUserTerm [i], splitLocationsUserTerm [i + 1] - splitLocationsUserTerm [i]));
			}
			splitCorrectTermList.Add (correctTerm.Substring (splitLocationsCorrectTerm [splitLocationsCorrectTerm.Count - 1]));
			splitUserTermList.Add (userTerm.Substring (splitLocationsUserTerm [splitLocationsUserTerm.Count - 1]));
			
			for (int i = 0; i < numberOfTerms; i++) {
				if (!splitCorrectTermList.Contains (splitUserTermList[i]) || !splitUserTermList.Contains (splitCorrectTermList[i])) {
					return false;
				}
			}
			return true;
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

		public static bool checkFractionsSimplestForm (string[] userAnswers, string[] correctAnswers)
		{
			if (userAnswers.Length != correctAnswers.Length)
				return false;

			for (var i = 0; i < correctAnswers.Length; i++) {
				float answer = 0;
				float userAnswer = 0;

				if (float.TryParse (correctAnswers [i], out answer)) {
					answer = float.Parse (correctAnswers [i]);
				} else {
					return false;
				}
				if (float.TryParse (userAnswers [i], out userAnswer)) {
					userAnswer = float.Parse (userAnswers [i]);
				} else {
					return false;
				}
				if (answer != userAnswer) {
					return false;
				}
			}
			return true;
		}

		public static Vector2 PointAtDirection(Vector2 origin,float angle,float radius)
		{
			return new Vector2(origin.x+radius * Mathf.Cos(Mathf.Deg2Rad * angle), origin.y+radius*Mathf.Sin(Mathf.Deg2Rad*angle));
		}

		public static string GetAngleValueInString(float ans)
		{
			string answer = "";

			ans = GetRounded (ans, 5);

			if(Mathf.Floor(ans)!=0)
				answer = "" + Mathf.Floor(ans) + deg;

			ans -= Mathf.Floor(ans);
			ans *= 60f;
			if (ans > 0.0001)
			{
				ans = GetRounded (ans, 3);

				if(Mathf.Floor(ans)!=0)
					answer += "" + Mathf.Floor(ans) + min;
				
				ans -= Mathf.Floor(ans);
				if (ans > 0.001){
					ans *= 60f;
					ans = GetRounded (ans,0);
				    answer += "" + ans + sec;
				}
			}

			return answer;
		}

		public static List<int> GetIntRandomDataSet (int startVal, int endVal, int dataSetLength) 
		{
			List<int> dataSet = new List<int> ();

			for (int i = 0; i < dataSetLength; i++)
				dataSet.Add (Random.Range (startVal, endVal));

			return dataSet;
		}

		public static List<float> GetFloatRandomDataSet (float startVal, float endVal, int dataSetLength, int NumberOfDecimalPlaces = 1) 
		{
			List<float> dataSet = new List<float> ();

			for (int i = 0; i < dataSetLength; i++)
				dataSet.Add (GetRounded (Random.Range (startVal, endVal), NumberOfDecimalPlaces));

			return dataSet;
		}

		public static List<int> GetIntegerMeanDataSet (int startVal, int endVal, int dataSetLength, out int mean)
		{
			List<int> dataSet = GetIntRandomDataSet (startVal, endVal, dataSetLength);
			int average = 0;

			for (int i = 0; i < dataSetLength; i++)
				average += dataSet[i];

			if (average % dataSetLength != 0) 
			{
				for (int i = 0; i < average % dataSetLength; i++)
					dataSet [i] --;
			}
			average -= average % dataSetLength;
			average /= dataSetLength; 

			mean = average;
			return dataSet;
		}

		public static List<float> GetFloatMeanDataSet (float startVal, float endVal, int dataSetLength, out float mean, int NumberOfDecimalPlaces = 1)
		{
			int MeanMultiple;
			List<int> iDataSet = GetIntegerMeanDataSet ((int)(startVal * Mathf.Pow (10, NumberOfDecimalPlaces)), (int)(endVal * Mathf.Pow (10, NumberOfDecimalPlaces)), dataSetLength, out MeanMultiple);
			List<float> fDataSet = new List<float> ();

			foreach (int iDataEntry in iDataSet)
				fDataSet.Add (GetRounded (iDataEntry / Mathf.Pow (10, NumberOfDecimalPlaces), NumberOfDecimalPlaces));

			mean = GetRounded (MeanMultiple / Mathf.Pow (10, NumberOfDecimalPlaces), NumberOfDecimalPlaces);
			return fDataSet;
		}

		public static float GetMedian (List<int> dataSet)
		{
			List<int> localDataSet = new List<int> (dataSet);
			localDataSet.Sort ();
			int dataSetLength = localDataSet.Count;
			float median;

			if (dataSetLength % 2 == 0)
				median = (float) (localDataSet [dataSetLength / 2 - 1] + localDataSet [dataSetLength / 2]) / 2f;
			else
				median = (float) localDataSet [(dataSetLength - 1) / 2]; 

			median = GetRounded (median, 1); 
			return median;
		}

		public static float GetMedian (List<float> dataSet, int NumberOfDecimalPlaces = 1)
		{
			List<float> localDataSet = new List<float> (dataSet);
			localDataSet.Sort ();
			int dataSetLength = localDataSet.Count;
			float median;

			if (dataSetLength % 2 == 0)
				median = (localDataSet [dataSetLength / 2 - 1] + localDataSet [dataSetLength / 2]) / 2f;
			else
				median = localDataSet [(dataSetLength - 1) / 2]; 

			median = GetRounded (median, NumberOfDecimalPlaces + 1); 
			return median;
		}

		public static List<int> GetIntModeDataSet (int startVal, int endVal, int dataSetLength, out List<int> modes, int numberOfModes = 1) 
		{
			List<int> modeDataSet = new List<int> ();
			List<int> usedModeDataset = new List<int> ();

			int maxRep = 2 + Random.Range (0, dataSetLength / 4);
			int modesLength = maxRep * numberOfModes;
			int randomValue;

			if (modesLength > dataSetLength) 
			{
				modes = usedModeDataset;
				Debug.Log ("In GetIntModeDataSet : dataSetLength too small or numberOfModes too large");
				return modeDataSet;
			}

			if (endVal - startVal - numberOfModes + 1 <= 0) 
			{
				modes = usedModeDataset;
				Debug.Log ("In GetIntModeDataSet : Range too small");
				return modeDataSet;
			}
				
			for (int i = 0; i < numberOfModes; i++)
			{
				randomValue = Random.Range (startVal, endVal);

				while (usedModeDataset.Contains (randomValue))
					randomValue = Random.Range (startVal, endVal);
				
				usedModeDataset.Add (randomValue);
			}
			modes = usedModeDataset;

			for (int j = 0; j < numberOfModes; j++) 
			{
				for (int k = 0; k < maxRep; k++) {
					modeDataSet.Add (usedModeDataset [j]);
				}
			}

			for (int i = modesLength; i < dataSetLength; i++) 
			{
				randomValue = Random.Range (startVal, endVal);
				int count = 0;
				while (usedModeDataset.Contains (randomValue) || modeDataSet.FindNumberOfOccurances (randomValue) == maxRep - 1) {
					count++;
					if (count == 100) {
						Debug.Log ("check endvalue : increased to" + endVal);
						endVal++;
					}
					randomValue = Random.Range (startVal, endVal);
				}

				modeDataSet.Add (randomValue);
			}

			modeDataSet.Shuffle ();

			return modeDataSet;
		}

		public static List<float> GetFloatModeDataSet (float startVal, float endVal, int dataSetLength, out List<float> modes, int numberOfModes = 1, int NumberOfDecimalPlaces = 1) 
		{
			List<float> modeDataSet = new List<float> ();
			List<float> usedModeDataset = new List<float> ();

			int maxRep = 2 + Random.Range (0, dataSetLength / 4);
			int modesLength = maxRep * numberOfModes;
			float randomValue;

			if (modesLength > dataSetLength) 
			{
				modes = usedModeDataset;
				Debug.Log ("In GetIntModeDataSet : dataSetLength too small or numberOfModes too large");
				return modeDataSet;
			}

			if (Mathf.Round ((endVal - startVal) * Mathf.Pow (10, NumberOfDecimalPlaces)) - numberOfModes + 1 <= 0) 
			{
				modes = usedModeDataset;
				Debug.Log ("In GetIntModeDataSet : Range too small");
				return modeDataSet;
			}

			for (int i = 0; i < numberOfModes; i++)
			{
				randomValue = GetRounded (Random.Range (startVal, endVal), NumberOfDecimalPlaces);

				while (usedModeDataset.Contains (randomValue))
					randomValue = GetRounded (Random.Range (startVal, endVal), NumberOfDecimalPlaces);

				usedModeDataset.Add (randomValue);
			}
			modes = usedModeDataset;

			for (int j = 0; j < numberOfModes; j++) 
			{
				for (int k = 0; k < maxRep; k++) {
					modeDataSet.Add (usedModeDataset [j]);
				}
			}

			for (int i = modesLength; i < dataSetLength; i++) 
			{
				randomValue = GetRounded (Random.Range (startVal, endVal), NumberOfDecimalPlaces);
				int count = 0;
				while (usedModeDataset.Contains (randomValue) || modeDataSet.FindNumberOfOccurances (randomValue) == maxRep - 1) {
					count++;
					if (count == 100) {
						Debug.Log ("check endvalue : increased to" + endVal);
						endVal += GetRounded (1f / Mathf.Pow (10, NumberOfDecimalPlaces), NumberOfDecimalPlaces);
					}
					randomValue = GetRounded (Random.Range (startVal, endVal), NumberOfDecimalPlaces);
				}
				modeDataSet.Add (randomValue);
			}

			modeDataSet.Shuffle ();

			return modeDataSet;
		}

		public static string GetMonthName (int monthName)
		{
			return new System.DateTime(2015, monthName, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("en"));
		}

		public static Vector3 GetLineParamters ( Vector2 point1, Vector2 point2)
		{
			//(y1 – y2)x + (x2 – x1)y + (x1y2 – x2y1) = 0
			return new Vector3 (point1.y - point2.y  ,point2.x - point1.x, point1.x * point2.y - point2.x * point1.y );
		}

		public static bool IsValidLinePoint( Vector3 lineParameters ,Vector2 point)
		{
			return lineParameters.x * point.x + lineParameters.y * point.y + lineParameters.z == 0;
		}

		public static float GetPointX( Vector3 lineParameters ,float y)
		{
			return lineParameters.x ==0 ? 0: (- lineParameters.z - lineParameters.y * y) /lineParameters.x;
		}

		public static float GetPointY( Vector3 lineParameters ,float x)
		{
			return lineParameters.y ==0 ? 0: (- lineParameters.z - lineParameters.x * x) /lineParameters.y;
		}
	}
}