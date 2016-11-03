using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    //Interface that has to be implemented for each TEXDraw comonent
    public interface ITEXDraw
    {

        string text { get; set; }

        Color color { get; set; }

        float size { get; set; }

        Wrapping autoWrap { get; set; }

        Fitting autoFit { get; set; }

        Filling autoFill { get; set; }

        void SetTextDirty();

        void SetTextDirty(bool forceRedraw);

        DrawingContext drawingContext { get; }

    }
}