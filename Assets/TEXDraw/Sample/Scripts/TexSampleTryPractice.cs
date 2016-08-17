using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using TexDrawLib;
using System.Text.RegularExpressions;

public class TexSampleTryPractice : MonoBehaviour
{

    public InputField input;
    public Text suggestTxt;

    public Text warningText;
    public GameObject warningBox;
    public TEXDraw tex;

    const string emptySuggestion =
        @"Just type backslash '\' and suggestions will come here

HINT: everythings suggested here is for defining possible symbols
backslashes also work for giving commands like roots, fraction, etc.

there's a lot of symbols, around +600 symbols defined in this package
remember that every symbols you typed are all CASE-sensitive.

superscript and subscript also worked here, type '^' or '_' after a character

Go to the TEXDraw documentations for full guide about this package.";

    const string startSuggestion = 
        @"You just type a backslash, type a character to seek possible symbols

If you want to give commands, go try some example below:

Fraction: <b>\frac{</b>abc<b>}{</b>123<b>}</b>
Root: <b>\root{</b>4<b>}</b> or <b>\\root[</b>3<b>]{</b>10<b>}</b>
Matrix: <b>\matrix{</b>1&3|4&2<b>}</b>
Negations: <b>\not{</b>3x<b>}</b>

please note that these braces: '{}' have to be in a pair in order to make it works properly.

Nested commands also welcomed here.";
        
    const string illegalSuggestion = 
        @"Finish or Check every pairs of braces in order to make it work again";
        
    const string floatScriptSuggestion = 
        @"In order to make a nested script, please put them in a group of brace first,
Example: <i>3^(4^8)</i> not <i>3^4^8</i>";

    const string emptyGapSuggestion = 
        @"Please fill the empty group (an empty space is good)";
    
    // When input text gets changed ...
    public void InputUpdate()
    {
        //Standard Update....
        tex.text = input.text;

        //Go find some suggestions...
        string typed = DetectTypedSymbol(input.text, input.caretPosition);
        string suggest;
        if (string.IsNullOrEmpty(typed))
            suggest = emptySuggestion;
        else if (typed == "\\")
            suggest = startSuggestion;
        else
            suggest = GetPossibleSymbols(typed.Substring(1));
        suggestTxt.text = suggest;
    }

    void Update()
    {
        //for warning informations, it can't be updated instantly
        //since changes happen only when repainting call
        warningBox.SetActive(tex.debugReport != string.Empty);
        warningText.text = tex.debugReport;
        if(!string.IsNullOrEmpty(tex.debugReport))
        {
            if(tex.debugReport.Contains("illegal"))
                suggestTxt.text = illegalSuggestion;
            if(tex.debugReport.Contains("float"))
                suggestTxt.text = floatScriptSuggestion;
            if(tex.debugReport.Contains("empty"))
                suggestTxt.text = emptyGapSuggestion;
        }

    }

    public string DetectTypedSymbol(string full, int caretPos)
    {
        string watchedStr = input.text.Substring(0, input.caretPosition);
        return Regex.Match(watchedStr, @"\\[\w]*$").Value;
    }

    public string GetPossibleSymbols(string raw)
    {
        string repRaw = "<b>" + raw + "</b>";
        return string.Join("\n", 
            tex.pref.symbolData.keys.FindAll(x => FuncPair(x, raw))
            .ConvertAll<string>(x => x.Replace(raw, repRaw)).ToArray());
    }

    bool FuncPair(string x, string raw)
    {
        return x.Contains(raw);
        //return x.Length >= raw.Length && x.Substring(0, raw.Length) == raw;
    }

    public void UpdateAlignment(int alignment)
    {
        tex.alignment = new Vector2(alignment / 2f, 0.5f);
    }

    public void AlignmentLeft(bool yes)
    {
        if(yes)
            UpdateAlignment(0);
    }

    public void AlignmentCenter(bool yes)
    {
        if(yes)
            UpdateAlignment(1);
    }

    public void AlignmentRight(bool yes)
    {
        if(yes)
            UpdateAlignment(2);
    }

    public void UpdateWrap(int wrap)
    {
        tex.autoWrap = (TEXDraw.WrappingMethod)wrap;
    }

}
