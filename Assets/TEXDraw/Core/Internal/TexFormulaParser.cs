using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

namespace TexDrawLib
{
    public class TexFormulaParser
    {
        // Special characters for parsing
        private const string extraSpace = "w";
        private const string extraSpaceSoft = "s";
        private const char escapeChar = '\\';

        private const char leftGroupChar = '{';
        private const char rightGroupChar = '}';
        private const char leftBracketChar = '[';
        private const char rightBracketChar = ']';

        private const char subScriptChar = '_';
        private const char superScriptChar = '^';
       
        private static string[] commands = new string[]
        {
            "root", "vmatrix", "matrix", "text", "math", "mclr", "clr", "not", "nnot", "hnot", "dnot", "unot", "onot", "vnot", "vnnot",
            "hold", "vhold", "bhold", "link", "ulink", "frac", "nfrac", "lfrac", "rfrac", "nlfrac", "nrfrac", "under", "over", "table",
            "vtable", "ltable", "rtable", "color", "size"
  
            // Uncomment here if you want more control on fractions
            /* , "llfrac", "rrfrac", "nllfrac", "nrrfrac", "lrfrac", "rlfrac", "nlrfrac", "nrlfrac", 
                 "clfrac", "lcfrac", "crfrac", "clfrac", "nclfrac", "nlcfrac", "ncrfrac", "nclfrac"
             */ 
        };

        private static bool IsSymbol(char c)
        {
            return TexUtility.RenderFont < 0 && !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
        }

        private static bool IsParserReserved(char c)
        {
            return (c == escapeChar) || (c == leftGroupChar) || (c == rightGroupChar) || (c == subScriptChar) || (c == superScriptChar);
        }

        private static bool IsWhiteSpace(char ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
        }

        public TexFormulaParser()
        {
        }

        public TexFormula Parse(string value)
        {
            var formula = ObjPool<TexFormula>.Get();
            var position = 0;
            while (position < value.Length) {
                char ch = value[position];
                if (IsWhiteSpace(ch)) {
                    formula.Add(AttachScripts(formula, value, ref position, SpaceAtom.Get()));
                    position++;
                } else if (ch == escapeChar) { 
                    ProcessEscapeSequence(formula, value, ref position);
                } else if (ch == leftGroupChar) {
                    formula.Add(AttachScripts(formula, value, ref position, Parse(ReadGroup(formula, value, 
                                    ref position, leftGroupChar, rightGroupChar)).GetRoot));
                } else if (ch == rightGroupChar) {
                    position++;
                } else if (ch == superScriptChar || ch == subScriptChar) {
                    formula.Add(AttachScripts(formula, value, ref position, SpaceAtom.Get(0, TexUtility.spaceHeight, 0)));
                } else {
                    var scriptsAtom = AttachScripts(formula, value, ref position,
                                          ConvertCharacter(formula, value, ref position, ch));
                    formula.Add(scriptsAtom);
                }
            }
            if (formula.RootAtom == null)
                formula.Add(SpaceAtom.Get());
            return formula;
        }

        StringBuilder builderGroup = new StringBuilder();

        private string ReadGroup(TexFormula formula, string value, ref int position, char openChar, char closeChar)
        {
            if (position == value.Length)
                return string.Empty;

            var result = builderGroup;
            builderGroup.Remove(0, builderGroup.Length);
            var group = 0;
            var readCloseGroup = true;
            if (value[position] != openChar)
                readCloseGroup = false;
            else
                position++;
            while (position < value.Length && !(value[position] == closeChar && group == 0)) {
                if (value[position] == escapeChar) {
                    result.Append(value[position]);
                    position++;
                    if (position == value.Length) {
                        // Reached end of formula but group has not been closed.
                        return result.ToString();
                    }
                } else if (value[position] == openChar)
                    group++;
                else if (value[position] == closeChar)
                    group--;
                result.Append(value[position]);
                position++;
            }

            if (position == value.Length) {
                // Reached end of formula but group has not been closed.
                //throw new TexParseException("illegal end,  missing '" + closeChar + "'!");
                return result.ToString();
            }

            if (readCloseGroup)
                position++;
            return result.ToString();
        }

