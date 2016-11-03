
#include "UnityCG.cginc"

struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float4 tangent : TANGENT;
	//float3 normal : NORMAL;
	half4 color : COLOR;
};

struct v2f
{
	half2 uv : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float2 uv2 : TEXCOORD2;
	float4 vertex : SV_POSITION;
	half4 color : COLOR;
};

v2f vert (appdata v)
{
	v2f o;
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.uv;
	o.uv1 = v.uv1;
	o.color = v.color;
	o.uv2 = v.tangent;
	return o;
}

half determineIndex(half2 uv1)
{
	   half x, y;
		x = floor(uv1.x*8+0.5h);
		y = floor(uv1.y*4+0.5h);
		return (y * 8) + x;
}

fixed4 mix (fixed4 vert, fixed4 tex)
{
	/*
	 *	The reason why using max:
	 * 	Font textures is alpha-only, means it's RGB will be black
	 * 	With colors from col, the output color would be the same as i.color 
	 *	But this comes problem for sprites: it's RGB value will be overwritten by col.
	 *	So, every use of Non-alpha-only sprites has set down the i.color to black,
	 *	which automatically handled by Charbox.cs
	 */
	return fixed4(max(vert, tex).rgb, vert.a * tex.a);
}
#ifdef TEX_4_1

	sampler2D _Font0;
	sampler2D _Font1;
	sampler2D _Font2;
	sampler2D _Font3;
	sampler2D _Font4;
	sampler2D _Font5;
	sampler2D _Font6;
	sampler2D _Font7;
	
	fixed4 getTexPoint(half2 uv, half index)
	{
		fixed4 alpha;
		if(index == 0)
		alpha = tex2D(_Font0, uv);
		else if(index == 1)
		alpha = tex2D(_Font1, uv);
		else if(index == 2)
		alpha = tex2D(_Font2, uv);
		else if(index == 3)
		alpha = tex2D(_Font3, uv);
		else if(index == 4)
		alpha = tex2D(_Font4, uv);
		else if(index == 5)
		alpha = tex2D(_Font5, uv);
		else if(index == 6)
		alpha = tex2D(_Font6, uv);
		else if(index == 7)
		alpha = tex2D(_Font7, uv);
		else if(index == 31)
		alpha = fixed4(0, 0, 0, 1);
		else
		alpha = fixed4(0, 0, 0, 0);
		return alpha;
	}
