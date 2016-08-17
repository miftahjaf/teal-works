//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DigitalRubyShared
{
    public class FingersScript : MonoBehaviour
    {
        [Tooltip("True to treat the mouse as a finger, false otherwise. Left, middle and right mouse buttons can be used as individual fingers and will all have the same location.")]
        public bool TreatMousePointerAsFinger = true;

        [Tooltip("Objects that should pass gestures through. By default, any game object with a UI element other than Text will capture gestures. You can add " +
            "additional objects into this array that will allow gestures to pass through, regardless of what UI elements or scripts are attached. " +
            "See the ShouldCaptureGesture method if you need to change the default behavior.")]
        public List<GameObject> PassThroughObjects;

        [Tooltip("The default DPI to use if the DPI cannot be determined")]
        public int DefaultDPI = 200;

        private const int mousePointerId1 = int.MaxValue - 2;
        private const int mousePointerId2 = int.MaxValue - 3;
        private const int mousePointerId3 = int.MaxValue - 4;

        private readonly List<KeyValuePair<float, float>> mousePrev = new List<KeyValuePair<float, float>>();
        private readonly List<GestureRecognizer> gestures = new List<GestureRecognizer>();
        private readonly List<GestureTouch> touchesBegan = new List<GestureTouch>();
        private readonly List<GestureTouch> touchesMoved = new List<GestureTouch>();
        private readonly List<GestureTouch> touchesEnded = new List<GestureTouch>();
        private readonly Dictionary<int, GameObject> gameObjectsForTouch = new Dictionary<int, GameObject>();
        private readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult>();
        private readonly List<GestureTouch> filteredTouches = new List<GestureTouch>();
        private readonly List<GestureTouch> touches = new List<GestureTouch>();
        private float rotateAngle;
        private float pinchScale = 1.0f;
        private GestureTouch rotatePinch1;
        private GestureTouch rotatePinch2;

        private IEnumerator MainThreadCallback(float delay, System.Action action)
        {
            if (action != null)
            {
                System.Diagnostics.Stopwatch w = System.Diagnostics.Stopwatch.StartNew();
                while (w.Elapsed.TotalSeconds < delay)
                {
                    yield return null;
                }
                action();
            }
        }

        private bool ShouldCaptureGesture(GameObject o)
        {
            // determine if the game object has only scripts or UI elements that should pass gestures through
            return (o.GetComponent<Text>() == null);
        }

        private GameObject GameObjectForTouch(int pointerId, float x, float y)
        {
            // Find a game object for a touch id
            if (EventSystem.current == null)
            {
                return null;
            }

            captureRaycastResults.Clear();
            PointerEventData p = new PointerEventData(EventSystem.current);
            p.position = new Vector2(x, y);
            p.clickCount = 1;
            p.dragging = false;
            EventSystem.current.RaycastAll(p, captureRaycastResults);

            if (captureRaycastResults.Count == 0)
            {
                return null;
            }

            // determine what game object, if any should capture the gesture
            foreach (RaycastResult r in captureRaycastResults)
            {
                if (r.gameObject != null && ShouldCaptureGesture(r.gameObject) &&
                    (PassThroughObjects == null || !PassThroughObjects.Contains(r.gameObject)))
                {
//                    return r.gameObject;
					return null;
                }
            }

            return null;
        }

        private void GestureTouchFromTouch(ref Touch t, out GestureTouch g)
        {
            // convert Unity touch to Gesture touch
            g = new GestureTouch(t.fingerId, t.position.x, t.position.y, t.position.x - t.deltaPosition.x, t.position.y - t.deltaPosition.y, 0.0f);
        }

        private void ProcessTouch(ref Touch t)
        {
            // process the touch, putting it in the appropriate list for it's state
            GestureTouch g;
            GestureTouchFromTouch(ref t, out g);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    touchesBegan.Add(g);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    touchesMoved.Add(g);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    touchesEnded.Add(g);
                    break;
            }
        }

        private void AddMouseTouch(int index, int id, float x, float y)
        {
            // add a touch from the mouse
            float prevX = mousePrev[index].Key;
            float prevY = mousePrev[index].Value;
            prevX = (prevX == float.MinValue ? x : prevX);
            prevY = (prevY == float.MinValue ? y : prevY);

            GestureTouch g = new GestureTouch(mousePointerId1, x, y, prevX, prevY, 0.0f);
            if (Input.GetMouseButtonDown(index))
            {
                mousePrev[index] = new KeyValuePair<float, float>(x, y);
                touchesBegan.Add(g);
            }
            else if (Input.GetMouseButton(index))
            {
                mousePrev[index] = new KeyValuePair<float, float>(x, y);
                touchesMoved.Add(g);
            }
            else if (Input.GetMouseButtonUp(index))
            {
                mousePrev[index] = new KeyValuePair<float, float>(float.MinValue, float.MinValue);
                touchesEnded.Add(g);
            }
        }

        private void ProcessTouches()
        {
            // process each touch in the Unity list of touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);

                // string d = string.Format ("Touch: {0} {1}", t.position, t.phase);
                // Debug.Log (d);

                ProcessTouch(ref t);
            }
        }

        private void RotateAroundPoint(ref float rotX, ref float rotY, float anchorX, float anchorY, float angleRadians)
        {
            // rotate around a point in 2D space
            float cosTheta = Mathf.Cos(angleRadians);
            float sinTheta = Mathf.Sin(angleRadians);
            float x = rotX - anchorX;
            float y = rotY - anchorY;
            rotX = ((cosTheta * x) - (sinTheta * y)) + anchorX;
            rotY = ((sinTheta * x) + (cosTheta * y)) + anchorY;
        }

        private void ProcessMouseButtons()
        {
            // if not using the mouse, bail
            if (!Input.mousePresent || !TreatMousePointerAsFinger)
            {
                return;
            }

            // add touches based on each mouse button
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            AddMouseTouch(0, mousePointerId1, x, y);
            AddMouseTouch(1, mousePointerId1, x, y);
            AddMouseTouch(2, mousePointerId1, x, y);
        }

        private void ProcessMouseWheel()
        {
            // if the mouse is not setup or the user doesn't want the mouse treated as touches, return right away
            if (!Input.mousePresent || !TreatMousePointerAsFinger)
            {
                return;
            }

            // the mouse wheel will act as a rotate and pinch / zoom
            const float threshold = 100.0f;
            const float deltaModifier = 0.025f;
            Vector2 delta = Input.mouseScrollDelta;
            float scrollDelta = (delta.y == 0.0f ? delta.x : delta.y) * deltaModifier;

            // add type 1 = moved, 2 = begin, 3 = ended, 4 = none
            int addType1 = 4;
            int addType2 = 4;

            // left or right control initial down means begin
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                addType1 = 2;
            }
            // left or right control still down means move
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                pinchScale += scrollDelta;
                addType1 = 1;
            }
            // left or right control initial up means end
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                addType1 = 3;
            }

            // left or right shift initial down means begin
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                addType2 = 2;
            }
            // left or right shift still down means move
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                rotateAngle += scrollDelta;
                addType2 = 1;
            }
            // left or right shift initial up means end
            else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                addType2 = 3;
            }

            // use the minimum add type so that moves are preferred over begins and begins are preferred over ends
            int addType = Mathf.Min(addType1, addType2);

            // no begins, moves or ends, set defaults and end
            if (addType == 4)
            {
                pinchScale = 1.0f;
                rotateAngle = 0.0f;
                return;
            }

            // calculate rotation
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            float xRot1 = x - threshold;
            float yRot1 = y;
            float xRot2 = x + threshold;
            float yRot2 = y; float distance = threshold * pinchScale;
            xRot1 = x - distance;
            yRot1 = y;
            xRot2 = x + distance;
            yRot2 = y;
            RotateAroundPoint(ref xRot1, ref yRot1, x, y, rotateAngle);
            RotateAroundPoint(ref xRot2, ref yRot2, x, y, rotateAngle);

