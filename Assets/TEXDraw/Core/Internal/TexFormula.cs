using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
    // Represents mathematical formula that can be rendered.
    public sealed class TexFormula : IFlushable
    {
        public static TexFormula Get(IList<TexFormula> formulaList)
        {
            var formula = ObjPool<TexFormula>.Get();
            if (formulaList.Count == 1)
                formula.Add(formulaList[0]);
            else
                formula.RootAtom = RowAtom.Get(formulaList);
            return formula;
        }

        public static TexFormula Get(TexFormula formula)
        {
            var formulas = ObjPool<TexFormula>.Get();
            formulas.Add(formula);
            return formulas;
        }

        public TexFormula()
        {
        }

        public string TextStyle;

        public Atom RootAtom;

        public Atom GetRoot
        {
            get
            {
                Atom root = RootAtom;
                RootAtom = null;
                ObjPool<TexFormula>.Release(this);
                return root;
            }
        }

        public TexRenderer GetRenderer(TexStyle style, float scale)
        {
            try
            {
                TexUtility.RenderSize = scale;
                return TexRenderer.Get(CreateBox(style), scale);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Add(TexFormula formula)
        {
            if (formula.RootAtom is RowAtom)
                Add(RowAtom.Get(formula.GetRoot));
            else
                Add(formula.RootAtom);
        }

        public void Add(Atom atom)
        {
            if (RootAtom == null)
            {
                RootAtom = atom;
            }
            else
            {
                if (!(RootAtom is RowAtom))
                    RootAtom = RowAtom.Get(RootAtom);
                ((RowAtom)RootAtom).Add(atom);
            }
        }


        public Box CreateBox(TexStyle style)
        {
            if (RootAtom == null)
                return StrutBox.EmptyLine;
            else
                return RootAtom.CreateBox(style);
        }

        public void Flush()
        {
            if (RootAtom != null)
            {
                RootAtom.Flush();
                RootAtom = null;
            }
            ObjPool<TexFormula>.Release(this);
        }

        bool m_flushed;

        public bool GetFlushed()
        { 
            return m_flushed;
        }

        public void SetFlushed(bool flushed)
        {
            m_flushed = flushed;
        }
    }
}