using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

namespace Cerebro {
	public static class StringHelper {

		public static string RemoveNumbers(string str) {
			string newText = "";
			for(int i = 0; i < str.Length; i++){
				if ((str[i] < 48) || (str[i] > 57)){ 
					newText += str[i];
				}
			}
			return newText;
		}

		public static string CreateSpacesFromCamelCase(string str) {
			string newText = "";
			for(int i = 0; i < str.Length; i++){
				if ((str [i] < 65) || (str [i] > 90)) { 
					newText += str [i];
				} else {
					newText += (" " + str [i]);
				}
			}
			return newText;
		}

		public static string Reverse( string s )
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse( charArray );
			return new string( charArray );
		}

		public static string RemoveUnicodeCharacters(string text) {
			string result = Regex.Replace(text, @"\p{Cs}", "");
			return result;
		}
	}
}
