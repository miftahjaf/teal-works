//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("MaterialUI/Material Button", 100)]
    public class MaterialButton : UIBehaviour, ILayoutGroup, ILayoutElement, ILayoutSelfController
    {
        [SerializeField]
        private RectTransform m_RectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = (RectTransform)transform;
                }
                return m_RectTransform;
            }
        }

        [SerializeField]
        private RectTransform m_ContentRectTransform;
        public RectTransform contentRectTransform
        {
            get { return m_ContentRectTransform; }
            set
            {
                m_ContentRectTransform = value;
                SetLayoutDirty();
            }
        }

        [SerializeField]
        private Button m_ButtonObject;
        public Button buttonObject
        {
            get
            {
                if (m_ButtonObject == null)
                {
                    m_ButtonObject = gameObject.GetAddComponent<Button>();
                }
                return m_ButtonObject;
            }
        }

        [SerializeField]
        private Graphic m_ImageObject;
        public Graphic imageObject
        {
            get { return m_ImageObject; }
            set
            {
                m_ImageObject = value;
                UpdateButtonSettings();
            }
        }

        [SerializeField]
        private Text m_TextObject;
        public Text textObject
        {
            get { return m_TextObject; }
            set
            {
                m_TextObject = value;
                SetLayoutDirty();
            }
        }

        [SerializeField]
        private Graphic m_Icon;
        public Graphic icon
        {
            get { return m_Icon; }
            set
            {
                m_Icon = value;
                SetLayoutDirty();
            }
        }

        [SerializeField]
        private MaterialRipple m_MaterialRipple;
        public MaterialRipple materialRipple
        {
            get
            {
                if (m_MaterialRipple == null)
                {
                    m_MaterialRipple = GetComponent<MaterialRipple>();
                }
                return m_MaterialRipple;
            }
        }

        [SerializeField]
        private MaterialShadow m_MaterialShadow;
        public MaterialShadow materialShadow
        {
            get
            {
                if (m_MaterialShadow == null)
                {
                    m_MaterialShadow = GetComponent<MaterialShadow>();
                }
                return m_MaterialShadow;
            }
        }

        [SerializeField]
        private CanvasGroup m_CanvasGroup;
        public CanvasGroup canvasGroup
        {
            get
            {
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = gameObject.GetAddComponent<CanvasGroup>();
                }
                return m_CanvasGroup;
            }
        }

        [SerializeField]
        private CanvasGroup m_ShadowsCanvasGroup;
        public CanvasGroup shadowsCanvasGroup
        {
            get { return m_ShadowsCanvasGroup; }
            set { m_ShadowsCanvasGroup = value; }
        }

        [SerializeField]
        private bool m_InteractableState;
        public bool interactableState
        {
            get { return m_InteractableState; }
        }

        [SerializeField]
        private string m_Text = "BUTTON";
        public string text
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                ChangeText();
                SetLayoutDirty();
            }
        }

        [SerializeField]
        private Color m_TextColor = MaterialColor.textDark;
        public Color textColor
        {
            get { return m_TextColor; }
            set { m_TextColor = value; }
        }

        [SerializeField]
        private Color m_IconColor = MaterialColor.iconDark;
        public Color iconColor
        {
            get { return m_IconColor; }
            set { m_IconColor = value; }
        }

        [SerializeField]
        private Color m_ButtonColor = Color.white;
        public Color buttonColor
        {
            get { return m_ButtonColor; }
            set { m_ButtonColor = value; }
        }

        [SerializeField]
        private ImageData m_IconData;
        public ImageData iconData
        {
            get { return m_IconData; }
            set
            {
                m_IconData = value;
                ChangeIcon();
            }
        }

        [SerializeField]
        private Vector2 m_ContentPadding = new Vector2(30f, 18f);
        public Vector2 contentPadding
        {
            get { return m_ContentPadding; }
            set
            {
                m_ContentPadding = value;
                SetLayoutDirty();
            }
        }

        [SerializeField]
        private Vector2 m_ContentSize;
        public Vector2 contentSize
        {
            get { return m_ContentSize; }
        }

        [SerializeField]
        private Vector2 m_Size;
        public Vector2 size
        {
            get { return m_Size; }
        }

        [SerializeField]
        private bool m_FitWidthToContent;
        public bool fitWidthToContent
        {
            get { return m_FitWidthToContent; }
            set
            {
                m_FitWidthToContent = value;
                m_Tracker.Clear();
                SetLayoutDirty();
            }
        }

        [SerializeField]
        private bool m_FitHeightToContent;
        public bool fitHeightToContent
        {
            get { return m_FitHeightToContent; }
            set
            {
                m_FitHeightToContent = value;
                m_Tracker.Clear();
                SetLayoutDirty();
            }
        }

        [SerializeField]
        private bool m_ShadowFitsLayout;
        public bool shadowFitsLayout
        {
            get { return m_ShadowFitsLayout; }
            set
            {
                m_ShadowFitsLayout = value;
                SetLayoutDirty();
            }
        }

        private string m_LastText;
        private Color m_LastTextColor;
        private Color m_LastIconColor;
        private Color m_LastButtonColor;
        private Vector2 m_LastPosition;

