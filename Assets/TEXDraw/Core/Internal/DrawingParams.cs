using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TexDrawLib
{
    public class DrawingParams
    {
        // ------------ Parameter must be filled by Component ------------------
        /// is it has defined rect bound?
        public bool hasRect;
        /// is it autofit?
        public bool autoFit;
        /// Wrap Mode: 0 = No Wrap, 1 = Wrap Letter, 2 = Word Wrap, 3 = Word Wrap Justified
        public int autoWrap;
        /// Rectangle Area, if rect is defined
        public Rect rectArea;
        /// Scale of rendered Graphic
        public float scale;
        /// Alignment vector (respect to Unity's coordinate system)
        public Vector2 alignment;
        /// Additional Space Size
        public float spaceSize;
        /// Rectangle pivot position, if rect is defined
        public Vector2 pivot;

        // ------------ Parameter must be filled by Component ------------------
        public Color color;
        public int fontSize;

        //Renderer Parameter
        List<TexRenderer> m_formulas = ListPool<TexRenderer>.Get();
        public List<TexRenderer> formulas
        {
            get
            {
                return m_formulas;
            }
            set
            {
                 m_formulas = value;
            }
        }
        public DrawingContext context;

        //Internal Computation Parameter
        Vector2 size;
        Vector2 offset;
        public Vector2 layoutSize;
        float factor = 1;

        public DrawingParams()
        {
        }

        public void Render()
        {
            if (hasRect && (rectArea.size.x <= 0 || rectArea.size.y <= 0))
                return;
            CalculateRenderedArea();
            //Layout bug fix.
            float shift;
            TexRenderer box;
            for (int i = formulas.Count - 1; i >= 0; i--)
            {
                box = formulas[i];
                box.Scale *= factor;
                shift = (box.Box.height - box.Box.depth) * box.Scale;
                box.Render(context, offset.x + alignment.x *
                    (size.x - box.Scale * box.Box.width), offset.y - shift);
                offset.y += (box.Box.totalHeight + spaceSize) * scale * factor;
            }
            factor = 1;
        }

        public void CalculateRenderedArea()
        {
            //Predict draw size
            size = Vector2.zero;
            for (int i = 0; i < formulas.Count; i++)
            {
                size.x = Mathf.Max(size.x, formulas[i].Box.width * scale);
                size.y += (formulas[i].Box.totalHeight + spaceSize) * scale;
            }
            size.y -= spaceSize * scale;
            layoutSize = size + Vector2.up * spaceSize * scale * 2;
            if (hasRect)
            {
                //Autowrap? only do if needed
                if (autoWrap > 0 && size.x > rectArea.width)
                {
                    size.x = 0;
                    for (int i = 0; i < formulas.Count; i++)
                    {
                        Box box = formulas[i].Box;
                        if (!(box is HorizontalBox && box.width * scale > rectArea.width))
                            continue;
                        float x = 0, y = 0, lastSpaceX = 0;
                        int lastSpaceIdx = -1;
                        List<int> spaceIdxs = ListPool<int>.Get();
                        for (int j = 0; j < box.Children.Count; j++)
                        {
                            if (box.Children[j] is StrutBox && ((StrutBox)box.Children[j]).policy == StrutPolicy.BlankSpace)
                            {
                                lastSpaceIdx = j;
                                lastSpaceX = x;
                                spaceIdxs.Add(lastSpaceIdx);
                            }
                            x += box.Children[j].width;
                            if (x * scale <= rectArea.width)
                                continue;
                            
                            if (autoWrap >= 2 && lastSpaceIdx >= 0)
                            {
                                //Omit the space by + 1
                                j = lastSpaceIdx + 1;
                                x = lastSpaceX;
                                if(autoWrap == 3)
                                {
                                    float extraWidth = (rectArea.width / scale - (x)) / (float)(spaceIdxs.Count - 1); 
                                    foreach (var idx in spaceIdxs)
                                    {
                                        box.Children[idx].width += extraWidth;
                                    }
                                    x = rectArea.width / scale;
                                }
                            }
                            else if(box.Children[j] is StrutBox && ((StrutBox)box.Children[j]).policy == StrutPolicy.BlankSpace)
                            {
                                x -= box.Children[j].width;
                                j += 1;
                            }
                            else
                                x -= box.Children[j].width;
                            j = Mathf.Max(1, j);
                            y = box.totalHeight;
                            int rem = box.Children.Count - j;
                            m_formulas.Insert(i + 1, TexRenderer.Get(HorizontalBox.Get(
                                        box.Children.GetRange(j, rem)), scale));
                            box.Children.RemoveRange(j, rem);
                            box.width = x;
                            size.x = Mathf.Max(size.x, x);
                            x = 0;
                            size.y += (spaceSize + y) * scale;
                            break;
                        }
                        ListPool<int>.Release(spaceIdxs);
                    }
                    layoutSize = size;// + Vector2.up * spaceSize * scale * 2;
                }
                //Autofit? then resize the prediction
                if (autoFit)
                {   
                    factor = 1;
                    if (size.x > 0)
                        factor = Mathf.Min(factor, rectArea.width / size.x);
                    if (size.y > 0)
                        factor = Mathf.Min(factor, rectArea.height / size.y);
                    size *= factor;
                }
                //Configure offset & alignment, Just comment out one of these things if you don't understood this ;)
                offset = -(
                    VecScale(size, (alignment)) + //Make sure the drawing pivot affected with aligment
                    VecScale(rectArea.size, VecNormal(alignment)) + //Make sure it stick on rect bound
                    -(rectArea.center)); //Make sure we calculate it in center (inside) of Rect no matter transform pivot has
            }
            else
            {
                //Miss lot of features
                offset = -VecScale(size, alignment);
            }
        }

        Vector2 VecScale(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        Vector2 VecNormal(Vector2 a)
        {
            return new Vector2(-a.x + 0.5f, -a.y + 0.5f);
        }

    }
}