//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace MaterialUI
{
	public class EmptyTextValidator : ITextValidator
	{
		public bool ValidateText(string text, Text validationText)
		{
			if (string.IsNullOrEmpty(text))
			{
				validationText.text = "Can't be empty";
				return true;
			}

			return false;
		}
	}

	public class EmailTextValidator : ITextValidator
	{
		public bool ValidateText(string text, Text validationText)
		{
			Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
			Match match = regex.Match(text);
			if (!match.Success)
			{
				validationText.text = "Format is invalid";
				return true;
			}

			return false;
		}
	}

	public class NameTextValidator : ITextValidator
	{
		private int m_MinimumLength = 3;

		public NameTextValidator() { }
		public NameTextValidator(int minimumLength)
		{
			m_MinimumLength = minimumLength;
		}

		public bool ValidateText(string text, Text validationText)
		{
			if (text.Length < m_MinimumLength)
			{
				validationText.text = "Format is invalid";
				return true;
			}

			Regex regex = new Regex(@"^\p{L}+(\s+\p{L}+)*$");
			Match match = regex.Match(text);
			if (!match.Success)
			{
				validationText.text = "Format is invalid";
				return true;
			}

			return false;
		}
	}

	public class PasswordTextValidator : ITextValidator
	{
		private int m_MinimumLength = 6;

		public PasswordTextValidator() { }
		public PasswordTextValidator(int minimumLength)
		{
			m_MinimumLength = minimumLength;
		}

		public bool ValidateText(string text, Text validationText)
		{
			if (text.Length < m_MinimumLength)
			{
				validationText.text = "Require at least " + m_MinimumLength + " characters";
				return true;
			}

			return false;
		}
	}

	public class SamePasswordTextValidator : ITextValidator
	{
		private InputField m_OriginalPasswordInputField;

		public SamePasswordTextValidator() { }
		public SamePasswordTextValidator(InputField originalPasswordInputField)
		{
			m_OriginalPasswordInputField = originalPasswordInputField;
		}

		public bool ValidateText(string text, Text validationText)
		{
			if (!text.Equals(m_OriginalPasswordInputField.text))
			{
				validationText.text = "Passwords are different!";
				return true;
			}

			return false;
		}
	}
}