#if UNITY_EDITOR
		private VectorImageData m_LastIconVectorImageData;
        private Sprite m_LastIconSprite;
#endif

        private DrivenRectTransformTracker m_Tracker = new DrivenRectTransformTracker();

#if UNITY_EDITOR
        public MaterialButton()
        {
            EditorUpdate.Init();
            EditorUpdate.onEditorUpdate += OnEditorUpdate;

            if (m_Icon)
            {
                m_LastIconSprite = m_IconData.sprite;
                m_LastIconVectorImageData = m_IconData.vectorImageData;
            }
        }
#endif

        protected override void OnEnable()
        {
            SetLayoutDirty();

#if UNITY_EDITOR
            OnValidate();

            if (m_Icon)
            {
                m_LastIconSprite = m_IconData.sprite;
                m_LastIconVectorImageData = m_IconData.vectorImageData;
            }
#endif
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
        }

#if UNITY_EDITOR
        protected override void OnDestroy()
        {
            EditorUpdate.onEditorUpdate -= OnEditorUpdate;
        }
#endif

        void Update()
        {
            if (Application.isPlaying)
            {
                CheckInteractive();
                UpdateButtonSettings();
            }
        }

#if UNITY_EDITOR
        private void OnEditorUpdate()
        {
            if (IsDestroyed())
            {
                EditorUpdate.onEditorUpdate -= OnEditorUpdate;
                return;
            }

            if (rectTransform.anchoredPosition != m_LastPosition)
            {
                m_LastPosition = rectTransform.anchoredPosition;
                EditorUtility.SetDirty(rectTransform);
            }

            CheckInteractive();
            UpdateButtonSettings();
        }
