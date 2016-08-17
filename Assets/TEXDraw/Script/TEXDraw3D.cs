using UnityEngine;
using System.Collections;
using System;
using TexDrawLib;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
[AddComponentMenu("Mesh/TEXDraw 3D")]
public class TEXDraw3D : MonoBehaviour
{
    public TEXPreference pref;

    const string assetID = "TEXDraw 3D Instance";
    public const string assetSID = "TEXDr 3D Instance";
    [TextArea(3, 15)]
    [SerializeField]
    string
        m_Text = "TEXDraw";

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
                Redraw();
            }
            else if (m_Text != value)
            {
                m_Text = value;
                Redraw();
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
                Redraw();
            }
        }
    }

    [SerializeField]
    float
        m_Size = 10f;

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
                Redraw();
            }
        }
    }

    [Range(1, 200)]
    [SerializeField]
    int
        m_FontSize = 40;

    public virtual int fontSize
    {
        get
        {
            return m_FontSize;
        }
        set
        {
            if (m_FontSize != value)
            {
                m_FontSize = Mathf.Max(value, 1);
                Redraw();
            }
        } 
    }

    [SerializeField]
    Color
        m_Color = Color.white;

    public virtual Color color
    {
        get
        {
            return m_Color;
        }
        set
        {
            if (m_Color != value)
            {
                m_Color = value;
                Redraw();
            }
        }
    }

    [SerializeField]
    Material
        m_Material;

    public virtual Material material
    {
        get
        {
            return m_Material;
        }
        set
        {
            if (m_Material != value)
            {
                m_Material = value;
                Repaint();
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
                Redraw();
            }
        }
    }

    [SerializeField]
    Vector2
        m_Align = new Vector2(0.5f, 0.5f);

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
                Redraw();
            }
        }
    }

    public string debugReport = String.Empty;
	
    #if UNITY_EDITOR
    void Reset()
    {
        m_Text = String.Empty;
        pref = TEXPreference.main;
        GetComponent<MeshRenderer>().material = pref.defaultMaterial;
    }

    [ContextMenu("Repick Preference Asset")]
    public void PickPreferenceAsset()
    {
        pref = TEXPreference.main;
    }

    void OnEnable()
    {
        if (!pref)
        {
            pref = TEXPreference.main;
            if (!pref)
                Debug.LogWarning("A TEXDraw 3D Component hasn't the preference yet");
        }
        if (drawingContext == null)
            drawingContext = new DrawingContext();

        Font.textureRebuilt += OnFontRebuild;
        if (!mesh)
        {
            mesh = new Mesh();
            mesh.name = assetID;
        }
        Redraw();
        Repaint();
    }
    #else
	void OnEnable()
	{
		if(!TEXPreference.main)
			TEXPreference.main = pref; //Assign the Preference to main stack
		else if(!pref)
			pref = TEXPreference.main; //This component may added runtimely
		if(drawingContext == null)
			drawingContext = new DrawingContext();
	Font.textureRebuilt += OnFontRebuild;
	if(!mesh)
	{
	mesh=new Mesh();
	mesh.name=assetID;
	}
    Redraw();
    Repaint();
    }
	#endif
    #region Engine

    public DrawingContext drawingContext;
    Mesh mesh;

    public void Redraw()
    {
        if (isActiveAndEnabled)
        {
            if (drawingContext == null || pref == null)
                OnEnable();
            #if UNITY_EDITOR
            if (pref.editorReloading)
                return;
            #endif
            TexUtility.RenderFont = m_FontIndex;
            drawingContext.Parse(m_Text, out debugReport);
            drawingContext.Render(mesh, GenerateParam());
            mesh.RecalculateBounds();
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    public void Repaint()
    {
        if (!m_Material)
            GetComponent<MeshRenderer>().material = pref.defaultMaterial;
        else
            GetComponent<MeshRenderer>().material = m_Material;
    }

    DrawingParams cacheParam;

    DrawingParams GenerateParam()
    {
        if (cacheParam == null)
        {
            cacheParam = new DrawingParams();
            cacheParam.hasRect = false;
            cacheParam.rectArea = new Rect();
        }
        cacheParam.alignment = m_Align;
        cacheParam.color = color;
        cacheParam.fontSize = m_FontSize;
        cacheParam.scale = m_Size;
        cacheParam.spaceSize = m_SpaceSize;
        return cacheParam;
    }

    void OnDestroy()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            DestroyImmediate(mesh);
        }
        else
        {
#endif
            Destroy(mesh);
#if UNITY_EDITOR
        }
#endif
    }

    void OnDisable()
    {
        Font.textureRebuilt -= OnFontRebuild;
    }

    void OnFontRebuild(Font f)
    {
        Redraw();
    }

    #endregion

}

