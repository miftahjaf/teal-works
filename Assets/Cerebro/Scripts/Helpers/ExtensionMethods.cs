using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cerebro {
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

		public static void Shuffle<T>(this List<T> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}

		public static int FindNumberOfOccurances<T>(this T[] array, T value)  
		{  
			if (array == null)
				return 0;
			
			int count = 0;
			int length = array.Length;
			for (int i = 0; i < length; i++) 
			{
				if (value.Equals(array [i]))
					count++;
			}
			return count;
		}

		public static int FindNumberOfOccurances<T>(this List<T> list, T value)  
		{  
			if (list == null)
				return 0;

			int count = 0;
			int length = list.Count;
			for (int i = 0; i < length; i++) 
			{
				if (value.Equals(list [i]))
					count++;
			}
			return count;
		}

		public static string Algebra(this string symbol)
		{
			return symbol;
		}
	}
}