using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TexDrawLib;

[ExecuteInEditMode]
public abstract class TEXLinkBase : MonoBehaviour
{
    protected MonoBehaviour target;
    protected Camera triggerCamera;
    public float transitionTime;
    public bool useRealtime;
    public Color normal;
    public Color hover;
    public Color pressed;
    public Color after;

    // Event delegates triggered on click.
    [SerializeField]
    private TEXLinkClickEvent m_OnLinkClicked = new TEXLinkClickEvent();

    protected DrawingContext m_DrawingContext;

    public TEXLinkClickEvent onClick
    {
        get { return m_OnLinkClicked; }
        set { m_OnLinkClicked = value; }
    }

    public ITEXDraw Target
    {
        get { return (ITEXDraw)target; }
    }

    protected virtual void Reset()
    {
        var t = GetComponents<MonoBehaviour>();
        for (int i = 0; i < t.Length; i++) {
            if (t[i] is ITEXDraw) {
                target = t[i];
                break;
            }
        }
        transitionTime = 0.1f;
        useRealtime = false;
       
        normal = Color.white;
        hover = new Color(0.9f, 0.9f, 0.9f, 1f);
        pressed = new Color(0.65f, 0.65f, 0.65f, 1f);
        after = new Color(0.7f, 0.7f, 1f, 1f);
    }


    protected virtual void OnEnable()
    {
        firstInit = true;
    }

    protected virtual void OnDisable()
    {
        ResetLinks();
    }


    public void ResetLinks()
	{
		if(m_DrawingContext == null)
			return;
        for (int i = 0; i < targetState.Count; i++) {
            targetState[i] = 0;
            m_DrawingContext.linkBoxTint[i] = Color.white;
        }
		timeLoop = 0;
    }

    protected List<Color> targetColorFrom = new List<Color>();
    protected List<Color> targetColor = new List<Color>();
    protected List<float> targetProgress = new List<float>();
    /// 0 = Normal, 1 = Hover, 2 = Pressed, 3 = After Press, 4 = After Hover, 5 = After Idle
    protected List<int> targetState = new List<int>();

    float timeLoop;

    int onMouseHot;

    bool firstInit = true;

    protected bool CheckTweenerReady()
    {
        if (!(target is ITEXDraw))
            return false;
        m_DrawingContext = Target.drawingContext;
        while (targetColor.Count < m_DrawingContext.linkBoxKey.Count) {
            targetColor.Add(Color.white);
            targetColorFrom.Add(Color.white);
            targetProgress.Add(1f);
            targetState.Add(0);
        }
        return true;
    }

    protected void DoTransition(int stateNow, int linkIdx)
    {
        if (!CheckTweenerReady() || linkIdx == -1)
            return;
        targetProgress[linkIdx] = 0f;
        targetColorFrom[linkIdx] = m_DrawingContext.linkBoxTint[linkIdx];
        targetState[linkIdx] = stateNow;
        switch (stateNow) {
            case 0: // Color when idle
                targetColor[linkIdx] = normal;
                break;
            case 1: // Color when mouse hovering
                targetColor[linkIdx] = hover;
                break;
            case 2: // Color when the control pressed down
                targetColor[linkIdx] = pressed;
                break;
            case 3: // Color when the control just been released
                targetColor[linkIdx] = hover;
                Press(linkIdx); //And invoke the call.
                break;
            case 4: // Color when the mouse re-enter after been pressed before
                targetColor[linkIdx] = hover;
                break;
            case 5: // Color when the mouse leaves the control after been pressed
                targetColor[linkIdx] = after;
                break;
        }
        timeLoop = 0;
    }

    protected float deltaTime
    {
        get { return useRealtime ? Time.unscaledDeltaTime : Time.deltaTime; }
    }

    protected virtual void Update()
    {
        if (!CheckTweenerReady())
            return;
        // Show the normal color as soon as it gets enabled
        if (firstInit && m_DrawingContext.linkBoxTint.Count > 0) {
            for (int i = 0; i < m_DrawingContext.linkBoxTint.Count; i++) {
                m_DrawingContext.linkBoxTint[i] = normal;
            }
            Target.SetTextDirty(true);
            firstInit = false;
        }

        UpdateInput();
        // Check and update each link transition
        for (int i = 0; i < targetState.Count; i++) {
            var state = SamplePointerStatus(i);
            if (targetState[i] != state) {
                var lastState = targetState[i];
                if (state == 0 && lastState == 5)
                    continue;
                else if (state == 0 && lastState == 4)
                    DoTransition(5, i);
                else if (state == 0 && lastState == 3)
                    DoTransition(5, i);
                else if (state == 1 && lastState == 5)
                    DoTransition(4, i);
                else if (state == 1 && lastState == 4)
                    continue;
                else if (state == 1 && lastState == 3)
                    continue;
                else if (state == 1 && lastState == 2)
                    DoTransition(3, i);
                else
                    DoTransition(state, i);
            }
        }

        // Handle the animation
        if (timeLoop > transitionTime)
            return;
        timeLoop += deltaTime;
        for (int i = 0; i < targetProgress.Count; i++) {
            if (targetProgress[i] >= 1f)
                continue;
            targetProgress[i] += deltaTime / transitionTime;
            m_DrawingContext.linkBoxTint[i] = Color.Lerp(targetColorFrom[i], targetColor[i], targetProgress[i]);
        }
        Target.SetTextDirty(true);
    }

    /// Samples the pointer status for specified link, return 0 = Idle, 1 = Hovered, 2 = Pressed
    protected abstract int SamplePointerStatus(int linkIdx);

    private void Press()
    {
        if (!isActiveAndEnabled)
            return;

        m_OnLinkClicked.Invoke(null);
    }

    private void Press(int index)
    {
        if (!isActiveAndEnabled)
            return;

        if (index == -1 || index >= m_DrawingContext.linkBoxKey.Count)
            m_OnLinkClicked.Invoke(null);
        else
            m_OnLinkClicked.Invoke(m_DrawingContext.linkBoxKey[index]);
    }

    private void Press(string input)
    {
        if (!isActiveAndEnabled)
            return;

        m_OnLinkClicked.Invoke(input);
    }


    public void OnDrawGizmosSelected()
    {
        if (!isActiveAndEnabled)
            return;
        var dc = Target.drawingContext;
        if (dc == null)
            return;
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < dc.linkBoxKey.Count; i++) {
            Rect r = dc.linkBoxRect[i];
            Gizmos.DrawWireCube((r.center), (r.size));
        }
    }


    protected Vector2 input_HoverPos = Vector2.zero;
    protected List<Vector2> input_PressPos = new List<Vector2>();

    void UpdateInput()
    {
        input_PressPos.Clear();

        if (Input.mousePresent) {
            input_HoverPos = Input.mousePosition;
            if (Input.GetMouseButton(0))
                input_PressPos.Add(Input.mousePosition);
        }
        if (Input.touchSupported) {
            for (int i = 0; i < Input.touchCount; i++) {
                var t = Input.GetTouch(i);
                input_PressPos.Add(t.position);
            }
        }
    }

    public void Test(string txt)
    {
        Debug.LogFormat("{0} ({1}): {2}", gameObject.name, Time.timeSinceLevelLoad, txt);
    }

}

[Serializable]
public class TEXLinkClickEvent : UnityEvent<string>
{
}
