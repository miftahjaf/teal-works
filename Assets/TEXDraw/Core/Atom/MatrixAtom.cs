using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TexDrawLib
{
    public class MatrixAtom : Atom
    {
       
        public MatrixAtom()
            : base()
        {
            Type = CharTypeInternal.Inner;
            Elements = ListPool<List<Atom>>.Get();
        }


        public List<List<Atom>> Elements;

        public void Add(List<Atom> atom)
        {
            if (atom != null)
                Elements.Add(atom);
        }


        public override Box CreateBox(TexStyle style)
        {
            List<List<Box>> boxes = ListPool<List<Box>>.Get();
            List<float> boxesHeight = ListPool<float>.Get();
            List<float> boxesWidth = ListPool<float>.Get();
          
            style = TexUtility.GetCrampedStyle(style);
            for (int i = 0; i < Elements.Count; i++)
            {
                boxes.Add(ListPool<Box>.Get());
                boxesHeight.Add(0);
                for (int j = 0; j < Elements[i].Count; j++)
                {
                    Box box;
                    if (Elements[i][j] != null)
                        box = (Elements[i][j].CreateBox(style));
                    else
                        box = (ObjPool<StrutBox>.Get());
                    boxes[i].Add(box);
                    if (j >= boxesWidth.Count)
                        boxesWidth.Add(box.width);
                    else
                        boxesWidth[j] = Mathf.Max(boxesWidth[j], box.width);
                    boxesHeight[i] = Mathf.Max(boxesHeight[i], box.totalHeight);
                }
            }
            var resultBox = ObjPool<VerticalBox>.Get();
            Box kern = TexUtility.GetBox(SpaceAtom.Get(TEXPreference.main.GetPreference("MatrixMargin", style), TEXPreference.main.GetPreference("MatrixMargin", style), 0), style);
            for (int i = 0; i < Elements.Count; i++)
            {
                var list = ObjPool<HorizontalBox>.Get();
                for (int j = 0; j < Elements[i].Count; j++)
                {
                    list.Add(VerticalBox.Get(HorizontalBox.Get(boxes[i][j], boxesWidth[j], TexAlignment.Center), boxesHeight[i], TexAlignment.Center));
                    if (j < Elements[i].Count - 1)
                        list.Add(kern);
                }
                resultBox.Add(list);
                if (i < Elements.Count - 1)
                    resultBox.Add(kern);
            }
            TexUtility.CentreBox(resultBox, style);
            return resultBox;
        }

        public override void Flush()
        {
            if (Elements != null)
            {
                Elements.Clear();
            }
            ObjPool<MatrixAtom>.Release(this);
        }

    }
}