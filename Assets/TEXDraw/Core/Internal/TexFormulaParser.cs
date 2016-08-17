using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            "root", "vmatrix", "matrix", "text", "color", "not", "nnot", "hnot", "dnot", "frac", "nfrac", "lfrac", "rfrac", "nlfrac", "nrfrac"

            // Uncomment here if you want more control on fractions
            /*, "llfrac", "rrfrac", "nllfrac", "nrrfrac", "lrfrac", "rlfrac", "nlrfrac", "nrlfrac", 
            "clfrac", "lcfrac", "crfrac", "clfrac", "nclfrac", "nlcfrac", "ncrfrac", "nclfrac"*/ 
        };

        private static bool IsSymbol(char c)
        {
            return !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
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
            while (position < value.Length)
            {
                char ch = value[position];
                if (IsWhiteSpace(ch))
                {
                    formula.Add(AttachScripts(formula, value, ref position, SpaceAtom.Get()));
                    position++;
                }
                else if (ch == escapeChar)
                { 
                    ProcessEscapeSequence(formula, value, ref position);
                }
                else if (ch == leftGroupChar)
                {
                    formula.Add(AttachScripts(formula, value, ref position, Parse(ReadGroup(formula, value, 
                                    ref position, leftGroupChar, rightGroupChar)).GetRoot));
                }
                else if (ch == rightGroupChar)
                {
                    throw new TexParseException("Found a closing '" + rightGroupChar
                        + "' without an opening '" + leftGroupChar + "'!");
                }
                else if (ch == superScriptChar || ch == subScriptChar)
                {
                    if (position == 0)
                        throw new TexParseException(string.Format(
                                "Every script needs a base: '{0}' and '{1}' can't be the first character!", 
                                superScriptChar, subScriptChar));
                    else
                        throw new TexParseException("float scripts found! Try using more braces.");
                }
                else
                {
                    var scriptsAtom = AttachScripts(formula, value, ref position,
                           ConvertCharacter(formula, value, ref position, ch));
                    formula.Add(scriptsAtom);
                }
            }

            return formula;
        }

        private string ReadGroup(TexFormula formula, string value, ref int position, char openChar, char closeChar)
        {
            if (position == value.Length || value[position] != openChar)
                throw new TexParseException("something wrong, missing '" + openChar + "'!");

            var result = new StringBuilder();
            var group = 0;
            position++;
            while (position < value.Length && !(value[position] == closeChar && group == 0))
            {
                if (value[position] == openChar)
                    group++;
                else if (value[position] == closeChar)
                    group--;
                result.Append(value[position]);
                position++;
            }

            if (position == value.Length)
            {
                // Reached end of formula but group has not been closed.
                throw new TexParseException("illegal end,  missing '" + closeChar + "'!");
            }

            position++;
            return result.ToString();
        }

        private TexFormula ReadScript(TexFormula formula, string value, ref int position)
        {
            if (position == value.Length)
                throw new TexParseException("illegal end, missing script!");

            SkipWhiteSpace(value, ref position);
            var ch = value[position];
            if (ch == leftGroupChar)
            {
                return Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
            }
            else
            {
                position++;
                return Parse(ch.ToString());
            }
        }

        private Atom ProcessCommand(TexFormula formula, string value, ref int position, string command)
        {
            SkipWhiteSpace(value, ref position);
            switch (command)
            {
                case "root":
                // Command is radical.

                    SkipWhiteSpace(value, ref position);
                    if (position == value.Length)
                        throw new TexParseException("illegal end!");

                    TexFormula degreeFormula = null;
                    if (value[position] == leftBracketChar)
                    {
                        // Degree of radical- is specified.
                        degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar));
                        SkipWhiteSpace(value, ref position);
                    }
                    return Radical.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
                        .GetRoot, degreeFormula == null ? null : degreeFormula.GetRoot);
                case "matrix":
                //Command is Matrix

                    SkipWhiteSpace(value, ref position);
                    if (position == value.Length)
                        throw new TexParseException("illegal end!");

                    List<List<Atom>> childs = new List<List<Atom>>();
                    MatrixAtom matrixAtom = ObjPool<MatrixAtom>.Get();
                    if (value[position] == leftGroupChar)
                    {
                        Atom parsedChild = (Parse(ReadGroup(
                                formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot);
                        childs.Add(ListPool<Atom>.Get());
                        if (parsedChild == null)
                            childs.Last().Add(SpaceAtom.Get());
                        if (parsedChild is RowAtom)
                        {
                            List<Atom> el = ((RowAtom)parsedChild).Elements;
                            childs.Last().Add(RowAtom.Get());
                            foreach (Atom a in el)
                            {
                                if (a is SymbolAtom)
                                {
                                    if (((SymbolAtom)a).Name == "ampersand")
                                        childs.Last().Add(RowAtom.Get());
                                    else if (((SymbolAtom)a).Name == "mid")
                                    {
                                        childs.Add(ListPool<Atom>.Get());
                                        childs.Last().Add(RowAtom.Get());
                                    }
                                    else
                                        ((RowAtom)childs.Last().Last()).Add(a);
                                }
                                else
                                    ((RowAtom)childs.Last().Last()).Add(a);
                            }
                        }
                        else
                            childs.Last().Add(parsedChild);
                        matrixAtom.Elements = childs;
                        return matrixAtom;
                    }
                    return null;
                case "vmatrix":
				//Command is Matrix (Vertical)

                    SkipWhiteSpace(value, ref position);
                    if (position == value.Length)
                        throw new TexParseException("illegal end!");

                    childs = new List<List<Atom>>();
                    matrixAtom = ObjPool<MatrixAtom>.Get();
                    if (value[position] == leftGroupChar)
                    {
                        Atom parsedChild = (Parse(ReadGroup(
                                formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot);
                        childs.Add(ListPool<Atom>.Get());
                        if (parsedChild == null)
                            childs.Last().Add(SpaceAtom.Get());
                        if (parsedChild is RowAtom)
                        {
                            List<Atom> el = ((RowAtom)parsedChild).Elements;
                            childs.Last().Add(ObjPool< RowAtom>.Get());
                            int vPool = 0, hPool = 0;
                            foreach (Atom a in el)
                            {
                                if (a is SymbolAtom)
                                {
                                    if (((SymbolAtom)a).Name == "ampersand")
                                    {
                                        hPool++;
                                        childs.Last().Add(RowAtom.Get());
                                        vPool = 0;
                                        for (int i = 0; i < childs.Count; i++)
                                        {
                                            if (childs[i].Count < hPool + 1)
                                                childs[i].Add(RowAtom.Get());
                                        }
                                    }
                                    else if (((SymbolAtom)a).Name == "mid")
                                    {
                                        vPool++;
                                        if (childs.Count <= vPool)
                                        {
                                            childs.Add(new List<Atom>());
                                            while (childs.Last().Count < hPool + 1)
                                                childs.Last().Add(new RowAtom());
                                        }
                                    }
                                    else
                                        ((RowAtom)childs[vPool].Last()).Add(a);
                                }
                                else
                                    ((RowAtom)childs[vPool].Last()).Add(a);
                            }
                        }
                        else
                            childs.Last().Add(parsedChild);
                        matrixAtom.Elements = childs;
                        return matrixAtom;
                    }
                    return null;
                case "text":
                    if (value[position] != leftGroupChar)
                        return null;
			
                    string val = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                    RowAtom row = RowAtom.Get();
                    for (int i = 0; i < val.Length; i++)
                    {
                        Atom charAtom;
                        if (char.IsWhiteSpace(val[i]))
                            charAtom = SpaceAtom.Get();
                        else
                            charAtom = CharAtom.Get(val[i], TEXPreference.main.defaultTypefaces[TexCharKind.Text]);
                        row.Add(charAtom);
                    }
                    return row;
                case "color":

                    string clr = null;
                    if (value[position] == leftBracketChar)
                    {
                        clr = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                        SkipWhiteSpace(value, ref position);
                        if (!(clr.Length > 0 && clr[0] == '#'))
                            clr = "#" + clr;
                    }

                    if (value[position] != leftGroupChar)
                        return null;

                    return AttrColorAtom.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
					.GetRoot, clr);
            }
            if (command.Length > 2 && command.Substring(command.Length - 3) == "not")
            {
                SkipWhiteSpace(value, ref position);
                int NotMode = 0;
                string prefix = command.Substring(0, command.Length - 3);
                if (prefix.Length > 0)
                {
                    switch (prefix[0])
                    {
                        case 'n':
                            NotMode = 1;
                            break;
                        case 'h':
                            NotMode = 2;
                            break;
                        case 'd':
                            NotMode = 3;
                            break;
                    }
                }
                if (position == value.Length)
                    throw new TexParseException("illegal end!");

                if (value[position] == leftGroupChar)
                {
                    return NegateAtom.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
						.GetRoot, NotMode);
                }
                return null;
            }
            if (command.Length > 3 && command.Substring(command.Length - 4) == "frac")
            {
                SkipWhiteSpace(value, ref position);

                int FracAlignT = 0, FracAlignB = 0;
                bool FracAlignN = true; 
                string prefix = command.Substring(0, command.Length - 4);
                if (prefix.Length > 0)
                {
                    if (prefix[0] == 'n')
                    {
                        FracAlignN = false;
                        prefix = prefix.Substring(1);
                    }
                    if (prefix.Length == 1)
                    {
                        FracAlignT = fracP(prefix[0]);
                        FracAlignB = FracAlignT;
                    }
                    else if (prefix.Length == 2)
                    {
                        FracAlignT = fracP(prefix[0]);
                        FracAlignB = fracP(prefix[1]);
                    }
                }
                if (position == value.Length)
                    throw new TexParseException("illegal end!");
                var numeratorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
                SkipWhiteSpace(value, ref position);
                var denominatorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));

                return FractionAtom.Get(numeratorFormula.GetRoot, denominatorFormula.GetRoot, FracAlignN,
                    (TexAlignment)FracAlignT, (TexAlignment)FracAlignB);
            }
				
            throw new TexParseException("Invalid command.");
        }

        int fracP(char c)
        {
            switch (c)
            {
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
            while (position < value.Length)
            {
                var ch = value[position];
                var isEnd = position == value.Length - 1;
                if (!char.IsLetter(ch) || isEnd)
                {
                    // Escape sequence has ended.
                    if (char.IsLetter(ch))
                    {
                        result.Append(ch);
                        position++;
                    }
                    break;
                }

                result.Append(ch);
                position++;
            }

            var command = result.ToString();

            SymbolAtom symbolAtom = null;
            try
            {
                symbolAtom = SymbolAtom.GetAtom(command);
            }
            catch (Exception)
            {
            }

            TexFont fontID = TEXPreference.main.GetFontByID(command);
				
            if (symbolAtom != null)
            {
                // Symbol was found.
                if (symbolAtom.GetRightType() == CharType.Accent && formula.RootAtom != null)
                {
                    //Accent is Found
                    Atom baseAtom;
                    if (formula.RootAtom is RowAtom)
                    {
                        RowAtom list = ((RowAtom)formula.RootAtom);
                        baseAtom = list.Elements.Last();
                        list.Elements.RemoveAt(list.Elements.Count - 1);
                        formula.Add(AttachScripts(formula, value, ref position, AccentedAtom.Get(baseAtom, symbolAtom)));
                    }
                    else
                    {
                        baseAtom = formula.RootAtom;
                        formula.RootAtom = null;
                        formula.Add(AttachScripts(formula, value, ref position, AccentedAtom.Get(baseAtom, symbolAtom)));
                    }
                }
                else
                    formula.Add(AttachScripts(formula, value, ref position, symbolAtom));
            }
            else if (command.Equals(extraSpace))
            {
                // Space was found.

                formula.Add(AttachScripts(formula, value, ref position, SpaceAtom.Get()));
            }
            else if (fontID != null)
            {
                if (value[position] == leftGroupChar)
                {
                    string val = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                    RowAtom row = RowAtom.Get();
                    for (int i = 0; i < val.Length; i++)
                    {
                        Atom charAtom;
                        if (char.IsWhiteSpace(val[i]))
                            charAtom = SpaceAtom.Get();
                        else if (val[i] < 0x7f)
                            charAtom = CharAtom.Get(val[i], fontID.index);
                        else
                            charAtom = UnicodeAtom.Get(fontID.index, val[i]);
                        row.Add(charAtom);
                    }
                    formula.Add(row);
                }
            }
            else if (command.Equals(extraSpaceSoft))
            {
                // Soft Space was found.

                formula.Add(AttachScripts(formula, value, ref position, SpaceAtom.Get(TexUtility.lineThickness / 3, 0, 0)));
            }
            else if (commands.Contains(command))
            {
                // Command was found.

                formula.Add(AttachScripts(formula, value, ref position, ProcessCommand(formula, value, ref position,
                            command)));
            }
            else
            {
                // Command aren't defined, use it as bolded text style
                RowAtom row = RowAtom.Get();
                for (int i = 0; i < command.Length; i++)
                {
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

            TexFormula superscriptFormula = null;
            TexFormula subscriptFormula = null;

            bool markAsBig = false;

            var ch = value[position];
            if (ch == superScriptChar)
            {
                // Attahch superscript.
                position++;
                if (value[position] == superScriptChar)
                {
                    markAsBig = true;
                    position++;
                }
                superscriptFormula = ReadScript(formula, value, ref position);

                if (position < value.Length && value[position] == subScriptChar)
                {
                    // Attach subscript also.
                    position++;
                    if (value[position] == subScriptChar)
                    {
                        markAsBig = true;
                        position++;
                    }
                    subscriptFormula = ReadScript(formula, value, ref position);
                }
            }
            else if (ch == subScriptChar)
            {
                // Add subscript.
                position++;
                if (value[position] == subScriptChar)
                {
                    markAsBig = true;
                    position++;
                }
                subscriptFormula = ReadScript(formula, value, ref position);

                if (position < value.Length && value[position] == superScriptChar)
                {
                    // Attach superscript also.
                    position++;
                    if (value[position] == superScriptChar)
                    {
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
            if (IsSymbol(character))
            {
                // Character is symbol.
                var charIdx = TEXPreference.main.charMapData.GetOrNone(character, -1);
                if (charIdx >= 0)
                    return SymbolAtom.Get(TEXPreference.main.GetChar(charIdx));
                if ((int)character >= 0x7f)
                    return UnicodeAtom.Get(TEXPreference.main.defaultTypefaces[TexCharKind.Unicode], character);
                throw new TexParseException("Unknown + " + character.ToString() + " Character");
            }
            else
            {
                // Character is alpha-numeric.
                return CharAtom.Get(character);
            }
        }

        private void SkipWhiteSpace(string value, ref int position)
        {
            while (position < value.Length && IsWhiteSpace(value[position]))
            {
                position++;
            }
        }
    }
}