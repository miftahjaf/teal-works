using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TexDrawLib
{
    /// A class for handling the math behind the final rendering process.
    /// All layouting stuff is handled here.
    public class DrawingParams
    {
        // ------------ Parameter must be filled by Component ------------------
        /// is it has defined rect bound?
        public bool hasRect;
        /// Auto Fit Mode: 0 = Off, 1 = Down Scale, 2 = Rect Size, 3 = Height Only, 4 = Scale, 5 = Best Fit
        public int autoFit;
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
        /// UV3 Filling: 0 = Off, 1 = Rectangle, 2 = Whole Text, 3 = WT Squared, 4 = Per line, 5 = Per word, 6 = Per character, 7 = PC Squared
        public int uv3Filling;

        // ------------ Parameter must be filled by Component ------------------
        public Color color;
        public int fontSize;
        //Renderer Parameter
        List<TexRenderer> m_formulas = ListPool<TexRenderer>.Get();

        public List<TexRenderer> formulas
        {
            get { return m_formulas; }
            set {
                m_formulas = value;
                PredictSize();
            }
        }

        public DrawingContext context;

        //Internal Computation Parameter
        public Vector2 size;
        public Vector2 offset;
        public Vector2 layoutSize;
        float factor = 1;

        public DrawingParams()
        {
        }

        public void Render()
        {
            if (hasRect && autoFit > 0 && (rectArea.size.x <= 0 || rectArea.size.y <= 0))
                return;
            CalculateRenderedArea();
            //Layout bug fix.
            float shift;
            offset.y += size.y;
            TexRenderer box;
            FillHelper verts = context.vertex;
            for (int i = 0; i < formulas.Count; i++) {
                int lastVerts = verts.currentVertCount;

                box = formulas[i];
                offset.y -= (box.Box.totalHeight + (i > 0 ? spaceSize : 0)) * scale * factor;
                box.Scale *= factor;
                shift = (box.Box.height - box.Box.depth) * box.Scale;
                float x = offset.x + alignment.x * (size.x - box.Scale * box.Box.width);
                float y = offset.y - shift;
                box.Render(context, x, y);
                switch (uv3Filling) {
                    case 4:
                        Rect r = new Rect(new Vector2(x, offset.y), box.RenderSize);
                        for (int j = lastVerts; j < verts.currentVertCount; j++) {
                            verts.SetUV3(inverseLerp(r, verts.m_Positions[j]), j);
                        }
                        break;
                    case 5:
                        //Toughest filling method: Per word - Not yet available
                        /*var boxes = box.Box.Children;
                        r = new Rect(offset, box.RenderSize);
                        for (int j = lastVerts; j < verts.currentVertCount; j++) {
                            verts.m_Uv2S[j] = inverseLerp(r, verts.m_Positions[j]);
                        }*/
                        break;
                }
            }
            switch (uv3Filling) {
                case 1:
                    Rect r;
                    if (hasRect)
                        r = rectArea;
                    else
                        r = new Rect(-VecScale(size, alignment), size);
                    for (int i = 0; i < verts.currentVertCount; i++) {
                        verts.SetUV3(inverseLerp(r, verts.m_Positions[i]), i);
                    }
                    break;
                case 2:
                    if (hasRect)
                        r = new Rect(-(
                                VecScale(size, (alignment)) +
                                VecScale(rectArea.size, VecNormal(alignment)) +
                                -(rectArea.center)), size);
                    else
                        r = new Rect(-VecScale(size, alignment), size);
                    for (int i = 0; i < verts.currentVertCount; i++) {
                        verts.SetUV3(inverseLerp(r, verts.m_Positions[i]), i);
                    }
                    break;
                case 3:
                    if (hasRect)
                        r = new Rect(-(
                                VecScale(size, (alignment)) +
                                VecScale(rectArea.size, VecNormal(alignment)) +
                                -(rectArea.center)), size);
                    else
                        r = new Rect(-VecScale(size, alignment), size);

                    var max = Mathf.Max(r.width, r.height);
                    var center = r.center;
                    r.size = Vector2.one * max;
                    r.center = center;
                    for (int i = 0; i < verts.currentVertCount; i++) {
                        verts.SetUV3(inverseLerp(r, verts.m_Positions[i]), i);
                    }
                    break;
                case 6:
                    for (int i = 0; i < verts.currentVertCount; i++) {
                        int l = i % 4;
                        verts.SetUV3(new Vector2(l == 0 | l == 3 ? 0 : 1, l < 2 ? 0 : 1), i);
                    }
                    break;
                case 7:
                    for (int i = 0; i < verts.currentVertCount; i += 4) {
                        Vector2 sz = verts.m_Positions[i + 2] - verts.m_Positions[i];
                        if (sz.x <= 0 || sz.y <= 0) {
                            for (int l = 0; l < 4; l++) {
                                verts.SetUV3(new Vector2(l == 0 | l == 3 ? 0 : 1, l < 2 ? 0 : 1), i);
                            }
                            continue;
                        }
                        float xMin, xMax, yMin, yMax;
                        if (sz.x > sz.y) {
                            var h = sz.y / sz.x;
                            xMin = 0;
                            xMax = 1;
                            yMin = (1 - h) / 2;  
                            yMax = 1 - yMin;
                        } else {
                            var v = sz.x / sz.y;
                            yMin = 0;
                            yMax = 1;
                            xMin = (1 - v) / 2;  
                            xMax = 1 - xMin;
                        }
                        for (int l = 0; l < 4; l++) {
                            verts.SetUV3(new Vector2(l == 0 | l == 3 ? xMin : xMax, l < 2 ? yMin : yMax), i + l);
                        }
                    }
                    break;
            }
        }

        public static Vector2 inverseLerp(Rect area, Vector2 pos)
        {
            pos.x = InverseLerp(area.xMin, area.xMax, pos.x);
            pos.y = InverseLerp(area.yMin, area.yMax, pos.y);
            return pos;
        }

        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b) {
	            return /*Mathf.Clamp01*/((value - a) / (b - a));
            }
            return 0;
        }

        public void PredictSize()
        {
            for (;;) {
				
                //Predict draw size
                size = Vector2.zero;
                for (int i = 0; i < formulas.Count; i++) {
                    Box box = formulas[i].Box;
                    size.x = Mathf.Max(size.x, box.width * scale);
                    size.y += (box.totalHeight + spaceSize) * scale;
                }
                size.y -= spaceSize * scale;
                layoutSize = size;
                factor = 1;

                //Zero means auto, let's change our rect size
                if (rectArea.width == 0)
                    rectArea.width = layoutSize.x;
                if (rectArea.height == 0)
                    rectArea.height = layoutSize.y;

                if (!hasRect)
                    return;
                //Autowrap? only do if needed
                if (autoWrap > 0 && size.x > rectArea.width) {
                    size.x = 0;
                    int i = 0;
                    List<int> spaceIdxs = ListPool<int>.Get();
                    while (i < formulas.Count) {
                        Box box = formulas[i].Box;
                        if (!(box is HorizontalBox && box.width * scale > rectArea.width)) {
                            i++;
                            continue;
                        }
                        float x = 0, xOri = 0, y = 0, lastSpaceX = 0;
                        int lastSpaceIdx = -1;
                        //Begin Per-character pooling
                        for (int j = 0; j < box.Children.Count; j++) {
                            //White line? make a mark
                            if (box.Children[j] is StrutBox && ((StrutBox)box.Children[j]).policy == StrutPolicy.BlankSpace) {
                                lastSpaceIdx = j; //last space, index
                                lastSpaceX = x; //last space, x position
                                spaceIdxs.Add(lastSpaceIdx); //record that space
                            }
                            x += box.Children[j].width;
                            xOri = x;
                            //Total length not yet to break our rect limit? continue 
                            if (x * scale <= rectArea.width)
                                continue;
                            //Now j is maximum limit character length. Now move any
                            //character before that to the new previous line

                            //Did we use word wrap? Track the last space index
                            if (autoWrap >= 2 && lastSpaceIdx >= 0) {
                                //Omit the last space by + 1
                                j = lastSpaceIdx + 1;
                                x = lastSpaceX;
                                xOri = lastSpaceX + box.Children[j - 1].width;
                                //Justify too? then expand our spaces width
                                if (autoWrap == 3) {
                                    float extraWidth = (rectArea.width / scale - (x)) / (float)(spaceIdxs.Count - 1); 
                                    for (int k = 0; k < spaceIdxs.Count; k++) {
                                        box.Children[spaceIdxs[k]].width += extraWidth;
                                    }
                                    x = rectArea.width / scale;
                                }
                            } else if (box.Children[j] is StrutBox && ((StrutBox)box.Children[j]).policy == StrutPolicy.BlankSpace) {
                                x -= box.Children[j].width;
                                j += 1;
                            } else {
                                x -= box.Children[j].width;
                                xOri = x;
                            }
                            if (j < 1) {
                                x += box.Children[j].width;
                                xOri = x;
                                continue;
                            }
                            int oriPartMark = m_formulas[i].partOfPreviousLine;
                            m_formulas[i].partOfPreviousLine = (autoWrap >= 2 && lastSpaceIdx >= 0) ? 2 : 1;
                            m_formulas.Insert(i, TexRenderer.Get(HorizontalBox.Get(
                                        box.Children.GetRangePool(0, j)), scale)); //Add to previous line,
                            box.Children.RemoveRange(0, j);
                            //Update our measurements, remember now m_formulas[i] is different with box
                            if (oriPartMark > 0)
                                m_formulas[i].partOfPreviousLine = oriPartMark;
                            box.width -= xOri;
                            y = m_formulas[i].Box.totalHeight;
                            formulas[i].Box.width = x;
                            size.x = Mathf.Max(size.x, x);
                            x = 0;
                            size.y += (spaceSize + y) * scale;
                            break;
                        }
                        spaceIdxs.Clear();
                        i++;
                    }
                    ListPool<int>.Release(spaceIdxs);
                    //Rescale again
                    if (rectArea.width == layoutSize.x)
                        rectArea.width = size.x;
                    if (rectArea.height == layoutSize.y)
                        rectArea.height = size.y;
                    layoutSize = size;// + Vector2.up * spaceSize * scale * 2;
                }
                //Autofit? then resize the prediction
                if (autoFit > 3 || autoFit == 1) {   
                    factor = autoFit == 4 ? 1000 : 1;
                    if (size.x > 0)
                        factor = Mathf.Min(factor, rectArea.width / size.x);
                    if (size.y > 0)
                        factor = Mathf.Min(factor, rectArea.height / size.y);
                    size *= factor;
                } 
                if (autoFit == 5 && factor < 1) {
                    factor = 1;
                    if (scale > 0.001f) {
                        scale *= 0.95f;
                        RevertBackList();
                        continue; //Start again
                    }
                }
                break; //Quit from never-ending loops
            }
        }

        public void RevertBackList()
        {
            for (int i = 0; i < m_formulas.Count; i++) {
                if (m_formulas[i].partOfPreviousLine > 0) {
                    Box prevBox = m_formulas[i - 1].Box;
                    //Doesn't know why we don't need this
                    //if (m_formulas[i].partOfPreviousLine == 2)
                    //    prevBox.Add(TexUtility.GetBox(SpaceAtom.Get(), TexStyle.Display));
                    if (prevBox is HorizontalBox)
                        ((HorizontalBox)prevBox).AddRange(m_formulas[i].Box);
                    else
                        prevBox.Add(m_formulas[i].Box);
                    m_formulas[i].Box = null;
                    m_formulas[i].Flush();
                    m_formulas.RemoveAt(i);
                    i--;
                }
            }
        }

        public void CalculateRenderedArea()
        {
            if (hasRect) {
                //Configure offset & alignment, Just comment out one of these things if you don't understood this ;)
                offset = -(
                    VecScale(size, (alignment)) + //Make sure the drawing pivot affected with aligment
                    VecScale(rectArea.size, VecNormal(alignment)) + //Make sure it stick on rect bound
                    -(rectArea.center)); //Make sure we calculate it in center (inside) of Rect no matter transform pivot has
            } else {
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