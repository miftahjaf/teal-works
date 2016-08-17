using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DigitalRubyShared
{
    public class DemoScript : MonoBehaviour
    {
        public FingersScript FingerScript;
        
        private TapGestureRecognizer tapGesture;
        private TapGestureRecognizer doubleTapGesture;
        
        private GestureTouch FirstTouch(ICollection<GestureTouch> touches)
        {
            foreach (GestureTouch t in touches)
            {
                return t;
            }
            return new GestureTouch();
        }

        private void DebugText(string text, params object[] format)
        {
            Debug.Log(string.Format(text, format));
        }

        


        private void TapGestureCallback(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                GestureTouch t = FirstTouch(touches);
                if (t.IsValid())
                {
                    DebugText("Tapped at {0}, {1}", t.X, t.Y);
                }
            }
        }

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.Updated += TapGestureCallback;
            FingerScript.AddGesture(tapGesture);
        }

        private void DoubleTapGestureCallback(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                GestureTouch t = FirstTouch(touches);
                if (t.IsValid())
                {
                    DebugText("Double tapped at {0}, {1}", t.X, t.Y);
                }
            }
        }

        private void CreateDoubleTapGesture()
        {
            doubleTapGesture = new TapGestureRecognizer();
            doubleTapGesture.NumberOfTapsRequired = 2;
            doubleTapGesture.Updated += DoubleTapGestureCallback;
            FingerScript.AddGesture(doubleTapGesture);
        }

        private void Start()
        {
            CreateTapGesture();
            CreateDoubleTapGesture();

            // single tap gesture requires that the double tap gesture fail
            tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
//                ReloadDemoScene();
            }
        }
    }
}
