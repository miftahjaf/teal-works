//The Default Shader, 2 Pass, Up to 16 Textures
Shader "TEXDraw/Default/1 Pass"
{
	Properties
	{
		[Header(Using 8 Textures and Single Draw Calls for Each Component)]
		_Color("Tint", Color) = (1,1,1,0)
		[Space]
		[MiniThumbTexture] _Font0("Font 0", 2D) = "white" {}
		[MiniThumbTexture] _Font1("Font 1", 2D) = "white" {}
		[MiniThumbTexture] _Font2("Font 2", 2D) = "white" {}
		[MiniThumbTexture] _Font3("Font 3", 2D) = "white" {}
		[MiniThumbTexture] _Font4("Font 4", 2D) = "white" {}
		[MiniThumbTexture] _Font5("Font 5", 2D) = "white" {}
		[MiniThumbTexture] _Font6("Font 6", 2D) = "white" {}
		[MiniThumbTexture] _Font7("Font 7", 2D) = "white" {}
		
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
		
		UsePass "TEXDraw/Default/4 Pass/PRIMARYPASS"
	}
}