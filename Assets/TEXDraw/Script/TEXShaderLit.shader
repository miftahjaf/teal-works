//This shader has exceeded maximum math operations (64), so we have to strip down some things.
Shader "GUI/TEXDraw/Lit Transparent"
{
	Properties
	{
		//_Color ("Main Color", Color) = (1,1,1,1)
		//_Specular ("Specular Color", Color) = (0,0,0,0)
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
		LOD 400

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType"="Plane"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater 0
		ColorMask [_ColorMask]

		CGPROGRAM 
		#pragma surface surf PPL alpha noshadow novertexlights nolightmap nofog
		#include "UnityCG.cginc"
		
		struct appdata_t
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			fixed4 color : COLOR;
			float3 normal : NORMAL;
		};

		struct Input
		{
			half2 uv_Font0;
			half2 uv2_Font1;
			fixed4 color : COLOR;
		};

		//Remember that up to 16 textures can be loaded in single shader
		//PS : The remaining one texture is preserved for block (plain white) meshes
		sampler2D _Font0;
		sampler2D _Font1; 
		sampler2D _Font2;
		sampler2D _Font3;
		sampler2D _Font4;
		sampler2D _Font5;
		sampler2D _Font6;
		sampler2D _Font7;
		sampler2D _Font8;
		sampler2D _Font9;
		sampler2D _FontA;
		sampler2D _FontB;
		sampler2D _FontC;
		sampler2D _FontD;
		sampler2D _FontE;

		//half4 _Color;
		half _Base;
		//fixed4 _Specular;

		half getTexPoint(half2 uv, half index)
		{
			half alpha;
				if(index == 0.0h)
					alpha = tex2D(_Font0, uv).a;
				else if(index == 1.0h)
					alpha = tex2D(_Font1, uv).a;
				else if(index == 2.0h)
					alpha = tex2D(_Font2, uv).a;
				else if(index == 3.0h)
					alpha = tex2D(_Font3, uv).a;
				else if(index == 4.0h)
					alpha = tex2D(_Font4, uv).a;
				else if(index == 5.0h)
					alpha = tex2D(_Font5, uv).a;
				else if(index == 6.0h)
					alpha = tex2D(_Font6, uv).a;
				else if(index == 7.0h)
					alpha = tex2D(_Font7, uv).a;
				else if(index == 8.0h)
					alpha = tex2D(_Font8, uv).a;
				else if(index == 9.0h)
					alpha = tex2D(_Font9, uv).a;
				else if(index == 10.0h)
					alpha = tex2D(_FontA, uv).a;
				else if(index == 11.0h)
					alpha = tex2D(_FontB, uv).a;
				else if(index == 12.0h)
					alpha = tex2D(_FontC, uv).a;
				else if(index == 13.0h)
					alpha = tex2D(_FontD, uv).a;
				else
					alpha = _Base;
				return alpha;		
		}

		void surf (Input IN, inout SurfaceOutput o)
		{			
			fixed4 col = IN.color;
			half z;
			z = (IN.uv2_Font1.y* 16) + IN.uv2_Font1.x*4;
			o.Albedo = col.rgb;
			o.Alpha = col.a * getTexPoint(IN.uv_Font0, z);
		} 

		half4 LightingPPL (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 nNormal = (s.Normal);
			//half shininess = 4.0;

			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif

			// Phong shading model
			//half reflectiveFactor = dot(-viewDir, reflect(lightDir, nNormal));

			// Blinn-Phong shading model
			half reflectiveFactor = dot(nNormal, normalize(lightDir + viewDir));
			
			half diffuseFactor = dot(nNormal, lightDir);
			//half specularFactor = pow(reflectiveFactor, shininess) * s.Specular;

			half4 c;
			c.rgb = (s.Albedo.rgb * diffuseFactor) * _LightColor0.rgb;
			c.rgb *= atten;
			c.a = s.Alpha;
			//clip (c.a - 0.01);
			return c;
		}


		ENDCG
	}
	Fallback "GUI/TEXDraw/Default"
}

