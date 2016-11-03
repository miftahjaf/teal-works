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

       	bool m_flushed = false;
	    public bool IsFlushed { get { return m_flushed; } set { m_flushed = value; } }
	    
    }
}