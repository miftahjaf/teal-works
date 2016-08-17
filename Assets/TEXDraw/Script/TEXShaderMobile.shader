//This shader is intended for some mobiles that imposed texture limit to 8 instead of 16
//It's light, but the downside of using this Shader is two draw calls needed for draw a single component.
Shader "GUI/TEXDraw/Mobile"
{
	Properties
	{
		_Color("Tint", Color) = (1,1,1,1)
		_Base("Block Alpha", Range(0, 1)) = 1
		[Space]
		[NoScaleOffset] _Font0("CMEX", 2D) = "white" {}
		[NoScaleOffset] _Font1("CMMI", 2D) = "white" {} 
		[NoScaleOffset] _Font2("CMSY", 2D) = "white" {}
		[NoScaleOffset] _Font3("CMR", 2D) = "white" {}
		[Space]
		[NoScaleOffset] _Font4("WASY", 2D) = "white" {}
		[NoScaleOffset] _Font5("STMARY", 2D) = "white" {}
		[NoScaleOffset] _Font6("MSAM", 2D) = "white" {}
		[NoScaleOffset] _Font7("MSBM", 2D) = "white" {}
		[Space]
		[NoScaleOffset] _Font8("Custom 0 (CMBX)", 2D) = "white" {}
		[NoScaleOffset] _Font9("Custom 1 (CMSS)", 2D) = "white" {}
		[NoScaleOffset] _FontA("Custom 2 (EUFM)", 2D) = "white" {}
		[NoScaleOffset] _FontB("Custom 3 (EURM)", 2D) = "white" {}
		[NoScaleOffset] _FontC("Custom 4 (EUSM)", 2D) = "white" {}
		[NoScaleOffset] _FontD("Custom 5 (RSFS)", 2D) = "white" {}
		[NoScaleOffset] _FontE("Custom 6 (BBOLD)", 2D) = "white" {}
		[Space]
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

	}
	SubShader
	{
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		Lighting Off 
		Cull Off 
		ZTest [unity_GUIZTestMode]
		ZWrite Off 
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				half4 color : COLOR;
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
			};

			sampler2D _Font0;
			sampler2D _Font1;
			sampler2D _Font2;
			sampler2D _Font3;
			sampler2D _Font4;
			sampler2D _Font5;
			sampler2D _Font6;
			sampler2D _Font7;

			half4 _Color;
			half _Base;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv1;
				o.color = v.color;
				return o;
			}

			half getTexPoint(half2 uv, half index)
			{
			half alpha = 0;
				if(index == 0)
					alpha = tex2D(_Font0, uv).a;
				else if(index == 1)
					alpha = tex2D(_Font1, uv).a;
				else if(index == 2)
					alpha = tex2D(_Font2, uv).a;
				else if(index == 3)
					alpha = tex2D(_Font3, uv).a;
				else if(index == 4)
					alpha = tex2D(_Font4, uv).a;
				else if(index == 5)
					alpha = tex2D(_Font5, uv).a;
				else if(index == 6)
					alpha = tex2D(_Font6, uv).a;
				else if(index == 7)
					alpha = tex2D(_Font7, uv).a;
				else
					alpha = 0;
				return alpha;
			}
			

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col=_Color * i.color;
				half x, y, z;
				x = floor(i.uv1.x*4+0.5h);
				y = floor(i.uv1.y*4+0.5h);
				z = (y * 4) + x;
				col.a *= getTexPoint(i.uv, z);
				return  col;
			}

			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				half4 color : COLOR;
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
			};

			//Remember that up to 16 textures can be loaded in single shader
			//PS : The remaining one texture is preserved for block (plain white) meshes
			sampler2D _Font8;
			sampler2D _Font9;
			sampler2D _FontA;
			sampler2D _FontB;
			sampler2D _FontC;
			sampler2D _FontD;
			sampler2D _FontE;

			half4 _Color;
			half _Base;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv1;
				o.color = v.color;
				return o;
			}

			half getTexPoint(half2 uv, half index)
			{
			half alpha = 0;
				if(index <= 7)
					alpha = 0;
				else if(index == 8)
					alpha = tex2D(_Font8, uv).a;
				else if(index == 9)
					alpha = tex2D(_Font9, uv).a;
				else if(index == 10)
					alpha = tex2D(_FontA, uv).a;
				else if(index == 11)
					alpha = tex2D(_FontB, uv).a;
				else if(index == 12)
					alpha = tex2D(_FontC, uv).a;
				else if(index == 13)
					alpha = tex2D(_FontD, uv).a;
				else if(index == 14)
					alpha = tex2D(_FontE, uv).a;
				else
					alpha = _Base;
				return alpha;
			}
			

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col=_Color * i.color;
				half x, y, z;
				x = floor(i.uv1.x*4+0.5h);
				y = floor(i.uv1.y*4+0.5h);
				z = (y * 4) + x;
				col.a *= getTexPoint(i.uv, z);
				return  col;
			}

			ENDCG
		}
	}
}