        private TexFormula ReadScript(TexFormula formula, string value, ref int position)
        {
            if (position == value.Length)
                //throw new TexParseException("illegal end, missing script!");
                return formula;

            SkipWhiteSpace(value, ref position);
            var ch = value[position];
            if (ch == leftGroupChar) {
                return Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
            } else {
                position++;
                return Parse(ch.ToString());
            }
        }

        private Atom ProcessCommand(TexFormula formula, string value, ref int position, string command)
        {
            SkipWhiteSpace(value, ref position);
            if (position == value.Length)
                return null;

            switch (command) {
                case "root":
                // Command is radical.

                    TexFormula degreeFormula = null;
                    if (value[position] == leftBracketChar) {
                        // Degree of radical- is specified.
                        degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar));
                        SkipWhiteSpace(value, ref position);
                    }
                    return Radical.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
                        .GetRoot, degreeFormula == null ? null : degreeFormula.GetRoot);
                case "vmatrix":
                case "matrix":
                //Command is Matrix
                    MatrixAtom matrixAtom = MatrixAtom.Get();
                    List<List<Atom>> childs = matrixAtom.Elements;

                    Atom parsedChild = (Parse(ReadGroup(
                                               formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot);
                    childs.Add(ListPool<Atom>.Get());
                    if (parsedChild == null)
                        MatrixAtom.Last(childs).Add(SpaceAtom.Get());
                    if (parsedChild is RowAtom) {
                        List<Atom> el = ((RowAtom)parsedChild).Elements;
                        if (command == "matrix")
                            MatrixAtom.ParseMatrix(el, childs);
                        else
                            MatrixAtom.ParseMatrixVertical(el, childs);
                        el.Clear();
                        ObjPool<RowAtom>.Release((RowAtom)parsedChild);
                    } else
                        MatrixAtom.Last(childs).Add(parsedChild);
                    matrixAtom.Elements = childs;
                    return matrixAtom;
                    
                case "math":
                case "text":
                    int idx = TEXPreference.main.defaultTypefaces[TexCharKind.Text];
                    if (value[position] == leftBracketChar) {
                        int.TryParse(ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar), out idx);
                        SkipWhiteSpace(value, ref position);
                    } else if (command == "math")
                        idx = -1;

                    var oldType = TexUtility.RenderFont;
                    TexUtility.RenderFont = idx;
                    var parsed = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot;
                    TexUtility.RenderFont = oldType;
                    return parsed;
                case "clr":
                case "mclr":
                case "color":

                    string clr = null;
                    if (value[position] == leftBracketChar) {
                        clr = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                        SkipWhiteSpace(value, ref position);
                    }

                    if (position == value.Length)
                        return null;
                    AttrColorAtom endColor;
                    var startColor = AttrColorAtom.Get(clr, command == "color" ? 1 : (command == "clr" ? 0 : 2), out endColor);
                    return InsertAttribute(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot, startColor, endColor);
                case "size":
                    string sz = null;
                    if (value[position] == leftBracketChar) {
                        sz = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                        SkipWhiteSpace(value, ref position);
                    }

                    if (position == value.Length)
                        return null;

                    return AttrSizeAtom.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
                    .GetRoot, sz);
                case "link":
                case "ulink":
                    string meta = null;
                    if (value[position] == leftBracketChar) {
                        meta = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                        SkipWhiteSpace(value, ref position);
                    }

                    if (position == value.Length)
                        return null;

