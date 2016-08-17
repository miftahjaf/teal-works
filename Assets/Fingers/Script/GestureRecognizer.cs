//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;
using System.Collections.Generic;

namespace DigitalRubyShared
{
    /// <summary>
    /// Gesture recognizer states
    /// </summary>
    public enum GestureRecognizerState
    {
        /// <summary>
        /// Gesture is possible
        /// </summary>
        Possible,

        /// <summary>
        /// Gesture has started
        /// </summary>
        Began,

        /// <summary>
        /// Gesture is executing
        /// </summary>
        Executing,

        /// <summary>
        /// Gesture has ended
        /// </summary>
        Ended,

        /// <summary>
        /// End is pending, if the dependant gesture fails
        /// </summary>
        EndPending,

        /// <summary>
        /// Gesture has failed
        /// </summary>
        Failed
    }

    /// <summary>
    /// Contains a touch event. Gesture touch references are not guaranteed to be the same even if the
    /// platform specific id is the same.
    /// </summary>
    public struct GestureTouch : IDisposable, IComparable<GestureTouch>
    {
        /// <summary>
        /// Invalid patform specific id
        /// </summary>
        public const int PlatformSpecificIdInvalid = -1;

        private int id;
        private float x;
        private float y;
        private float previousX;
        private float previousY;
        private float pressure;
        private float rawX;
        private float rawY;

        /// <summary>
        /// Constructor
        /// </summary>
        public GestureTouch(int platformSpecificId, float x, float y, float previousX, float previousY, float pressure)
        {
            this.id = platformSpecificId;
            this.x = x;
            this.y = y;
            this.previousX = previousX;
            this.previousY = previousY;
            this.pressure = pressure;
            rawX = float.NaN;
            rawY = float.NaN;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GestureTouch(int platformSpecificId, float x, float y, float previousX, float previousY, float pressure, float rawX, float rawY)
        {
            this.id = platformSpecificId;
            this.x = x;
            this.y = y;
            this.previousX = previousX;
            this.previousY = previousY;
            this.pressure = pressure;
            this.rawX = rawX;
            this.rawY = rawY;
        }

        /// <summary>
        /// Determines whether this instance is valid
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            return (Id != PlatformSpecificIdInvalid);
        }

        public int CompareTo(GestureTouch other)
        {
            return this.id.CompareTo(other.id);
        }

        /// <summary>
        /// Returns a hash code for this GestureTouch
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return Id;
        }

