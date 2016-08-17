using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Atom (smallest unit) of TexFormula.
namespace TexDrawLib
{
    public abstract class Atom : IFlushable
    {
        public Atom()
        {
            Type = CharType.Ordinary;
        }

        public CharType Type
        {
            get;
            set;
        }

        public abstract Box CreateBox(TexStyle style);

        // Gets type of leftmost child item.
        public virtual CharType GetLeftType()
        {
            return Type;
        }

        // Gets type of leftmost child item.
        public virtual CharType GetRightType()
        {
            return Type;
        }

        public virtual void Flush()
        {
            Type = CharType.Ordinary;
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