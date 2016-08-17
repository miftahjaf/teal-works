using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
    public enum CharType
    {
        Ordinary = 0,
        Geometry = 1,
        Operator = 2,
        Relation = 3,
        Arrow = 4,
        OpenDelimiter = 5,
        CloseDelimiter = 6,
        BigOperator = 7,
        Accent = 9,
    }

    public static class CharTypeInternal
    {
        static public readonly CharType Invalid = (CharType)(-1);
        static public readonly CharType Inner = (CharType)8;
    }

    public enum ExtensionType
    {
        Repeat = 0,
        Top = 1,
        Bottom = 2,
        Mid = 3
    }

    public enum TexAlignment
    {
        Center = 0,
        Left = 1,
        Right = 2,
        Top = 3,
        Bottom = 4
    }

    public enum TexStyle
    {
        Display = 0,
        //DisplayCramped = 1
        Text = 2,
        //TextCramped = 3
        Script = 4,
        //ScriptCramped = 5
        ScriptScript = 6,
        //ScriptScriptCramped = 7
    }

    public enum TexCharKind
    {
        None = -1,
        Numbers = 0,
        Capitals = 1,
        Small = 2,
        Commands = 3,
        Text = 4,
        Unicode = 5
    }

    public enum StrutPolicy
    {
        Misc = 0,
        BlankSpace = 1,
        Glue = 2,
        EmptyLine = 3,
        TabSpace = 4
    }

}