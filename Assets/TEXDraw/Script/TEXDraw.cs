using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TexDrawLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[AddComponentMenu("UI/TEXDraw UI")]
public class TEXDraw : MaskableGraphic, ILayoutElement
{
    public enum WrappingMethod
    {
        NoWrap = 0,
        LetterWrap = 1,
        WordWrap = 2,
        WordWrapJustified = 3
    }

    public TEXPreference pref;

    [TextArea(3, 15)][SerializeField]
    string m_Text = "TEXDraw";
    [NonSerialized]
    string m_TextLayout = string.Empty;

    public virtual string text
    {
        get
        {
            return m_Text;
        }
        set
        {
            if (String.IsNullOrEmpty(value))
            {
                if (String.IsNullOrEmpty(m_Text))
                    return;
                m_Text = "";
                SetVerticesDirty();
            }
            else if (m_Text != value)
            {
                m_Text = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    int m_FontIndex = -1;

    public virtual int fontIndex
    {
        get
        {
            return m_FontIndex;
        }
        set
        {
            if (m_FontIndex != value)
            {
                m_FontIndex = Mathf.Clamp(value, -1, 15);
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    float m_Size = 50f;

    public virtual float size
    {
        get
        {
            return m_Size;
        }
        set
        {
            if (m_Size != value)
            {
                m_Size = Mathf.Max(value, 0f);
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    bool m_AutoFit = true;

    public virtual bool autoFit
    {
        get
        {
            return m_AutoFit;
        }
        set
        {
            if (m_AutoFit != value)
            {
                m_AutoFit = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    WrappingMethod m_AutoWrap = 0;

    public virtual WrappingMethod autoWrap
    {
        get
        {
            return m_AutoWrap;
        }
        set
        {
            if (m_AutoWrap != value)
            {
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
        get
        {
            return m_SpaceSize;
        }
        set
        {
            if (m_SpaceSize != value)
            {
                m_SpaceSize = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    [SerializeField]
    Vector2 m_Align = new Vector2(0.5f, 0.5f);

    public virtual Vector2 alignment
    {
        get
        {
            return m_Align;
        }
        set
        {
            if (m_Align != value)
            {
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
        m_Text = String.Empty;
        pref = TEXPreference.main;
    }

    [ContextMenu("Repick Preference Asset")]
    public void PickPreferenceAsset()
    {
        pref = TEXPreference.main;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!pref)
        {
            pref = TEXPreference.main;
            if (!pref)
                Debug.LogWarning("A TEXDraw Component hasn't the preference yet");
        }
        if (drawingContext == null)
            drawingContext = new DrawingContext();
    }
    #else
	protected override void OnEnable()
	{
    	if(!TEXPreference.main)
			TEXPreference.main = pref; //Assign the Preference to main stack
		else if(!pref)
			pref = TEXPreference.main; //This component may added runtimely
		if(drawingContext == null)
			drawingContext = new DrawingContext();
    base.OnEnable();
	}
#endif

    #region Engine

    public DrawingContext drawingContext;

    #if (UNITY_5_4_OR_NEWER || UNITY_5_3 || UNITY_5_2_3 || UNITY_5_2_2)
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (pref == null)
            pref = TEXPreference.main;
        if (drawingContext == null)
            drawingContext = new DrawingContext();
        vh.Clear();
        
        





#if UNITY_EDITOR
        if (pref.editorReloading)
            return;
        #endif
    
        TexUtility.RenderFont = m_FontIndex;
        drawingContext.Parse(/*RegexProcess*/(m_Text), out debugReport);        
        drawingContext.Render(vh, GenerateParam());
    }
#else
    protected override void OnPopulateMesh(Mesh m)
    {
        if (pref == null)
            pref = TEXPreference.main;
        if (drawingContext == null)
            drawingContext = new DrawingContext();
        
        #if UNITY_EDITOR
        if (pref.editorReloading)
            return;
        #endif
        TexUtility.RenderFont = m_FontIndex;
        drawingContext.Parse(/*RegexProcess*/(m_Text), out debugReport);        
        drawingContext.Render(m, GenerateParam());
    }
    #endif

    string RegexProcess(string s)
    {
        //This will recognize \n as new line
        return Regex.Replace(s, @"\\n[\W]", "\n");
    }

    public float pixelsPerUnit
    {
        get
        {
            //		var localCanvas = canvas;
            //		if (!localCanvas)
            return 1;

            //		return localCanvas.scaleFactor;
        }
    }

    DrawingParams cacheParam;

    DrawingParams GenerateParam()
    {
        if (cacheParam == null)
        {
            cacheParam = new DrawingParams();
            cacheParam.hasRect = true;
        }
        cacheParam.autoFit = m_AutoFit;
        cacheParam.autoWrap = (int)m_AutoWrap;
        cacheParam.alignment = m_Align;
        cacheParam.color = color;
        try
        {
            cacheParam.fontSize = (int)(m_Size * canvas.GetComponent<CanvasScaler>().dynamicPixelsPerUnit);
        }
        catch (Exception)
        {
            cacheParam.fontSize = (int)(m_Size);
        }
        cacheParam.pivot = rectTransform.pivot;
        cacheParam.rectArea = rectTransform.rect;
        cacheParam.scale = m_Size;
        cacheParam.spaceSize = m_SpaceSize;
        return cacheParam;
    }

    public override Material defaultMaterial
    {
        get
        {
            return pref.defaultMaterial;
        }
    }

    #endregion

    #region Layout

    public virtual void CalculateLayoutInputHorizontal()
    {
    }

    public virtual void CalculateLayoutInputVertical()
    {
    }

    public virtual float minWidth
    {
        get { return -1; }
    }

    public virtual float preferredWidth
    {
        get
        {
            if (cacheParam == null || drawingContext == null)
            {
                drawingContext = new DrawingContext();
                GenerateParam();
            }
            if (m_TextLayout != m_Text)
            {
                drawingContext.Parse(text);
                if (!drawingContext.parsingComplete)
                    return 0;
                cacheParam.formulas = DrawingContext.ToRenderers(drawingContext.parsed, cacheParam);
                cacheParam.CalculateRenderedArea();
                m_TextLayout = m_Text;
            }
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
        get
        {
            if (cacheParam == null || drawingContext == null)
            {
                drawingContext = new DrawingContext();
                GenerateParam();
            }
            if (m_TextLayout != m_Text)
            {
                drawingContext.Parse(text);
                if (!drawingContext.parsingComplete)
                    return 0;
                cacheParam.formulas = DrawingContext.ToRenderers(drawingContext.parsed, cacheParam);
                cacheParam.CalculateRenderedArea();
                m_TextLayout = m_Text;
            }
            return cacheParam.layoutSize.y;
        }
    }

    public virtual float flexibleHeight { get { return -1; } }

    public virtual int layoutPriority { get { return 0; } }

    #endregion
}




