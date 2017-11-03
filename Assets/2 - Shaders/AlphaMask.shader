// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/AlphaMask"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"PreviewType" = "Plane"
	}

		Pass
	{
		//ZWrite Off
		//Cull Off
		//Blend OneMinusDstColor One

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		half4 color : COLOR;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 screenuv : TEXCOORD1;
		half4 color : COLOR;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) * 0.5;
		o.color = v.color;
		return o;
	}

	sampler2D _MainTex;
	sampler2D _LightUniverseTex;
	sampler2D _AlphaMaskTex;

	float4 frag(v2f i) : SV_Target
	{
		float4 alphaMask = tex2D(_AlphaMaskTex, i.screenuv);
		alphaMask.a *= alphaMask.a;

		float2 uv_displaced = i.uv.xy;
		float tiny = 0.01f;
		
		uv_displaced.x += (-0.5 + i.uv.x) * sin(_Time.w) * tiny * alphaMask.a; //sin(_Time.w) * alphaMask.a;
		uv_displaced.y += (-0.5 + i.uv.y) * cos(_Time.w) * tiny * alphaMask.a; //cos(_Time.w) * alphaMask.a;
		
		float4 mainColor = tex2D(_MainTex, i.uv) * i.color;
		float4 otherColor = tex2D(_LightUniverseTex, i.screenuv) * i.color;

		float4 finalComp = (1 - alphaMask.a) * mainColor + alphaMask.a * otherColor;
		float test = clamp((abs(sin(_Time.w)) * 0.5), 0.1, 0.5);

		if (alphaMask.a > 0.1 && alphaMask.a < test)
			finalComp -= alphaMask.a * (.5, .5, .5);

		return finalComp;
	}
		ENDCG
	}
	}
}