#if DEBUG

            if (scrollDelta != 0.0f)
            {
                Debug.Log("Mouse delta: " + scrollDelta);
            }

#endif

            // calculate rotation and zoom based on mouse values
            if (addType == 1)
            {
                // moved
                rotatePinch1 = new GestureTouch(int.MaxValue - 5, xRot1, yRot1, rotatePinch1.X, rotatePinch1.Y, 0.0f);
                rotatePinch2 = new GestureTouch(int.MaxValue - 6, xRot2, yRot2, rotatePinch2.X, rotatePinch2.Y, 0.0f);
                touchesMoved.Add(rotatePinch1);
                touchesMoved.Add(rotatePinch2);
            }
            else if (addType == 2)
            {
                // begin
                rotatePinch1 = new GestureTouch(int.MaxValue - 5, xRot1, yRot1, xRot1, yRot1, 0.0f);
                rotatePinch2 = new GestureTouch(int.MaxValue - 6, xRot2, yRot2, xRot2, yRot2, 0.0f);
                touchesBegan.Add(rotatePinch1);
                touchesBegan.Add(rotatePinch2);
            }
            else if (addType == 3)
            {
                // end
                touchesEnded.Add(rotatePinch1);
                touchesEnded.Add(rotatePinch2);
            }
        }

        private void Awake()
        { 
            // setup state
            for (int i = 0; i < 3; i++)
            {
                mousePrev.Add(new KeyValuePair<float, float>(float.MinValue, float.MinValue));
            }

            // setup DPI, using a default value if it cannot be determined
            DeviceInfo.PixelsPerInch = (int)Screen.dpi;
            if (DeviceInfo.PixelsPerInch > 0)
            {
                DeviceInfo.UnitMultiplier = DeviceInfo.PixelsPerInch;
            }
            else
            {
                // pick a sensible dpi since we don't know the actual DPI
                DeviceInfo.UnitMultiplier = DeviceInfo.PixelsPerInch = DefaultDPI;
            }

            // set the main thread callback so gestures can callback after a delay
            GestureRecognizer.MainThreadCallback = (float delay, System.Action callback) =>
            {
				if(this.gameObject.activeSelf) {
                	StartCoroutine(MainThreadCallback(delay, callback));
				}
            };
        }

		private ICollection<GestureTouch> FilterTouches(ICollection<GestureTouch> touches, GestureRecognizer r)
        {
            // if any gestures are over an object that is not allowing touches, remove them
			filteredTouches.Clear();
            foreach (GestureTouch t in touches)
            {
                if (gameObjectsForTouch[t.Id] == r.PlatformSpecificView as GameObject)
                {
                    filteredTouches.Add(t);
                }
            }
			touches.Clear();
			foreach (GestureTouch t in filteredTouches)
			{
				touches.Add(t);
			}
			return touches;
        }

        private void OnLevelWasLoaded(int level)
        {
            GestureRecognizer.ActiveGestures.Clear();
        }

        private void Start()
        {
            GestureRecognizer.ActiveGestures.Clear();
        }

        private void Update()
        {
            // clear out all touches for each phase
            touchesBegan.Clear();
            touchesMoved.Clear();
            touchesEnded.Clear();

            // process touches and mouse
            ProcessTouches();
            ProcessMouseButtons();
            ProcessMouseWheel();

            // keep track of game objects and touches
            foreach (GestureTouch t in touchesBegan)
            {
                gameObjectsForTouch[t.Id] = GameObjectForTouch(t.Id, t.X, t.Y);
            }

            // for each gesture, process the touches
            foreach (GestureRecognizer r in gestures)
            {
                r.ProcessTouchesBegan(FilterTouches(touchesBegan, r));
                r.ProcessTouchesMoved(FilterTouches(touchesMoved, r));
                r.ProcessTouchesEnded(FilterTouches(touchesEnded, r));
            }

            // remove any game objects that are no longer being touched
            foreach (GestureTouch t in touchesEnded)
            {
                gameObjectsForTouch.Remove(t.Id);
            }

            // clear touches
            touches.Clear();

            // add all the touches
            touches.AddRange(touchesBegan);
            touches.AddRange(touchesMoved);
            touches.AddRange(touchesEnded);
        }

        /// <summary>
        /// Add a gesture to the fingers script. This gesture will give callbacks when it changes state.
        /// </summary>
        /// <param name="gesture">Gesture to add</param>
        /// <return>True if the gesture was added, false if the gesture was already added</return>
        public bool AddGesture(GestureRecognizer gesture)
        {
            if (gestures.Contains(gesture))
            {
                return false;
            }
            gestures.Add(gesture);
            return true;
        }

        /// <summary>
        /// Remove a gesture from the script. The gesture will no longer give callbacks.
        /// </summary>
        /// <param name="gesture">GEsture to remove</param>
        /// <returns>True if the gesture was removed, false if it was not in the script</returns>
        public bool RemoveGesture(GestureRecognizer gesture)
        {
            return gestures.Remove(gesture);
        }

        /// <summary>
        /// Gets a collection of the current touches
        /// </summary>
        public ICollection<GestureTouch> Touches { get { return touches; } }
    }
}
