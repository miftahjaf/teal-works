using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TexConstantUpdater : MonoBehaviour {
    public Text FPS;
    public TEXDraw tex;
    public bool constantUpdate = true;
    [Range(0, 2)]
    public float poolLength;
    float pool;
    float lastGC;
	// Update is called once per frame
	void Update () {
        pool += Time.deltaTime;
        if(constantUpdate)
        {
            tex.color = HSVToRGB(UnityEngine.Random.value, 1, 1, false);
         //   GC.Collect();
        }
        if(pool < poolLength)
            return;
        pool = 0; 
        float deltaGC =  GC.GetTotalMemory(false) - lastGC;
        FPS.text = string.Format("{0:F1} FPS, {1:F1} KB GC", (1 / Time.deltaTime), deltaGC < 0 ? (object)"<color=red>GC Flush</color>" : (object)(deltaGC / 1000));
        lastGC += deltaGC;
	}

    public void SetUpdate(bool v)
    {
        constantUpdate = v;
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

}
