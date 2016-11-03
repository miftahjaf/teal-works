using UnityEngine;
using System.Collections;
using UnityEditor;

public class MiniThumbTextureDrawer : MaterialPropertyDrawer
{
    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        GUILayout.Space(2);
        Texture t = prop.textureValue;
        if(t)
            prop.textureValue = editor.TexturePropertyMiniThumbnail(position, prop, label + " (" + t.name + ")", "RGBA Texture for " + label);
        else
            prop.textureValue = editor.TexturePropertyMiniThumbnail(position, prop, label, "RGBA Texture for " + label);
    }
}

