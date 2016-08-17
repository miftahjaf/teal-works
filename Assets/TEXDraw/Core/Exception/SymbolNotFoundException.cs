using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
#if UNITY_EDITOR
    [Obsolete("No longer been used since V2.4")]
    public class SymbolNotFoundException : Exception
    {
        public SymbolNotFoundException (string symbolName)
            : base (string.Format ("Cannot find symbol with the name '{0}'.", symbolName))
        {
        }
    }
#endif
}