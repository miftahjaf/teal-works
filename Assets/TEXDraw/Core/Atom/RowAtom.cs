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
            var atom = Get();
            foreach (var formula in formulaList) {
                if (formula.RootAtom != null)
                    atom.Elements.Add(formula.RootAtom);
            }
            return atom;
        }

        public static RowAtom Get(Atom baseAtom)
        {
            var atom = Get();
            if (baseAtom != null) {
                if (baseAtom is RowAtom) {
                    var els = ((RowAtom)baseAtom).Elements;
                    for (int i = 0; i < els.Count; i++) {
                        atom.Elements.Add(els[i]);
                    }
                } else
                    atom.Elements.Add(baseAtom);
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
            if (atom != null) {
                if (atom is RowAtom) {
                    if (Elements.Count > 0 && Elements[Elements.Count - 1] is SymbolAtom && ((SymbolAtom)Elements[Elements.Count - 1]).IsDelimeter) {
                        Elements.Add(atom);
                        return;
                    }
                    var els = ((RowAtom)atom).Elements;
                    for (int i = 0; i < els.Count; i++) {
                        Elements.Add(els[i]);
                    }
                    els.Clear();
                    atom.Flush();
                } else
                    Elements.Add(atom);
            }
        }

        public override Box CreateBox(TexStyle style)
        {

            // Create result box.
            var resultBox = HorizontalBox.Get();
            Atom curAtom = null, prevAtom = null;
            var resultPos = 0;
            // Create and add box for each atom in row.
            for (int i = 0; i < Elements.Count; i++) {
                curAtom = (Elements[i]);
                
                // Create and add glue box, unless atom is first of row or previous/current atom is spaces.
                if (i != 0 && prevAtom != null && !(prevAtom is SpaceAtom) && !(curAtom is SpaceAtom)) {
                    Box spaceBox = SpaceAtom.CreateGlueBox(prevAtom.GetRightType(), curAtom.GetLeftType(), style);
                    if (spaceBox != null) {
                        resultBox.Add(spaceBox);
                        resultPos++;
                    }
                }
                // Create and add box for atom.
                GenerateDelimiterBox(resultBox, ref i, ref resultPos, style);
                //resultBox.Add(curBox);
                prevAtom = curAtom;
            }
            return resultBox;
        }

        Box lastGeneratedBox;

        public Box GenerateDelimiterBox(HorizontalBox result, ref int elementPos, ref int resultPos, TexStyle style)
        {
            var curAtom = Elements[elementPos];
            var nextAtom = elementPos + 1 < Elements.Count ? Elements[elementPos + 1] : null;
            var prevAtom = elementPos > 0 ? Elements[elementPos - 1] : null;
	    	
            if (!(curAtom is SymbolAtom) || !((SymbolAtom)curAtom).IsDelimeter) {
                var box = curAtom.CreateBox(style);
                result.Add(resultPos, box);
                lastGeneratedBox = box;
                resultPos++;
                return box;
            }
	    	
            var minHeight = 0f;
            var ourPos = resultPos;
            if (nextAtom != null && curAtom.GetRightType() == CharType.OpenDelimiter) {
                elementPos++;
                var nextBox = GenerateDelimiterBox(result, ref elementPos, ref resultPos, style);
                minHeight = nextBox.totalHeight;
            } else if (prevAtom != null && curAtom.GetLeftType() == CharType.CloseDelimiter) {
                var prevBox = lastGeneratedBox;
                minHeight = prevBox.totalHeight;
            } else {
                if (prevAtom != null) {
                    var prevBox = lastGeneratedBox;
                    minHeight = prevBox.totalHeight;
                }
                if (nextAtom != null) {
                    elementPos++;
                    var nextBox = GenerateDelimiterBox(result, ref elementPos, ref resultPos, style);
                    minHeight = Mathf.Max(nextBox.totalHeight, minHeight);
                }
			  
            }
            var curBox = DelimiterFactory.CreateBox(((SymbolAtom)curAtom).Name, minHeight, style);
            TexUtility.CentreBox(curBox, style);
            result.Add(ourPos, curBox);
            if(ourPos == resultPos)
                lastGeneratedBox = curBox;
            resultPos++;
            return curBox;
        }


        public override CharType GetLeftType()
        {
            if (Elements.Count == 0)
                return Type;
            return Elements[0].GetLeftType();
        }

        public override CharType GetRightType()
        {
            if (Elements.Count == 0)
                return Type;
            return Elements[Elements.Count - 1].GetRightType();
        }

        public override void Flush()
        {
            for (int i = 0; i < Elements.Count; i++) {
                Elements[i].Flush();
            }
            Elements.Clear();
            ObjPool<RowAtom>.Release(this);
        }
    }
}