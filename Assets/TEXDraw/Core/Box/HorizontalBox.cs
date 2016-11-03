using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
    // Box containing horizontal stack of child boxes.
    public class HorizontalBox : Box
    {
        private float childBoxesTotalwidth = 0f;
        public bool ExtensionMode = false;

        public static HorizontalBox Get(Box box, float width, TexAlignment alignment)
        {
            var Box = ObjPool<HorizontalBox>.Get();
            if (box.width >= width) {
                Box.Add(box);
                return Box;
            }
            var extrawidth = Mathf.Max(width - box.width, 0);
            if (alignment == TexAlignment.Center) {
                var strutBox = StrutBox.Get(extrawidth / 2f, 0, 0, 0);
                Box.Add(strutBox);
                Box.Add(box);
                Box.Add(strutBox);
            } else if (alignment == TexAlignment.Left) {
                Box.Add(box);
                Box.Add(StrutBox.Get(extrawidth, 0, 0, 0));
            } else if (alignment == TexAlignment.Right) {
                Box.Add(StrutBox.Get(extrawidth, 0, 0, 0));
                Box.Add(box);
            }
            return Box;
        }

        public static HorizontalBox Get(Box box)
        {
            var Box = ObjPool<HorizontalBox>.Get();
            Box.Add(box);
            return Box;
        }

        public static HorizontalBox Get()
        {
            return  ObjPool<HorizontalBox>.Get();
        }

        public static HorizontalBox Get(Box[] box)
        {
            var Box = ObjPool<HorizontalBox>.Get();
            for (int i = 0; i < box.Length; i++) {
                Box.Add(box[i]);
            }
            return Box;
        }

        //Specific for DrawingParams
        public static HorizontalBox Get(List<Box> box)
        {
            var Box = ObjPool<HorizontalBox>.Get();
            for (int i = 0; i < box.Count; i++) {
                Box.Add(box[i]);
            }
            ListPool<Box>.ReleaseNoFlush(box);
            return Box;
        }

        //Specific for DrawingParams
        public void AddRange(List<Box> box)
        {
            for (int i = 0; i < box.Count; i++) {
                Add(box[i]);
            }
            ListPool<Box>.ReleaseNoFlush(box);
        }

        public void AddRange(Box box)
        {
            for (int i = 0; i < box.Children.Count; i++) {
                Add(box.Children[i]);
            }
        }

        public override void Add(Box box)
        {
            base.Add(box);

            childBoxesTotalwidth += box.width;
            width = Mathf.Max(width, childBoxesTotalwidth);
            height = Mathf.Max((Children.Count == 0 ? float.NegativeInfinity : height), box.height - box.shift);
            depth = Mathf.Max((Children.Count == 0 ? float.NegativeInfinity : depth), box.depth + box.shift);
        }

        public override void Add(int position, Box box)
        {
            base.Add(position, box);

            childBoxesTotalwidth += box.width;
            width = Mathf.Max(width, childBoxesTotalwidth);
            height = Mathf.Max((Children.Count == 0 ? float.NegativeInfinity : height), box.height - box.shift);
            depth = Mathf.Max((Children.Count == 0 ? float.NegativeInfinity : depth), box.depth + box.shift);
        }

        public override void Draw(DrawingContext drawingContext, float scale, float x, float y)
        {
            base.Draw(drawingContext, scale, x, y);

            var curX = x;
            if (ExtensionMode) {
                float offset = TEXPreference.main.GetPreference("ExtentPadding") * 2;
                for (int i = 0; i < Children.Count; i++) {
                    Box child = Children[i];
                    var extWidth = (i == 0 || i == Children.Count - 1) ? offset : offset * 2;
                    {
                        child.width += extWidth;
                        if (child is CharBox)
                            ((CharBox)child).italic += extWidth;
                    }
                    if (i > 0)
                        curX -= offset;
                    child.Draw(drawingContext, scale, curX, y - child.shift);
                    {
                        child.width -= extWidth;
                        if (child is CharBox)
                            ((CharBox)child).italic -= extWidth;
                    }
                    if (i > 0)
                        curX += offset;
                    curX += child.width;
                }
            } else {
                for (int i = 0; i < Children.Count; i++) {
                    Box box = Children[i];
                    box.Draw(drawingContext, scale, curX, y - box.shift);
                    curX += box.width;
                }
            }
        }

        public override void Flush()
        {
            base.Flush();
            childBoxesTotalwidth = 0;
            ExtensionMode = false;
            ObjPool<HorizontalBox>.Release(this);
        }
    }
}