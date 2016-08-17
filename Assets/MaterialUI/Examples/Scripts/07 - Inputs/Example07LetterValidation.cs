//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Text.RegularExpressions;
using MaterialUI;
using UnityEngine.UI;
using UnityEngine;

public class Example07LetterValidation : MonoBehaviour, ITextValidator
{
    public bool ValidateText(string text, Text validationText)
    {
        if (new Regex("[^a-zA-Z ]").IsMatch(text))
        {
            validationText.text = "Must only contain letters";
            return true;
        }
        else
        {
            return false;
        }
    }
}