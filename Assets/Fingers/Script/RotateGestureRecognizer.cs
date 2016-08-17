//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;

namespace DigitalRubyShared
{
    /// <summary>
    /// A rotate gesture uses two touches to call back rotation angles as the two touches twist around
    /// a central point
    /// </summary>
    public class RotateGestureRecognizer : GestureRecognizer
    {
        private float startAngle = float.MinValue;
        private float previousAngle;
        private float focusX, focusY;

        // these will go into one finger mode
        protected float anchorX = 0.0f;
        protected float anchorY = 0.0f;
        protected float offsetX = 0.0f;
        protected float offsetY = 0.0f;
        protected bool oneFinger;

        private float CurrentAngle()
        {
            float xDiff, yDiff;
            if (oneFinger)
            {
                xDiff = (CurrentTouches[0].X + offsetX) - anchorX;
                yDiff = (CurrentTouches[0].Y + offsetY) - anchorY;
            }
            else
            {
                xDiff = CurrentTouches[1].X - CurrentTouches[0].X;
                yDiff = CurrentTouches[1].Y - CurrentTouches[0].Y;
            }
            return (float)Math.Atan2(yDiff, xDiff);
        }
        
        private void UpdateAngle()
        {
            float currentAngle = CurrentAngle();
            RotationRadians = (currentAngle - startAngle);
            RotationRadiansDelta = (currentAngle - previousAngle);
            previousAngle = currentAngle;
            CalculateFocus(out focusX, out focusY);
            SetState(GestureRecognizerState.Executing);
        }

        private void CheckForStart()
        {
            float angle = CurrentAngle();            
            if (startAngle == float.MinValue)
            {
                startAngle = previousAngle = angle;
            }
            else
            {
                float angleDiff = Math.Abs(angle - startAngle);
                if (angleDiff >= AngleThreshold)
                {
                    startAngle = previousAngle = angle;
                    SetState(GestureRecognizerState.Began);
                }
            }
        }

        protected override void StateChanged()
        {
            base.StateChanged();

            if (State == GestureRecognizerState.Ended || State == GestureRecognizerState.Failed)
            {
                startAngle = float.MinValue;
                RotationRadians = 0.0f;
            }
        }

        protected override void TouchesBegan()
        {
            if (State == GestureRecognizerState.Began || State == GestureRecognizerState.Executing)
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        protected override void TouchesMoved()
        {
            if (CurrentTouches.Count == 2 || (oneFinger && CurrentTouches.Count == 1))
            {
                if (State == GestureRecognizerState.Began || State == GestureRecognizerState.Executing)
                {
                    UpdateAngle();
                }
                else
                {
                    CheckForStart();
                }
            }
            else
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        protected override void TouchesEnded()
        {
            if (State == GestureRecognizerState.Executing || State == GestureRecognizerState.Began)
            {
                SetState(GestureRecognizerState.Ended);
            }
            else
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        public RotateGestureRecognizer()
        {
            AngleThreshold = (float)Math.PI * 0.024f; // 1/40th of a half circle to start rotating
        }

        /// <summary>
        /// Focus x value (average of all touches)
        /// </summary>
        /// <value>The focus x.</value>
        public float FocusX { get { return focusX; } }

        /// <summary>
        /// Focus y value (average of all touches)
        /// </summary>
        /// <value>The focus y.</value>
        public float FocusY { get { return focusY; } }

        /// <summary>
        /// Angle threshold in radians that must be met before rotation starts
        /// </summary>
        /// <value>The angle threshold.</value>
        public float AngleThreshold { get; set; }

        /// <summary>
        /// The current rotation angle in radians
        /// </summary>
        /// <value>The rotation angle in radians</value>
        public float RotationRadians { get; set; }

        /// <summary>
        /// The change in rotation radians
        /// </summary>
        /// <value>The rotation radians delta</value>
        public float RotationRadiansDelta { get; set; }

        /// <summary>
        /// The current rotation angle in degrees
        /// </summary>
        /// <value>The rotation angle in degrees</value>
        public float RotationDegrees { get { return RotationRadians * (180.0f / (float)Math.PI); } }

        /// <summary>
        /// The change in rotation degrees
        /// </summary>
        /// <value>The rotation degrees delta</value>
        public float RotationDegreesDelta { get { return RotationRadiansDelta * (180.0f / (float)Math.PI); } }
    }
}