#endif
#ifdef TEX_4_2
			sampler2D _Font8;
			sampler2D _Font9;
			sampler2D _FontA;
			sampler2D _FontB;
			sampler2D _FontC;
			sampler2D _FontD;
			sampler2D _FontE;
			sampler2D _FontF;
			
			fixed4 getTexPoint(half2 uv, half index)
			{
				fixed4 alpha;
				if(index == 8)
				alpha = tex2D(_Font8, uv);
				else if(index == 9)
				alpha = tex2D(_Font9, uv);
				else if(index == 10)
				alpha = tex2D(_FontA, uv);
				else if(index == 11)
				alpha = tex2D(_FontB, uv);
				else if(index == 12)
				alpha = tex2D(_FontC, uv);
				else if(index == 13)
				alpha = tex2D(_FontD, uv);
				else if(index == 14)
				alpha = tex2D(_FontE, uv);
				else if(index == 15)
				alpha = tex2D(_FontF, uv);
				else
				alpha = fixed4(0, 0, 0, 0);
				return alpha;
			}
			#endif
			
			#ifdef TEX_4_3
			sampler2D _Font10;
			sampler2D _Font11;
			sampler2D _Font12;
			sampler2D _Font13;
			sampler2D _Font14;
			sampler2D _Font15;
			sampler2D _Font16;
			sampler2D _Font17;
			
			fixed4 getTexPoint(half2 uv, half index)
			{
				fixed4 alpha;
				if(index == 16)
				alpha = tex2D(_Font10, uv);
				else if(index == 17)
				alpha = tex2D(_Font11, uv);
				else if(index == 18)
				alpha = tex2D(_Font12, uv);
				else if(index == 19)
				alpha = tex2D(_Font13, uv);
				else if(index == 20)
				alpha = tex2D(_Font14, uv);
				else if(index == 21)
				alpha = tex2D(_Font15, uv);
				else if(index == 22)
				alpha = tex2D(_Font16, uv);
				else if(index == 23)
				alpha = tex2D(_Font17, uv);
				else
				alpha = fixed4(0, 0, 0, 0);
				return alpha;
			}
			#endif
			
			#ifdef TEX_4_4
						sampler2D _Font18;
			sampler2D _Font19;
			sampler2D _Font1A;
			sampler2D _Font1B;
			sampler2D _Font1C;
			sampler2D _Font1D;
			sampler2D _Font1E;
			

			fixed4 getTexPoint(half2 uv, half index)
			{
				fixed4 alpha;
				if(index == 24)
				alpha = tex2D(_Font18, uv);
				else if(index == 25)
				alpha = tex2D(_Font19, uv);
				else if(index == 26)
				alpha = tex2D(_Font1A, uv);
				else if(index == 27)
				alpha = tex2D(_Font1B, uv);
				else if(index == 28)
				alpha = tex2D(_Font1C, uv);
				else if(index == 29)
				alpha = tex2D(_Font1D, uv);
				else if(index == 30)
				alpha = tex2D(_Font1E, uv);
				else
				alpha = fixed4(0, 0, 0, 0);
				return alpha;
			}
			#endif
			#ifdef TEX_5_1

			sampler2D _Font0;
			sampler2D _Font1;
			sampler2D _Font2;
			sampler2D _Font3;
			sampler2D _Font4;
			sampler2D _Font5;
			
			fixed4 getTexPoint(half2 uv, half index)
			{
				fixed4 alpha;
				if(index == 0)
				alpha = tex2D(_Font0, uv);
				else if(index == 1)
				alpha = tex2D(_Font1, uv);
				else if(index == 2)
				alpha = tex2D(_Font2, uv);
				else if(index == 3)
				alpha = tex2D(_Font3, uv);
				else if(index == 4)
				alpha = tex2D(_Font4, uv);
				else if(index == 5)
				alpha = tex2D(_Font5, uv);
				else if(index == 31)
				alpha = fixed4(0, 0, 0, 1);
				else
				alpha = fixed4(0, 0, 0, 0);
				return alpha;
			}

			#endif
			#ifdef TEX_5_2
				sampler2D _Font6;
			sampler2D _Font7;
			sampler2D _Font8;
			sampler2D _Font9;
			sampler2D _FontA;
			sampler2D _FontB;
			
			fixed4 getTexPoint(half2 uv, half index)
			{
				fixed4 alpha;
				if(index == 6)
					alpha = tex2D(_Font6, uv);
				else if(index == 7)
					alpha = tex2D(_Font7, uv);
				else if(index == 8)
				alpha = tex2D(_Font8, uv);
				else if(index == 9)
				alpha = tex2D(_Font9, uv);
				else if(index == 10)
				alpha = tex2D(_FontA, uv);
				else if(index == 11)
				alpha = tex2D(_FontB, uv);
				else
				alpha = fixed4(0, 0, 0, 0);
				return alpha;
			}

			#endif
			
			#ifdef TEX_5_3
			
			sampler2D _FontC;
			sampler2D _FontD;
			sampler2D _FontE;
			sampler2D _FontF;
			sampler2D _Font10;
			sampler2D _Font11;
			
			fixed4 getTexPoint(half2 uv, half index)
			{
				fixed4 alpha;
				if(index == 12)
				alpha = tex2D(_FontC, uv);
				else if(index == 13)
				alpha = tex2D(_FontD, uv);
				else if(index == 14)
				alpha = tex2D(_FontE, uv);
				else if(index == 15)
				alpha = tex2D(_FontF, uv);
				else if(index == 16)
				alpha = tex2D(_Font10, uv);
				else if(index == 17)
				alpha = tex2D(_Font11, uv);
				else
				alpha = fixed4(0, 0, 0, 0);
				return alpha;
			}

			#endif
			#ifdef TEX_5_4
			sampler2D _Font12;
			sampler2D _Font13;
			sampler2D _Font14;
			sampler2D _Font15;
			sampler2D _Font16;
			sampler2D _Font17;
			
			fixed4 getTexPoint(half2 uv, half index)
			{
				fixed4 alpha;
				if(index == 18)
				alpha = tex2D(_Font12, uv);
				else if(index == 19)
				alpha = tex2D(_Font13, uv);
				else if(index == 20)
				alpha = tex2D(_Font14, uv);
				else if(index == 21)
				alpha = tex2D(_Font15, uv);
				else if(index == 22)
				alpha = tex2D(_Font16, uv);
				else if(index == 23)
				alpha = tex2D(_Font17, uv);
				else
				alpha = fixed4(0, 0, 0, 0);
				return alpha;
			}

			#endif
			
			#ifdef TEX_5_5
						sampler2D _Font18;
			sampler2D _Font19;
			sampler2D _Font1A;
			sampler2D _Font1B;
			sampler2D _Font1C;
			sampler2D _Font1D;
			sampler2D _Font1E;
			

			fixed4 getTexPoint(half2 uv, half index)
			{
				fixed4 alpha;
				if(index == 24)
				alpha = tex2D(_Font18, uv);
				else if(index == 25)
				alpha = tex2D(_Font19, uv);
				else if(index == 26)
				alpha = tex2D(_Font1A, uv);
				else if(index == 27)
				alpha = tex2D(_Font1B, uv);
				else if(index == 28)
				alpha = tex2D(_Font1C, uv);
				else if(index == 29)
				alpha = tex2D(_Font1D, uv);
				else if(index == 30)
				alpha = tex2D(_Font1E, uv);
				else
				alpha = fixed4(0, 0, 0, 0);
				return alpha;
			}

			#endif