        /// <summary>
        /// Checks if this GestureTouch equals another GestureTouch
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal to obj, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is GestureTouch)
            {
                return ((GestureTouch)obj).Id == Id;
            }
            return false;
        }

        /// <summary>
        /// Invalidates this gesture touch object
        /// </summary>
        public void Dispose()
        {
            id = PlatformSpecificIdInvalid;
        }

        /// <summary>
        /// Unique id for the touch
        /// </summary>
        /// <value>The platform specific identifier</value>
        public int Id { get { return id; } }

        /// <summary>
        /// X value
        /// </summary>
        /// <value>The x value</value>
        public float X { get { return x; } }

        /// <summary>
        /// Y value
        /// </summary>
        /// <value>The y value</value>
        public float Y { get { return y; } }

        /// <summary>
        /// Previous x value
        /// </summary>
        /// <value>The previous x value</value>
        public float PreviousX { get { return previousX; } }

        /// <summary>
        /// Previous Y value
        /// </summary>
        /// <value>The previous y value</value>
        public float PreviousY { get { return previousY; } }

        /// <summary>
        /// Screen x coordinate, NAN if unknown
        /// </summary>
        /// <value>The screen x coordinate.</value>
        public float ScreenX { get { return rawX; } }

        /// <summary>
        /// Screen y coordinate, NAN if unknown
        /// </summary>
        /// <value>The screen y coordinate.</value>
        public float ScreenY { get { return rawY; } }

        /// <summary>
        /// Pressure, 0 if unknown
        /// </summary>
        /// <value>The pressure of the touch</value>
        public float Pressure { get { return pressure; } }

        /// <summary>
        /// Change in x value
        /// </summary>
        /// <value>The delta x</value>
        public float DeltaX { get { return x - previousX; } }

        /// <summary>
        /// Change in y value
        /// </summary>
        /// <value>The delta y</value>
        public float DeltaY { get { return y - previousY; } }
    }

    /// <summary>
    /// Gesture recognizer updated
    /// </summary>
    public delegate void GestureRecognizerUpdated(GestureRecognizer gesture, ICollection<GestureTouch> touches);

    public class GestureVelocityTracker
    {
        private struct VelocityHistory
        {
            public float VelocityX;
            public float VelocityY;
            public float Angle;
            public float Seconds;
        }

        private const int maxHistory = 4;

        private readonly List<VelocityHistory> history = new List<VelocityHistory>();
        private readonly System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        private float previousX;
        private float previousY;

        private bool AddItem(float velocityX, float velocityY, float elapsed)
        {
            bool significantVelocityChange = false;

            VelocityHistory item = new VelocityHistory
            {
                VelocityX = velocityX,
                VelocityY = velocityY,
                Seconds = elapsed,
                Angle = (float)Math.Atan2(velocityY, velocityX)
            };
            if (history.Count != 0 && (velocityX != 0.0f || velocityY != 0.0f))
            {
                // detect change in velocity
                VelocityHistory prev = history[history.Count - 1];
                float diff = Math.Abs(item.Angle - prev.Angle);
                if (diff > Math.PI)
                {
                    diff = (2.0f * (float)Math.PI) - diff;
                }
                if (diff > Math.PI * 0.1f)
                {
                    significantVelocityChange = true;
                    history.Clear();
                }
            }
            history.Add(item);

            if (history.Count > maxHistory)
            {
                history.RemoveAt(0);
            }

            float totalSeconds = 0.0f;
            VelocityX = VelocityY = 0.0f;
            foreach (VelocityHistory h in history)
            {
                totalSeconds += h.Seconds;
            }
            foreach (VelocityHistory h in history)
            {
                float weight = h.Seconds / totalSeconds;
                VelocityX += (h.VelocityX * weight);
                VelocityY += (h.VelocityY * weight);
            }

            timer.Reset();
            timer.Start();

            return significantVelocityChange;
        }

        public void Reset()
        {
            timer.Reset();
            VelocityX = VelocityY = 0.0f;
            history.Clear();
        }

        public void Restart()
        {
            Restart(float.MinValue, float.MinValue);
        }

        public void Restart(float previousX, float previousY)
        {
            this.previousX = previousX;
            this.previousY = previousY;
            Reset();
            timer.Start();
        }

        public bool Update(float x, float y)
        {
            float elapsed = ElapsedSeconds;
            float px = previousX;
            float py = previousY;
            float velocityX = (x - px) / elapsed;
            float velocityY = (y - py) / elapsed;
            previousX = x;
            previousY = y;

            return AddItem(velocityX, velocityY, elapsed);
        }

        public float ElapsedSeconds { get { return (float)timer.Elapsed.TotalSeconds; } }
        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }
    }

    /// <summary>
    /// A gesture recognizer allows handling gestures as well as ensuring that different gestures
    /// do not execute at the same time. Platform specific code is required to create GestureTouch
    /// sets and pass them to the appropriate gesture recognizer(s). Creating extension methods
    /// on the GestureRecognizer class is a good way.
    /// </summary>
    public class GestureRecognizer : IDisposable
    {
        private static readonly GestureRecognizer allGesturesReference = new GestureRecognizer();
        private GestureRecognizerState state = GestureRecognizerState.Possible;
        private readonly List<GestureTouch> currentTouches = new List<GestureTouch>();
        private GestureRecognizer requireGestureRecognizerToFail;
        private readonly HashSet<GestureRecognizer> failGestures = new HashSet<GestureRecognizer>();
        private readonly List<GestureRecognizer> simultaneousGestures = new List<GestureRecognizer>();
        private readonly GestureVelocityTracker velocityTracker = new GestureVelocityTracker();

        public static readonly HashSet<GestureRecognizer> ActiveGestures = new HashSet<GestureRecognizer>();

        private void FailGestureNow()
        {
            state = GestureRecognizerState.Failed;
            if (ActiveGestures.Remove(this))
            {
                System.Diagnostics.Debug.WriteLine("Removed " + this.ToString() + " gesture from active list");
            }
            StateChanged();
            foreach (GestureRecognizer r in failGestures)
            {
                if (r.state == GestureRecognizerState.EndPending)
                {
                    r.SetState(GestureRecognizerState.Ended);
                }
            }
        }

        private void SetCurrentTouches(IEnumerable<GestureTouch> touches)
        {
            currentTouches.Clear();
            currentTouches.AddRange(touches);
            currentTouches.Sort();
        }

        private void RemoveFromActiveGestures()
        {
            if (ActiveGestures.Remove(this))
            {
                System.Diagnostics.Debug.WriteLine("Removed " + this.ToString() + " gesture from active list");
            }
        }

        private

#if PCL || PORTABLE || HAS_TASKS

        async

#endif

        static void RunActionAfterDelayInternal(float seconds, Action action)
        {

#if PCL || PORTABLE || HAS_TASKS

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(seconds));

            action();

#else

            MainThreadCallback(seconds, action);

#endif

        }

        protected GestureVelocityTracker VelocityTracker { get { return velocityTracker; } }

        protected IList<GestureTouch> CurrentTouches { get { return currentTouches; } }

        protected void CalculateFocus(out float x, out float y)
        {
            x = 0.0f;
            y = 0.0f;

            foreach (GestureTouch t in CurrentTouches)
            {
                x += t.X;
                y += t.Y;
            }

            x /= (float)CurrentTouches.Count;
            y /= (float)CurrentTouches.Count;
        }

        /// <summary>
        /// Called when state changes
        /// </summary>
        protected virtual void StateChanged()
        {
            if (Updated != null)
            {
                Updated(this, currentTouches);
            }

            if (failGestures.Count != 0 && (state == GestureRecognizerState.Began || state == GestureRecognizerState.Executing ||
                state == GestureRecognizerState.Ended))
            {
                foreach (GestureRecognizer r in failGestures)
                {
                    r.FailGestureNow();
                }
            }
        }

        /// <summary>
        /// Sets the state of the gesture. Continous gestures should set the executing state every time they change.
        /// </summary>
        /// <param name="value">True if success, false if the gesture was forced to fail or the state is pending a require gesture recognizer to fail state change</param>
        protected bool SetState(GestureRecognizerState value)
        {
            if (value == GestureRecognizerState.Failed)
            {
                FailGestureNow();
                return true;
            }
            // if we are trying to execute from a non-executing state and there are gestures already executing,
            // we need to make sure we are allowed to execute simultaneously
            else if (ActiveGestures.Count != 0 &&
            (
                value == GestureRecognizerState.Began ||
                value == GestureRecognizerState.Executing ||
                value == GestureRecognizerState.Ended
            ) && state != GestureRecognizerState.Began && state != GestureRecognizerState.Executing)
            {
                // check all the active gestures and if any are not allowed to simultaneously
                // execute with this gesture, fail this gesture immediately
                foreach (GestureRecognizer r in ActiveGestures)
                {
                    if (r != this &&
                        !simultaneousGestures.Contains(r) &&
                        !r.simultaneousGestures.Contains(this) &&
                        !simultaneousGestures.Contains(allGesturesReference) &&
                        !r.simultaneousGestures.Contains(allGesturesReference))
                    {
                        FailGestureNow();
                        return false;
                    }
                }
            }

            if (requireGestureRecognizerToFail != null && value == GestureRecognizerState.Ended &&
                requireGestureRecognizerToFail.state != GestureRecognizerState.Failed)
            {
                // the other gesture will end the state when it fails, or fail this gesture if it executes
                state = GestureRecognizerState.EndPending;
                return false;
            }
            else
            {
                state = value;
                if (state == GestureRecognizerState.Began || state == GestureRecognizerState.Executing)
                {
                    ActiveGestures.Add(this);
                }
                else if (state == GestureRecognizerState.Ended)
                {
                    // a tap gesture for example needs to be active for one loop so that other tap gestures do not fire at the same time
                    ActiveGestures.Add(this);
                    RunActionAfterDelay(0.01f, RemoveFromActiveGestures);
                }
                else
                {
                    RemoveFromActiveGestures();
                }
                StateChanged();
            }

            return true;
        }

        /// <summary>
        /// Call with the touches that began, child class should override
        /// </summary>
        /// <param name="touches">Touches that began</param>
        protected virtual void TouchesBegan()
        {

        }

        /// <summary>
        /// Call with the touches that moved, child class should override
        /// </summary>
        /// <param name="touches">Touches that moved</param>
        protected virtual void TouchesMoved()
        {

        }

        /// <summary>
        /// Call with the touches that ended, child class should override
        /// </summary>
        /// <param name="touches">Touches that ended</param>
        protected virtual void TouchesEnded()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GestureRecognizer()
        {
            state = GestureRecognizerState.Possible;
            PlatformSpecificViewScale = 1;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~GestureRecognizer()
        {
            Dispose();
        }

        /// <summary>
        /// Reset all internal state  for the gesture recognizer
        /// </summary>
        public virtual void Reset()
        {
            currentTouches.Clear();
            ActiveGestures.Remove(this);
            SetState(GestureRecognizerState.Possible);
        }

        /// <summary>
        /// Call with the touches that began
        /// </summary>
        /// <param name="touches">Touches that began</param>
        public void ProcessTouchesBegan(ICollection<GestureTouch> touches)
        {
            if (touches != null && touches.Count != 0)
            {
                SetCurrentTouches(touches);
                TouchesBegan();
            }
        }

        /// <summary>
        /// Call with the touches that moved
        /// </summary>
        /// <param name="touches">Touches that moved</param>
        public void ProcessTouchesMoved(ICollection<GestureTouch> touches)
        {
            if (touches != null && touches.Count != 0)
            {
                SetCurrentTouches(touches);
                TouchesMoved();
            }
        }

        /// <summary>
        /// Call with the touches that ended
        /// </summary>
        /// <param name="touches">Touches that ended</param>
        public void ProcessTouchesEnded(ICollection<GestureTouch> touches)
        {
            if (touches != null && touches.Count != 0)
            {
                SetCurrentTouches(touches);
                TouchesEnded();
            }
        }

        public void ProcessTouchesCancelled(ICollection<GestureTouch> touches)
        {
            if (touches != null && touches.Count != 0)
            {
                foreach (GestureTouch t in touches)
                {
                    if (currentTouches.Contains(t))
                    {
                        SetState(GestureRecognizerState.Failed);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether two points are within a specified distance, accounting for the scaling in the platform specific view scale property
        /// </summary>
        /// <returns>True if within distance false otherwise</returns>
        /// <param name="x1">The first x value.</param>
        /// <param name="y1">The first y value.</param>
        /// <param name="x2">The second x value.</param>
        /// <param name="y2">The second y value.</param>
        /// <param name="d">Distance</param>
        public bool PointsAreWithinDistance(float x1, float y1, float x2, float y2, float d)
        {
            return ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) * PlatformSpecificViewScale) < (d * d);
        }

        /// <summary>
        /// Gets the distance between two points, accounting for the platform specific view scale
        /// </summary>
        /// <returns>The distance between the two points.</returns>
        /// <param name="x1">The first x value.</param>
        /// <param name="y1">The first y value.</param>
        /// <param name="x2">The second x value.</param>
        /// <param name="y2">The second y value.</param>
        public float DistanceBetweenPoints(float x1, float y1, float x2, float y2)
        {
            double a = (double)(x2 - x1);
            double b = (double)(y2 - y1);

            return (float)Math.Sqrt(a * a + b * b) * PlatformSpecificViewScale;
        }

        /// <summary>
        /// Dispose of the gesture and ensure it is removed from the global list of active gestures
        /// </summary>
        public virtual void Dispose()
        {
            ActiveGestures.Remove(this);
            foreach (GestureRecognizer r in simultaneousGestures.ToArray())
            {
                DisallowSimultaneousExecution(r);
            }
            foreach (GestureRecognizer r in failGestures)
            {
                if (r.requireGestureRecognizerToFail == this)
                {
                    r.requireGestureRecognizerToFail = null;
                }
            }
        }

        /// <summary>
        /// Allows the simultaneous execution with other gesture. This links both gestures so this method
        /// only needs to be called once on one of the gestures.
        /// Pass null to allow simultaneous execution with all gestures.
        /// </summary>
        /// <param name="other">Gesture to execute simultaneously with</param>
        public void AllowSimultaneousExecution(GestureRecognizer other)
        {
            other = (other ?? allGesturesReference);
            simultaneousGestures.Add(other);
            if (other != allGesturesReference)
            {
                other.simultaneousGestures.Add(this);
            }
        }

        /// <summary>
        /// Disallows the simultaneous execution with other gesture. This unlinks both gestures so this method
        /// only needs to be called once on one of the gestures.
        /// By default, gesures are not allowed to execute simultaneously, so you only need to call this method
        /// if you previously allowed the gestures to execute simultaneously.
        /// Pass null to disallow simulatneous execution with all gestures (i.e. you previously called
        /// AllowSimultaneousExecution with a null value.
        /// </summary>
        /// <param name="other">Gesture to no longer execute simultaneously with</param>
        public void DisallowSimultaneousExecution(GestureRecognizer other)
        {
            other = (other ?? allGesturesReference);
            simultaneousGestures.Remove(other);
            if (other != allGesturesReference)
            {
                other.simultaneousGestures.Remove(this);
            }
        }

        public static void RunActionAfterDelay(float seconds, Action action)
        {
            RunActionAfterDelayInternal(seconds, action);
        }

        public static int NumberOfGesturesInProgress()
        {
            return ActiveGestures.Count;
        }

        /// <summary>
        /// Get the current gesture recognizer state
        /// </summary>
        /// <value>Gesture recognizer state</value>
        public GestureRecognizerState State { get { return state; } }

        /// <summary>
        /// Executes when the gesture changes
        /// </summary>
        public event GestureRecognizerUpdated Updated;

        /// <summary>
        /// Velocity x in pixels (not all gestures support velocity calculations)
        /// </summary>
        /// <value>The velocity x value in pixels</value>
        public float VelocityX { get { return velocityTracker.VelocityX; } }

        /// <summary>
        /// Velocity y in pixels (not all gestures support velocity calculations)
        /// </summary>
        /// <value>The velocity y value in pixels</value>
        public float VelocityY { get { return velocityTracker.VelocityY; } }

        /// <summary>
        /// A platform specific view object that this gesture can execute in, null if none
        /// </summary>
        /// <value>The platform specific view this gesture can execute in</value>
        public object PlatformSpecificView { get; set; }

        /// <summary>
        /// Gets the scale of the platform specific view. Default is 1.
        /// </summary>
        /// <value>The platform specific view scale</value>
        public float PlatformSpecificViewScale { get; set; }

        /// <summary>
        /// If this gesture reaches the EndPending state and the specified gesture fails,
        /// this gesture will end. If the specified gesture begins, executes or ends,
        /// then this gesture will immediately fail.
        /// </summary>
        public GestureRecognizer RequireGestureRecognizerToFail
        {
            get { return requireGestureRecognizerToFail; }
            set
            {
                if (value != requireGestureRecognizerToFail)
                {
                    if (requireGestureRecognizerToFail != null)
                    {
                        requireGestureRecognizerToFail.failGestures.Remove(this);
                    }
                    requireGestureRecognizerToFail = value;
                    if (requireGestureRecognizerToFail != null)
                    {
                        requireGestureRecognizerToFail.failGestures.Add(this);
                    }
                }
            }
        }

#if !PCL && !HAS_TASKS

        public delegate void CallbackMainThreadDelegate(float delay, Action callback);

        public static CallbackMainThreadDelegate MainThreadCallback;

#endif

    }
}

