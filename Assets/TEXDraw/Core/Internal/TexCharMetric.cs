using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
    // Specifies font metrics for single character.
    public class TexCharMetric : IFlushable
    {
        public static TexCharMetric Get(TexChar Char, float Height, float Depth, float Bearing, float Italic, float Width, float Scale)
        {
            var metric = ObjPool<TexCharMetric>.Get();
            metric.ch = Char;
            metric.height = Height * Scale;
            metric.depth = Depth * Scale;
            metric.bearing = Bearing * Scale;
            metric.italic = Italic * Scale;
            metric.width = Width * Scale;
            return metric;
        }

        public void Flush()
        {
            ch = null;
            ObjPool<TexCharMetric>.Release(this);
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

        public TexChar ch;

        public float height;

        public float depth;

        public float bearing;

        public float italic;

        public float width;

        public float advanceDelta
        {
            get
            {
                return italic + bearing - width;
            }
        }

        public float totalHeight
        {
            get
            {
                return height + depth;
            }
        }
    }
}