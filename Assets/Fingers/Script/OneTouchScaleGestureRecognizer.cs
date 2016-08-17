using System;

namespace DigitalRubyShared
{
    public class OneTouchScaleGestureRecognizer : GestureRecognizer
    {
        private float startX;
        private float startY;
        private float previousDistance;

        public OneTouchScaleGestureRecognizer()
        {
            ScaleMultiplier = 1.0f;
            ZoomSpeed = 3.0f;
        }

        protected override void TouchesBegan()
        {
            base.TouchesBegan();

            if (CurrentTouches.Count != 1 || (State != GestureRecognizerState.Possible && State != GestureRecognizerState.Failed))
            {
                SetState(GestureRecognizerState.Failed);
            }
            else
            {
                SetState(GestureRecognizerState.Possible);
                startX = CurrentTouches[0].X + OffsetX;
                startY = CurrentTouches[0].Y + OffsetY;
            }
        }

        protected override void TouchesMoved()
        {
            base.TouchesMoved();

            if (CurrentTouches.Count != 1 || (State != GestureRecognizerState.Possible && State != GestureRecognizerState.Executing))
            {
                SetState(GestureRecognizerState.Failed);
                return;
            }

            float x = CurrentTouches[0].X + OffsetX;
            float y = CurrentTouches[0].Y + OffsetY;
            float distance = DistanceBetweenPoints(startX, startY, x, y);

            if (State == GestureRecognizerState.Possible)
            {                
                if (distance >= ThresholdUnits * DeviceInfo.UnitMultiplier)
                {
                    ScaleMultiplier = 1.0f;
                    previousDistance = DistanceBetweenPoints(AnchorX, AnchorY, startX, startY);
                    SetState(GestureRecognizerState.Began);
                }
                else
                {
                    return;
                }
            }

            distance = DistanceBetweenPoints(AnchorX, AnchorY, x, y);
            ScaleMultiplier = (distance / previousDistance);
            float zoomDiff = (ScaleMultiplier - 1.0f) * ZoomSpeed;
            ScaleMultiplier = 1.0f + zoomDiff;
            previousDistance = distance;
            SetState(GestureRecognizerState.Executing);
        }

        protected override void TouchesEnded()
        {
            base.TouchesEnded();

            if (State == GestureRecognizerState.Executing)
            {
                SetState(GestureRecognizerState.Ended);
            }
        }

        /// <summary>
        /// The current scale multiplier. Multiply your current scale value by this to scale.
        /// </summary>
        /// <value>The scale multiplier.</value>
        public float ScaleMultiplier { get; set; }

        /// <summary>
        /// Additional multiplier for ScaleMultiplier. This will making scaling happen slower or faster.
        /// </summary>
        /// <value>The zoom speed.</value>
        public float ZoomSpeed { get; set; }

        /// <summary>
        /// The threshold in units that the touch must move to start the gesture
        /// </summary>
        /// <value>The threshold units.</value>
        public float ThresholdUnits { get; set; }

        /// <summary>
        /// The anchor x coordinate, to use for determining scale.
        /// </summary>
        /// <value>The anchor x.</value>
        public float AnchorX { get; set; }

        /// <summary>
        /// The anchor y coordinate, to use for determining scale.
        /// </summary>
        /// <value>The anchor y.</value>
        public float AnchorY { get; set; }

        /// <summary>
        /// Amount to offset touch values by in x direction
        /// </summary>
        /// <value>The x offset.</value>
        public float OffsetX { get; set; }

        /// <summary>
        /// Amount to offset touch values by in y direction
        /// </summary>
        /// <value>The y offset</value>
        public float OffsetY { get; set; }
    }
}

