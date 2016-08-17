using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
	// Box representing other box with delimeter and script box over or under it.
	public class OverUnderBox : Box
	{
		public static OverUnderBox Get(Box baseBox, Box delimeterBox, Box scriptBox, float kern, bool over)
		{
            var box = ObjPool<OverUnderBox>.Get();
			box.BaseBox = baseBox;
            box.DelimeterBox = delimeterBox;
            box.ScriptBox = scriptBox;
            box.Kern = kern;
            box.Over = over;

			// Calculate dimensions of box.
            box.width = baseBox.width;
            box.height = baseBox.height + (over ? delimeterBox.width : 0) +
			(over && scriptBox != null ? scriptBox.height + scriptBox.depth + kern : 0);
            box.depth = baseBox.depth + (over ? 0 : delimeterBox.width) +
			(!over && scriptBox == null ? 0 : scriptBox.height + scriptBox.depth + kern);
            return box;
		}

        public Box BaseBox;

        public Box DelimeterBox;

        public Box ScriptBox;

		// Kern between delimeter and Script.
        public float Kern;

		// True to draw delimeter and script over base; false to draw under base.
        public bool Over;

		public override void Draw (DrawingContext drawingContext, float scale, float x, float y)
		{
			BaseBox.Draw (drawingContext, scale, x, y);

			if (Over)
			{
				// Draw delimeter and script boxes over base box.
				var centerY = y - BaseBox.height - DelimeterBox.width;
//            var translationX = x + DelimeterBox.width / 2;
				//           var translationY = centerY + DelimeterBox.width / 2;

				//   drawingContext.PushTransform(new TranslateTransform(translationX * scale, translationY * scale));
				//   drawingContext.PushTransform(new RotateTransform(90));
				DelimeterBox.Draw (drawingContext, scale, -DelimeterBox.width / 2,
					-DelimeterBox.depth + DelimeterBox.width / 2);
				//   drawingContext.Pop();
				//   drawingContext.Pop();

				// Draw script box as superscript.
				if (ScriptBox != null)
					ScriptBox.Draw (drawingContext, scale, x, centerY - Kern - ScriptBox.depth);
			}
			else
			{
				// Draw delimeter and script boxes under base box.
				var centerY = y + BaseBox.depth + DelimeterBox.width;
				// var translationX = x + DelimeterBox.width / 2;
				//  var translationY = centerY - DelimeterBox.width / 2;

				//drawingContext.PushTransform(new TranslateTransform(translationX * scale, translationY * scale));
				// drawingContext.PushTransform(new RotateTransform(90));
				DelimeterBox.Draw (drawingContext, scale, -DelimeterBox.width / 2,
					-DelimeterBox.depth + DelimeterBox.width / 2);
				// drawingContext.Pop();
				// drawingContext.Pop();

				// Draw script box as subscript.
				if (ScriptBox != null)
					ScriptBox.Draw (drawingContext, scale, x, centerY + Kern + ScriptBox.height);
			}

		}
	}
}