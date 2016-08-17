using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TexDrawLib
{
    // Atom representing horizontal row of other atoms, separated by glue.
    public class RowAtom : Atom
    {
        public static RowAtom Get(IList<TexFormula> formulaList)
        {
            var atom = ObjPool<RowAtom>.Get();
            foreach (var formula in formulaList)
            {
                if (formula.RootAtom != null)
                    atom.Elements.Add(formula.RootAtom);
            }
            return atom;
        }

        public static RowAtom Get(Atom baseAtom)
        {
            var atom = ObjPool<RowAtom>.Get();
               if (baseAtom != null)
            {
                if (baseAtom is RowAtom)
                {
                    foreach (var childatom in ((RowAtom)baseAtom).Elements)
                        atom.Elements.Add(childatom);
                }
                else
                {
                    atom.Elements.Add(baseAtom);
                }
            }
            return atom;
        }

        public static RowAtom Get()
        {
            return ObjPool<RowAtom>.Get();
        }
            
            
        public List<Atom> Elements = new List<Atom>();

        public void Add(Atom atom)
        {
            if (atom != null)
                Elements.Add(atom);
        }
            
        public override Box CreateBox(TexStyle style)
        {

            // Create result box.
            var resultBox = HorizontalBox.Get();
            Atom curAtom = null, prevAtom = null, nextAtom = null;
        
            // Create and add box for each atom in row.
            for (int i = 0; i < Elements.Count; i++)
            {
                curAtom = (Elements[i]);
                nextAtom = (i < Elements.Count - 1) ? (Atom)Elements[i + 1] : null;
   
                // Create and add glue box, unless atom is first of row or previous/current atom is spaces.
                if (i != 0 && prevAtom != null && !(prevAtom is SpaceAtom) && !(curAtom is SpaceAtom))
                {
                    Box spaceBox = SpaceAtom.CreateGlueBox(prevAtom.GetRightType(), curAtom.GetLeftType(), style);
                    resultBox.Add(spaceBox);
                }
                // Create and add box for atom.
                Box curBox = curAtom.CreateBox(style);
                if (curAtom is SymbolAtom && ((SymbolAtom)curAtom).IsDelimeter)
                {
                    if (nextAtom != null && curAtom.GetRightType() == CharType.OpenDelimiter)
                    {
                        var nextBox = nextAtom.CreateBox(style);
                        curBox = DelimiterFactory.CreateBox(((SymbolAtom)curAtom).Name, nextBox.totalHeight, style);
                    }
                    else if (prevAtom != null && curAtom.GetLeftType() == CharType.CloseDelimiter)
                    {
                        var prevBox = prevAtom.CreateBox(style);
                        curBox = DelimiterFactory.CreateBox(((SymbolAtom)curAtom).Name, prevBox.totalHeight, style);
                    }
                    else
                    {
                        float height = 0;
                        if (nextAtom != null)
                        {
                            var nextBox = nextAtom.CreateBox(style);
                            height = nextBox.totalHeight;
                        }
                        if (prevAtom != null)
                        {
                            var prevBox = prevAtom.CreateBox(style);
                            height = Mathf.Max(height, prevBox.totalHeight);
                        }
                        curBox = DelimiterFactory.CreateBox(((SymbolAtom)curAtom).Name, height, style);
                    }
                    TexUtility.CentreBox(curBox, style);
                }
                resultBox.Add(curBox);
                prevAtom = curAtom;
            }

            return resultBox;
        }

        public override CharType GetLeftType()
        {
            if (Elements.Count == 0)
                return Type;
            return Elements.First().GetLeftType();
        }

        public override CharType GetRightType()
        {
            if (Elements.Count == 0)
                return Type;
            return Elements.Last().GetRightType();
        }

        public override void Flush()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Flush();
            }
            Elements.Clear();
            ObjPool<RowAtom>.Release(this);
        }
    }
}