//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using MaterialUI;
using UnityEngine.UI;
using UnityEngine;

public class Example07LengthValidation : MonoBehaviour, ITextValidator
{
    public bool ValidateText(string text, Text validationText)
    {
        if (text.Length <= 10)
        {
            return false;
        }
        else
        {
            validationText.text = "Must be at most 10 characters";
            return true;
        }
    }
}