                    string groupBrack = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                    if (meta == null)
                        meta = groupBrack;
                    return AttrLinkAtom.Get(Parse(groupBrack).GetRoot, meta, command == "ulink");
                case "under":
                    command = "unot";
                    break;
                case "over":
                    command = "onot";
                    break;
            }
            if (command.Length > 2 && command.Substring(command.Length - 3) == "not") {
                int NotMode = 0;
                string prefix = command.Substring(0, command.Length - 3);
                if (prefix.Length > 0) {
                    switch (prefix[0]) {
                        case 'n':
                            NotMode = 1;
                            break;
                        case 'h':
                            NotMode = 2;
                            break;
                        case 'd':
                            NotMode = 3;
                            break;
                        case 'u':
                            NotMode = 4;
                            break;
                        case 'o':
                            NotMode = 5;
                            break;
                        case 'v':
                            if (prefix.Length > 1 && prefix[1] == 'n')
                                NotMode = 7;
                            else
                                NotMode = 6;
                            break;
                    }
                }
                if (position == value.Length)
                    return null;

                string sz = null;
                if (value[position] == leftBracketChar) {
                    sz = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                    SkipWhiteSpace(value, ref position);
                }

                return NegateAtom.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
						.GetRoot, NotMode, sz);
                
            }
            if (command.Length > 3 && command.Substring(command.Length - 4) == "frac") {
                
                int FracAlignT = 0, FracAlignB = 0;
                bool FracAlignN = true; 
                string prefix = command.Substring(0, command.Length - 4);
                if (prefix.Length > 0) {
                    if (prefix[0] == 'n') {
                        FracAlignN = false;
                        prefix = prefix.Substring(1);
                    }
                    if (prefix.Length == 1) {
                        FracAlignT = fracP(prefix[0]);
                        FracAlignB = FracAlignT;
                    } else if (prefix.Length == 2) {
                        FracAlignT = fracP(prefix[0]);
                        FracAlignB = fracP(prefix[1]);
                    }
                }
                if (position == value.Length)
                    return null;
                Atom numeratorFormula = null, denominatorFormula = null;
                numeratorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot;
                SkipWhiteSpace(value, ref position);
                if (position != value.Length)
                    denominatorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot;
                
                return FractionAtom.Get(numeratorFormula, denominatorFormula, FracAlignN,
                    (TexAlignment)FracAlignT, (TexAlignment)FracAlignB);
            }
            if (command.Length > 4 && command.Substring(command.Length - 5) == "table") {
                bool vertical = false;
                int align = 1 + 8 + 64;
                string prefix = command.Substring(0, command.Length - 5);
                if (prefix.Length > 0) {
                    if (prefix[0] == 'v') {
                        vertical = true;
                        prefix = prefix.Substring(1);
                    }
                    if (prefix.Length == 1) {
                        var pref = fracP(prefix[0]);
                        align = Math.Max(1, pref * 2) + Math.Max(8, pref * 16) + Math.Max(64, pref * 128);
                    } else if (prefix.Length == 3) {
                        var pref0 = fracP(prefix[0]);
                        var pref1 = fracP(prefix[1]);
                        var pref2 = fracP(prefix[2]);
                        align = Math.Max(1, pref0 * 2) + Math.Max(8, pref1 * 16) + Math.Max(64, pref2 * 128);
                    }
                }
	            
                int lineStyleH = 0, lineStyleV = 0;
                if (value[position] == leftBracketChar) {
                    string lineOpt;
                    int lineP = 0;
                    lineOpt = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                    for (int i = 0; i < lineOpt.Length; i++) {
                        if (!int.TryParse(lineOpt[i].ToString(), out	lineP))
                            continue;
                        if (i >= 6)
                            break;
                        switch (i) {
                            case 0:
                                lineStyleH += lineP >= 2 ? 17 : lineP;
                                break;
                            case 1:
                                lineStyleH += lineP >= 2 ? 10 : (lineP == 1 ? 2 : 0);
                                break;
                            case 2:
                                lineStyleH += lineP >= 1 ? 4 : 0;
                                break;
                            case 3:
                                lineStyleV += lineP >= 2 ? 17 : lineP;
                                break;
                            case 4:
                                lineStyleV += lineP >= 2 ? 10 : (lineP == 1 ? 2 : 0);
                                break;
                            case 5:
                                lineStyleV += lineP >= 1 ? 4 : 0;
                                break;				            
                        }
                    }
                    SkipWhiteSpace(value, ref position);
                } else {
                    lineStyleH = 7;
                    lineStyleV = 7;
                }
	            
	            
                List<List<Atom>> childs = new List<List<Atom>>();
                MatrixAtom matrixAtom = ObjPool<MatrixAtom>.Get();
                matrixAtom.horizontalAlign = align;
                matrixAtom.horizontalLine = lineStyleH;
                matrixAtom.verticalLine = lineStyleV;

                Atom parsedChild = (Parse(ReadGroup(
                                           formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot);
                childs.Add(ListPool<Atom>.Get());
                if (parsedChild == null)
                    MatrixAtom.Last(childs).Add(SpaceAtom.Get());
                if (parsedChild is RowAtom) {
                    List<Atom> el = ((RowAtom)parsedChild).Elements;
                    if (!vertical)
                        MatrixAtom.ParseMatrix(el, childs);
                    else
                        MatrixAtom.ParseMatrixVertical(el, childs);
                    el.Clear();
                    ObjPool<RowAtom>.Release((RowAtom)parsedChild);
                } else
                    MatrixAtom.Last(childs).Add(parsedChild);
                matrixAtom.Elements = childs;
                return matrixAtom;

            }
            throw new TexParseException("Invalid command.");
        }

        static int fracP(char c)
        {
            switch (c) {
                case 'l':
                    return 1;
                case 'c':
                    return 0;
                case 'r':
                    return 2;
                default:
                    return 0;
            }
        }

        private void ProcessEscapeSequence(TexFormula formula, string value, ref int position)
        {
            var result = new StringBuilder();
            position++;
            while (position < value.Length) {
                var ch = value[position];
                var isEnd = position == value.Length - 1;
                if (!char.IsLetter(ch) || isEnd) {
                    // Escape sequence has ended.
                    if (char.IsLetter(ch)) {
                        result.Append(ch);
                        position++;
                    }
                    break;
                }

                result.Append(ch);
                position++;
            }

            var command = result.ToString();

            if (command.Length == 0) {
                if (position < value.Length) {
                    var nextChar = value[position];
                    if (IsParserReserved(nextChar))
                        formula.Add(ConvertCharacter(formula, value, ref position, nextChar));
                }
                return;
            }

            SymbolAtom symbolAtom = SymbolAtom.GetAtom(command);

            TexFont fontID = TEXPreference.main.GetFontByID(command);
				
            if (symbolAtom != null) {
                // Symbol was found.
                if (symbolAtom.GetRightType() == CharType.Accent && formula.RootAtom != null) {
                    //Accent is Found
                    Atom baseAtom = formula.RootAtom;
                    /*if (baseAtom is RowAtom) {
                        var row = (RowAtom)baseAtom;
                        baseAtom = MatrixAtom.Last(row.Elements);
                        row.Elements.RemoveAt(row.Elements.Count - 1);
                    } else*/
                    {
                        formula.RootAtom = null;
                    }
                    formula.Add(AttachScripts(formula, value, ref position, AccentedAtom.Get(baseAtom, symbolAtom)));   
                } else
                    formula.Add(AttachScripts(formula, value, ref position, symbolAtom));
            } else if (command.Equals(extraSpace)) {
                // Space was found.

                formula.Add(AttachScripts(formula, value, ref position, SpaceAtom.Get()));
            } else if (fontID != null) {
                SkipWhiteSpace(value, ref position);
                if (position == value.Length)
                    return;
                FontStyle style = TexUtility.RenderFontStyle;
                if (value[position] == leftBracketChar) {
                    string prefix = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                    SkipWhiteSpace(value, ref position);
                    switch (prefix) {
                        case "b":
                            style = FontStyle.Bold;
                            break;
                        case "i":
                            style = FontStyle.Italic;
                            break;
                        case "bi":
                            style = FontStyle.BoldAndItalic;
                            break;
                        default:
                            style = FontStyle.Normal;
                            break;
                    }
                }

                var oldType = TexUtility.RenderFont;
                var oldStyle = TexUtility.RenderFontStyle;
                TexUtility.RenderFont = fontID.index;
                TexUtility.RenderFontStyle = style;
                var parsed = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot;
                TexUtility.RenderFont = oldType;
                TexUtility.RenderFontStyle = oldStyle;
                formula.Add(parsed);
                
            } else if (command.Equals(extraSpaceSoft)) {
                // Soft Space was found.

                formula.Add(AttachScripts(formula, value, ref position, SpaceAtom.Get(TexUtility.lineThickness / 3, 0, 0)));
            } else if (commands.Contains(command)) {
                // Command was found.

                formula.Add(AttachScripts(formula, value, ref position, ProcessCommand(formula, value, ref position,
                            command)));
            } else {
                // Command aren't defined, use it as bolded text style
                RowAtom row = RowAtom.Get();
                for (int i = 0; i < command.Length; i++) {
                    var charAtom = CharAtom.Get(command[i], TEXPreference.main.defaultTypefaces[TexCharKind.Commands]);
                    row.Add(charAtom);
                }
                formula.Add(AttachScripts(formula, value, ref position, row));
            }

        }

        private Atom AttachScripts(TexFormula formula, string value, ref int position, Atom atom)
        {
            if (position == value.Length)
                return atom;
            if (position == value.Length - 1) {
                if (value[position] == superScriptChar || value[position] == subScriptChar) {
                    position++;
                    return atom;
                }
            }

            TexFormula superscriptFormula = null;
            TexFormula subscriptFormula = null;

            bool markAsBig = false;

            var ch = value[position];
            if (ch == superScriptChar) {
                // Attahch superscript.
                position++;
                if (value[position] == superScriptChar) {
                    markAsBig = true;
                    position++;
                }
                superscriptFormula = ReadScript(formula, value, ref position);

                if (position < value.Length && value[position] == subScriptChar) {
                    // Attach subscript also.
                    position++;
                    if (value[position] == subScriptChar) {
                        markAsBig = true;
                        position++;
                    }
                    subscriptFormula = ReadScript(formula, value, ref position);
                }
            } else if (ch == subScriptChar) {
                // Add subscript.
                position++;
                if (value[position] == subScriptChar) {
                    markAsBig = true;
                    position++;
                }
                subscriptFormula = ReadScript(formula, value, ref position);

                if (position < value.Length && value[position] == superScriptChar) {
                    // Attach superscript also.
                    position++;
                    if (value[position] == superScriptChar) {
                        markAsBig = true;
                        position++;
                    }
                    superscriptFormula = ReadScript(formula, value, ref position);
                }
            }

            if (superscriptFormula == null && subscriptFormula == null)
                return atom;

            // Check whether to return Big Operator or Scripts.
            if (atom.GetRightType() == CharType.BigOperator || markAsBig)
                return BigOperatorAtom.Get(atom, subscriptFormula == null ? null : subscriptFormula.GetRoot,
                    superscriptFormula == null ? null : superscriptFormula.GetRoot);
            else
                return ScriptsAtom.Get(atom, subscriptFormula == null ? null : subscriptFormula.GetRoot,
                    superscriptFormula == null ? null : superscriptFormula.GetRoot);
        }

        private Atom ConvertCharacter(TexFormula formula, string value, ref int position, char character)
        {
            position++;
            if (IsSymbol(character)) {
                // Character is symbol (and math).
                var charIdx = TEXPreference.main.charMapData.GetOrNone(character, -1);
                if (charIdx >= 0)
                    return SymbolAtom.Get(TEXPreference.main.GetChar(charIdx));
                if ((int)character >= 0x7f)
                    return UnicodeAtom.Get(TEXPreference.main.defaultTypefaces[TexCharKind.Unicode], character);
                throw new TexParseException("Unknown " + character.ToString() + " Character");
            } else {
                // Character is alpha-numeric.
                if ((int)character >= 0x7f)
                    return UnicodeAtom.Get(TexUtility.RenderFont, character);
                return CharAtom.Get(character);
            }
        }

        private void SkipWhiteSpace(string value, ref int position)
        {
            while (position < value.Length && IsWhiteSpace(value[position])) {
                position++;
            }
        }

        private Atom InsertAttribute(Atom atom, Atom begin, Atom end)
        {
            if (!(atom is RowAtom))
                atom = RowAtom.Get(atom);
            var row = (RowAtom)atom;
            row.Add(end);
            row.Elements.Insert(0, begin);
            return row;
        }
    }
}