#endif

        private void CheckInteractive()
        {
            if (m_InteractableState != buttonObject.interactable)
            {
                m_InteractableState = buttonObject.interactable;

                if (m_InteractableState)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;

                    if (shadowsCanvasGroup)
                    {
                        shadowsCanvasGroup.alpha = 1f;
                    }
                }
                else
                {
                    canvasGroup.alpha = 0.5f;
                    canvasGroup.blocksRaycasts = false;

                    if (shadowsCanvasGroup)
                    {
                        shadowsCanvasGroup.alpha = 0f;
                    }
                }
            }
        }

        public void ChangeColor()
        {
            if (imageObject)
            {
                if (materialRipple)
                {
                    if (materialRipple.rippleCount == 0)
                    {
                        imageObject.color = buttonColor;
                        materialRipple.RefreshSettings();
                    }
                    else
                    {
                        materialRipple.UpdateNormalColor(buttonColor);
                    }
                }
                else if (buttonColor != imageObject.color)
                {
                    imageObject.color = buttonColor;
                }
            }
        }

        public void ChangeText()
        {
            if (textObject)
            {
                if (text != textObject.text)
                {
                    textObject.text = text;
                    SetLayoutDirty();
                }
            }
        }

        public void ChangeTextColor()
        {
            if (textObject)
            {
                if (textColor != textObject.color)
                {
                    textObject.color = textColor;
                }
            }
        }

        public void ChangeIconColor()
        {
            if (icon)
            {
                if (iconColor != icon.color)
                {
                    icon.color = iconColor;
                }
            }
        }

        public void ChangeIcon()
        {
            if (m_Icon != null)
            {
                if (m_Icon.GetImageData() != m_IconData)
                {
                    m_Icon.SetImage(m_IconData);
                }
            }
        }

        private void UpdateButtonSettings()
        {
            if (imageObject)
            {
                if (buttonColor != imageObject.color)
                {
                    if (m_LastButtonColor == buttonColor)
                    {
                        m_ButtonColor = imageObject.color;
                    }
                }
            }

            if (textObject)
            {
                if (text != textObject.text)
                {
                    if (m_LastText == text)
                    {
                        text = textObject.text;
                    }
                }

                if (textColor != textObject.color)
                {
                    if (m_LastTextColor == textColor)
                    {
                        m_TextColor = textObject.color;
                    }
                }
            }

            if (icon)
            {
                if (iconColor != icon.color)
                {
                    if (m_LastIconColor == iconColor)
                    {
                        m_IconColor = icon.color;
                    }
                }
            }

            m_LastButtonColor = buttonColor;
            m_LastText = text;
            m_LastTextColor = textColor;
            m_LastIconColor = iconColor;

            SetLayoutDirty();
        }

        public void Convert(bool noExitGUI = false)
        {
#if UNITY_EDITOR
            bool isCircle = textObject == null;
            bool isRaised = m_ShadowsCanvasGroup != null;

            string flatRoundedSquare = "Assets/MaterialUI/Images/RoundedSquare/roundedsquare_";
            string raisedRoundedSquare = "Assets/MaterialUI/Images/RoundedSquare_Stroke/roundedsquare_stroke_";

            string imagePath = "";

            if (!isCircle)
            {
                imagePath = isRaised ? flatRoundedSquare : raisedRoundedSquare;
            }

            if (isRaised)
            {
                DestroyImmediate(m_ShadowsCanvasGroup.gameObject);
                m_ShadowsCanvasGroup = null;

                if (materialShadow)
                {
                    DestroyImmediate(materialShadow);
                }

                if (materialRipple != null)
                {
                    materialRipple.highlightWhen = MaterialRipple.HighlightActive.Hovered;
                }

                if (m_ButtonColor == Color.white)
                {
                    m_ImageObject.color = Color.clear;
                    UpdateButtonSettings();
                }
            }
            else
            {
                string path = isCircle ? "Assets/MaterialUI/Prefabs/Components/Buttons/Floating Action Button.prefab" : "Assets/MaterialUI/Prefabs/Components/Buttons/Button.prefab";

                GameObject tempButton = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));

                GameObject newShadow = tempButton.transform.FindChild("Shadows").gameObject;

                m_ShadowsCanvasGroup = newShadow.GetComponent<CanvasGroup>();

                RectTransform newShadowRectTransform = (RectTransform)newShadow.transform;

                newShadowRectTransform.SetParent(rectTransform);
                newShadowRectTransform.SetAsFirstSibling();
                newShadowRectTransform.localScale = Vector3.one;
                newShadowRectTransform.localEulerAngles = Vector3.zero;

                RectTransform tempRectTransform = m_ImageObject != null
                    ? (RectTransform)m_ImageObject.transform
                    : rectTransform;

                if (isCircle)
                {
                    newShadowRectTransform.anchoredPosition = Vector2.zero;
                    RectTransformSnap newSnapper = newShadow.GetAddComponent<RectTransformSnap>();
                    newSnapper.sourceRectTransform = tempRectTransform;
                    newSnapper.valuesArePercentage = true;
                    newSnapper.snapWidth = true;
                    newSnapper.snapHeight = true;
                    newSnapper.snapEveryFrame = true;
                    newSnapper.paddingPercent = new Vector2(225, 225);
                    Vector3 tempVector3 = rectTransform.GetPositionRegardlessOfPivot();
                    tempVector3.y -= 1f;
                    newShadowRectTransform.position = tempVector3;
                }
                else
                {
                    newShadowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempRectTransform.GetProperSize().x + 54);
                    newShadowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempRectTransform.GetProperSize().y + 55);
                    Vector3 tempVector3 = rectTransform.GetPositionRegardlessOfPivot();
                    tempVector3.y -= 0.5f;
                    newShadowRectTransform.position = tempVector3;
                }

                DestroyImmediate(tempButton);

                gameObject.AddComponent<MaterialShadow>();

                materialShadow.shadowsActiveWhen = MaterialShadow.ShadowsActive.Hovered;

                materialShadow.shadowAnims = newShadow.GetComponentsInChildren<ShadowAnim>();

                materialShadow.isEnabled = true;

                if (materialRipple != null)
                {
                    materialRipple.highlightWhen = MaterialRipple.HighlightActive.Clicked;
                }

                if (m_ImageObject)
                {
                    if (m_ButtonColor == Color.clear)
                    {
                        m_ImageObject.color = Color.white;
                        UpdateButtonSettings();
                    }
                }
            }

            if (!isCircle)
            {
                SpriteSwapper spriteSwapper = GetComponent<SpriteSwapper>();

                if (spriteSwapper != null)
                {
                    spriteSwapper.sprite1X = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "100%.png");
                    spriteSwapper.sprite2X = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "200%.png");
                    spriteSwapper.sprite4X = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "400%.png");
                }
                else
                {
                    if (m_ImageObject)
                    {
                        ((Image)m_ImageObject).sprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "100%.png");
                    }
                }
            }
            else
            {
                if (!isRaised)
                {

                    RectTransform tempRectTransform = (RectTransform)new GameObject("Stroke", typeof(VectorImage)).transform;

                    tempRectTransform.SetParent(m_ImageObject.rectTransform);
                    tempRectTransform.localScale = Vector3.one;
                    tempRectTransform.localEulerAngles = Vector3.zero;
                    tempRectTransform.anchorMin = Vector2.zero;
                    tempRectTransform.anchorMax = Vector2.one;
                    tempRectTransform.anchoredPosition = Vector2.zero;
                    tempRectTransform.sizeDelta = Vector2.zero;

                    VectorImage vectorImage = tempRectTransform.GetComponent<VectorImage>();
                    vectorImage.vectorImageData = MaterialUIIconHelper.GetIcon("circle_stroke_thin").vectorImageData;
                    vectorImage.sizeMode = VectorImage.SizeMode.MatchMin;
                    vectorImage.color = new Color(0f, 0f, 0f, 0.125f);

                    tempRectTransform.name = "Stroke";
                }
                else
                {
                    VectorImage[] images = imageObject.GetComponentsInChildren<VectorImage>();

                    for (int i = 0; i < images.Length; i++)
                    {
                        if (images[i].name == "Stroke")
                        {
                            DestroyImmediate(images[i].gameObject);
                        }
                    }
                }
            }

            name = isRaised ? name.Replace("Raised", "Flat") : name.Replace("Flat", "Raised");

            if (!noExitGUI)
            {
                GUIUtility.ExitGUI();
            }
