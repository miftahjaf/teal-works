
#if (UNITY_5_4_OR_NEWER || UNITY_5_3 || UNITY_5_2_3 || UNITY_5_2_2)
#define USE_VERTEX_HELPER
#endif

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TexDrawLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[AddComponentMenu("TEXDraw/TEXDraw UI", 1)]
public class TEXDraw : MaskableGraphic, ITEXDraw, ILayoutElement, ILayoutSelfController
{

    public TEXPreference pref;

    private DrivenRectTransformTracker layoutTracker;

    [TextArea(3, 15)][SerializeField]
    string m_Text = "TEXDraw";
    [NonSerialized]
    bool m_TextDirty = true;


    public virtual string text
    {
        get { return m_Text; }
        set {
            if (m_Text != value) {
                m_Text = value;
                m_TextDirty = true;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    int m_FontIndex = -1;

    public virtual int fontIndex
    {
        get { return m_FontIndex; }
        set {
            if (m_FontIndex != value) {
                m_FontIndex = Mathf.Clamp(value, -1, 15);
                m_TextDirty = true;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    float m_Size = 50f;

    public virtual float size
    {
        get { return m_Size; }
        set {
            if (m_Size != value) {
                m_Size = Mathf.Max(value, 0f);
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    Fitting m_AutoFit = Fitting.DownScale;

    public virtual Fitting autoFit
    {
        get { return m_AutoFit; }
        set {
            if (m_AutoFit != value) {
                layoutTracker.Clear();
                m_AutoFit = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    Wrapping m_AutoWrap = 0;

    public virtual Wrapping autoWrap
    {
        get { return m_AutoWrap; }
        set {
            if (m_AutoWrap != value) {
                m_AutoWrap = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }


    [SerializeField]
    [Range(0, 2)]
    float m_SpaceSize = 0.2f;

    public virtual float spaceSize
    {
        get { return m_SpaceSize; }
        set {
            if (m_SpaceSize != value) {
                m_SpaceSize = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    Filling m_AutoFill = 0;

    public virtual Filling autoFill
    {
        get { return m_AutoFill; }
        set {
            if (m_AutoFill != value) {
                m_AutoFill = value;
                SetVerticesDirty();
            }
        }
    }

    [SerializeField]
    Vector2 m_Align = new Vector2(0.5f, 0.5f);

    public virtual Vector2 alignment
    {
        get { return m_Align; }
        set {
            if (m_Align != value) {
                m_Align = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    public string debugReport = string.Empty;

    #if UNITY_EDITOR
    protected override void Reset()
    {
        pref = TEXPreference.main;
    }

    [ContextMenu("Repick Preference Asset")]
    public void PickPreferenceAsset()
    {
        pref = TEXPreference.main;
    }

    [ContextMenu("Open Preference")]
    public void OpenPreference()
    {
        UnityEditor.Selection.activeObject = pref;   
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_TextDirty = true;
        if (!pref) {
            pref = TEXPreference.main;
            if (!pref)
                Debug.LogWarning("A TEXDraw Component hasn't the preference yet");
        }
        Font.textureRebuilt += TextureRebuilted;
    }
    #else
	protected override void OnEnable()
	{
	    m_TextDirty = true;
    	if(!TEXPreference.main)
			TEXPreference.main = pref; //Assign the Preference to main stack
		else if(!pref)
			pref = TEXPreference.main; //This component may added runtimely
	   base.OnEnable();
	   Font.textureRebuilt += TextureRebuilted;
    }
#endif

    protected override void OnDisable()
    {
        Font.textureRebuilt -= TextureRebuilted;
        base.OnDisable();
        layoutTracker.Clear();
    }
	
	
	void TextureRebuilted(Font obj)
	{
      //  Debug.Log(obj.name, obj);
		Invoke("SetVerticesDirty",0);
	}
	

    #region Engine

    DrawingContext m_cachedDrawing;

    public DrawingContext drawingContext
    {
        get {
            if (m_cachedDrawing == null)
                m_cachedDrawing = new DrawingContext();
            return m_cachedDrawing;
        }
    }

    protected virtual void FillMesh(Mesh m)
    {
        if (pref == null)
            pref = TEXPreference.main;
         
        #if UNITY_EDITOR
        if (pref.editorReloading)
            return;
        #endif

        CheckTextDirty();

        drawingContext.Render(m, cacheParam);
        hasBoxRecentlyFilled = false;
    }

    public void SetTextDirty()
    {
        SetTextDirty(false);
    }


    public void SetTextDirty(bool forceRedraw)
    {
        hasBoxRecentlyFilled = false;
        m_TextDirty = true;
        if (forceRedraw)
            SetAllDirty();
    }

    [NonSerialized]
    bool hasBoxRecentlyFilled = false;

    void CheckTextDirty()
    {
        if (m_TextDirty) {
            TexUtility.RenderFont = m_FontIndex;
            drawingContext.Parse(/*RegexProcess*/(m_Text), out debugReport);   
            m_TextDirty = false;
        }
        if (!hasBoxRecentlyFilled || (cacheParam.rectArea != rectTransform.rect)) {
	        GenerateParam();
            cacheParam.formulas = DrawingContext.ToRenderers(drawingContext.parsed, cacheParam);
            hasBoxRecentlyFilled = true;
        }
    }

    public override void SetVerticesDirty()
	{
		//This is redundant: 
        base.SetVerticesDirty();
    }

    string RegexProcess(string s)
    {
        //This will recognize \n as new line
        return Regex.Replace(s, @"\\n[\W]", "\n");
    }

    DrawingParams cacheParam;

    DrawingParams GenerateParam()
    {
        if (cacheParam == null) {
            cacheParam = new DrawingParams();
            cacheParam.hasRect = true;
        }
        cacheParam.autoFit = (int)m_AutoFit;
        cacheParam.autoWrap = m_AutoFit == Fitting.RectSize ? 0 : (int)m_AutoWrap;
        cacheParam.alignment = m_Align;
        cacheParam.color = color;
        cacheParam.fontSize = (int)(m_Size * canvas.scaleFactor);
        cacheParam.pivot = rectTransform.pivot;
        cacheParam.rectArea = rectTransform.rect;
        cacheParam.scale = m_Size;
        cacheParam.spaceSize = m_SpaceSize;
        cacheParam.uv3Filling = (int)m_AutoFill;
        return cacheParam;
    }

    public override Material defaultMaterial
    {
        get { return pref.defaultMaterial; }
    }


    List<BaseMeshEffect> meshEffects = new List<BaseMeshEffect>(4);

    protected override void UpdateGeometry()
    {
        FillMesh(workerMesh);

        GetComponents<BaseMeshEffect>(meshEffects);
        for (int i = 0; i < meshEffects.Count; i++) {
            meshEffects[i].ModifyMesh(workerMesh);
        }

        canvasRenderer.SetMesh(workerMesh);
    }

    #endregion

    #region Layout

    public virtual void CalculateLayoutInputHorizontal()
    {
    }

    public virtual void CalculateLayoutInputVertical()
    {
    }

    public virtual void SetLayoutHorizontal()
    {
        CheckTextDirty();
        layoutTracker.Clear();
        if (m_AutoFit == Fitting.RectSize) {
            layoutTracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cacheParam.layoutSize.x);
        }
    }

    public virtual void SetLayoutVertical()
    {
        CheckTextDirty();
        if (m_AutoFit == Fitting.RectSize || m_AutoFit == Fitting.HeightOnly) {
            layoutTracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cacheParam.layoutSize.y);
        }
    }

    public virtual float minWidth
    {
        get { return -1; }
    }

    public virtual float preferredWidth
    {
        get {
          
            CheckTextDirty();
            return cacheParam.layoutSize.x;
        }
    }

    public virtual float flexibleWidth { get { return -1; } }

    public virtual float minHeight
    {
        get { return -1; }
    }

    public virtual float preferredHeight
    {
        get {
            
            CheckTextDirty();
            return cacheParam.layoutSize.y;
        }
    }

    public virtual float flexibleHeight { get { return -1; } }

    public virtual int layoutPriority { get { return 0; } }

    #endregion
}




