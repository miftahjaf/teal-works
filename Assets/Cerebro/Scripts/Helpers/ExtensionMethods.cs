using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
	private static System.Random rng = new System.Random();  
	public static void Shuffle<T>(this T[] array)  
	{  
		int n = array.Length;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = array[k];  
			array[k] = array[n];  
			array[n] = value;  
		}  
	}

}
