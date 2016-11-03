using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TexConstantUpdater : MonoBehaviour {
    public Text FPS;
    public TEXDraw tex;
    public Text tex2;
    public bool constantUpdate = true;
    public bool updatingTexDraw = true;
    int charLength = 3500;
    [Range(0, 2)]
    public float poolLength;
    float pool;
    float lastGC;
	// Update is called once per frame
	void Update () {
        pool += Time.deltaTime;
        if(constantUpdate)
        {
	        if(updatingTexDraw)
	        {
		        tex.color = HSVToRGB(UnityEngine.Random.value, 1, 1, false);
		        tex.SetTextDirty();
	        } else
                tex2.color = HSVToRGB(UnityEngine.Random.value, 1, 1, false);
         //   GC.Collect();
        }
        if(pool < poolLength)
            return;
        pool = 0; 
        float deltaGC =  GC.GetTotalMemory(false) - lastGC;
        FPS.text = string.Format("{2} Chars, {0:F1} FPS,\n+{1:F1} KB GC"
            , (1 / Time.deltaTime), deltaGC < 0 ? (object)"<color=red>GC Flush</color>" : (object)(deltaGC / 1000)
        , charLength);
        lastGC += deltaGC;
	}

    public void SetUpdate(bool v)
    {
        constantUpdate = v;
    }

    public void SetTxtLength(float length)
    {
        charLength = Mathf.FloorToInt(length);
        tex.text = text.Substring(0, charLength );
        tex2.text = text.Substring(0, charLength);

    }

    public void SetUpdatingTexDraw(bool usingTexDraw)
    {
        updatingTexDraw = usingTexDraw;
        tex.gameObject.SetActive(usingTexDraw);
        tex2.gameObject.SetActive(!usingTexDraw);
    }

    public static Color HSVToRGB (float H, float S, float V, bool hdr)
    {
        Color white = Color.white;
        if (S == 0)
        {
            white.r = V;
            white.g = V;
            white.b = V;
        }
        else
        {
            if (V == 0)
            {
                white.r = 0;
                white.g = 0;
                white.b = 0;
            }
            else
            {
                white.r = 0;
                white.g = 0;
                white.b = 0;
                float num = H * 6;
                int num2 = (int)Mathf.Floor (num);
                float num3 = num - (float)num2;
                float num4 = V * (1 - S);
                float num5 = V * (1 - S * num3);
                float num6 = V * (1 - S * (1 - num3));
                int num7 = num2;
                switch (num7 + 1)
                {
                    case 0:
                        white.r = V;
                        white.g = num4;
                        white.b = num5;
                        break;
                    case 1:
                        white.r = V;
                        white.g = num6;
                        white.b = num4;
                        break;
                    case 2:
                        white.r = num5;
                        white.g = V;
                        white.b = num4;
                        break;
                    case 3:
                        white.r = num4;
                        white.g = V;
                        white.b = num6;
                        break;
                    case 4:
                        white.r = num4;
                        white.g = num5;
                        white.b = V;
                        break;
                    case 5:
                        white.r = num6;
                        white.g = num4;
                        white.b = V;
                        break;
                    case 6:
                        white.r = V;
                        white.g = num4;
                        white.b = num5;
                        break;
                    case 7:
                        white.r = V;
                        white.g = num6;
                        white.b = num4;
                        break;
                }
                if (!hdr)
                {
                    white.r = Mathf.Clamp (white.r, 0, 1);
                    white.g = Mathf.Clamp (white.g, 0, 1);
                    white.b = Mathf.Clamp (white.b, 0, 1);
                }
            }
        }
        return white;
    }

    static public string text = @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna. Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci. Aenean nec lorem. In porttitor. Donec laoreet nonummy augue. Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy. Fusce aliquet pede non pede. Suspendisse dapibus lorem pellentesque magna. Integer nulla. Donec blandit feugiat ligula. Donec hendrerit, felis et imperdiet euismod, purus ipsum pretium metus, in lacinia nulla nisl eget sapien. Donec ut est in lectus consequat consequat. Etiam eget dui. Aliquam erat volutpat. Sed at lorem in nunc porta tristique. Proin nec augue. Quisque aliquam tempor magna. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nunc ac magna. Maecenas odio dolor, vulputate vel, auctor ac, accumsan id, felis. Pellentesque cursus sagittis felis. Pellentesque porttitor, velit lacinia egestas auctor, diam eros tempus arcu, nec vulputate augue magna vel risus. Cras non magna vel ante adipiscing rhoncus. Vivamus a mi. Morbi neque. Aliquam erat volutpat. Integer ultrices lobortis eros. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin semper, ante vitae sollicitudin posuere, metus quam iaculis nibh, vitae scelerisque nunc massa eget pede. Sed velit urna, interdum vel, ultricies vel, faucibus at, quam. Donec elit est, consectetuer eget, consequat quis, tempus quis, wisi. In in nunc. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos hymenaeos. Donec ullamcorper fringilla eros. Fusce in sapien eu purus dapibus commodo. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Cras faucibus condimentum odio. Sed ac ligula. Aliquam at eros. Etiam at ligula et tellus ullamcorper ultrices. In fermentum, lorem non cursus porttitor, diam urna accumsan lacus, sed interdum wisi nibh nec nisl. Ut tincidunt volutpat urna. Mauris eleifend nulla eget mauris. Sed cursus quam id felis. Curabitur posuere quam vel nibh. Cras dapibus dapibus nisl. Vestibulum quis dolor a felis congue vehicula. Maecenas pede purus, tristique ac, tempus eget, egestas quis, mauris. Curabitur non eros. Nullam hendrerit bibendum justo. Fusce iaculis, est quis lacinia pretium, pede metus molestie lacus, at gravida wisi ante at libero. Quisque ornare placerat risus. Ut molestie magna at mi. Integer aliquet mauris et nibh. Ut mattis ligula posuere velit. Nunc sagittis. Curabitur varius fringilla nisl. Duis pretium mi euismod erat. Maecenas id augue. Nam vulputate. Duis a quam non neque lobortis malesuada. Praesent euismod. Donec nulla augue, venenatis scelerisque, dapibus a, consequat at, leo. Pellentesque libero lectus, tristique ac, consectetuer sit amet, imperdiet ut, justo. Sed aliquam odio vitae tortor. Proin hendrerit tempus arcu. In hac habitasse platea dictumst. Suspendisse potenti. Vivamus vitae massa adipiscing est lacinia sodales. Donec metus massa, mollis vel, tempus placerat, vestibulum condimentum, ligula. Nunc lacus metus, posuere eget, lacinia eu, varius quis, libero. Aliquam nonummy adipiscing augue. Er inceptos hymenos.";
}