#endif
        }

        public void ClearTracker()
        {
            m_Tracker.Clear();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetLayoutDirty();
        }

        protected override void OnCanvasGroupChanged()
        {
            SetLayoutDirty();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetLayoutDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = GetComponent<RectTransform>();
            }
            if (m_ButtonObject == null)
            {
                m_ButtonObject = gameObject.GetAddComponent<Button>();
            }
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = gameObject.GetAddComponent<CanvasGroup>();
            }

            if (m_Icon != null)
            {
                if (m_Icon.GetType() == typeof(Image))
                {
                    m_IconData.imageDataType = ImageDataType.Sprite;

                    if (m_LastIconSprite == m_IconData.sprite)
                    {
                        m_IconData.sprite = m_Icon.GetSpriteImage();
                    }
                    else
                    {
                        m_Icon.SetImage(m_IconData);
                    }

                    m_LastIconSprite = m_IconData.sprite;
                }
                else
                {
                    m_IconData.imageDataType = ImageDataType.VectorImage;

                    if (m_LastIconVectorImageData == m_IconData.vectorImageData)
                    {
                        m_IconData.vectorImageData = m_Icon.GetVectorImage();
                    }
                    else
                    {
                        m_Icon.SetImage(m_IconData);
                    }

                    m_LastIconVectorImageData = m_IconData.vectorImageData;
                }
            }

            SetLayoutDirty();
        }
#endif

        public void SetLayoutDirty()
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        public void SetLayoutHorizontal()
        {
            if (m_FitWidthToContent)
            {
                if (m_ContentRectTransform == null) return;
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_Size.x);
                m_Tracker.Add(this, m_ContentRectTransform, DrivenTransformProperties.SizeDeltaX);
                m_ContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_ContentSize.x);
            }
        }

        public void SetLayoutVertical()
        {
            if (m_FitHeightToContent)
            {
                if (m_ContentRectTransform == null) return;
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Size.y);
                m_Tracker.Add(this, m_ContentRectTransform, DrivenTransformProperties.SizeDeltaY);
                m_ContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_ContentSize.y);
            }
        }

        public void CalculateLayoutInputHorizontal()
        {
            if (m_FitWidthToContent)
            {
                if (m_ContentRectTransform == null) return;
                m_ContentSize.x = LayoutUtility.GetPreferredWidth(m_ContentRectTransform);
                m_Size.x = m_ContentSize.x + m_ContentPadding.x;
            }
            else
            {
                m_Size.x = -1;
            }
        }

        public void CalculateLayoutInputVertical()
        {
            if (m_FitHeightToContent)
            {
                if (m_ContentRectTransform == null) return;
                m_ContentSize.y = LayoutUtility.GetPreferredHeight(m_ContentRectTransform);
                m_Size.y = m_ContentSize.y + m_ContentPadding.y;
            }
            else
            {
                m_Size.y = -1;
            }
        }

        public float minWidth { get { return enabled ? m_Size.x : 0; } }
        public float preferredWidth { get { return -1; } }
        public float flexibleWidth { get { return -1; } }
        public float minHeight { get { return enabled ? m_Size.y : 0; } }
        public float preferredHeight { get { return -1; } }
        public float flexibleHeight { get { return -1; } }
        public int layoutPriority { get { return 1; } }
    }
}