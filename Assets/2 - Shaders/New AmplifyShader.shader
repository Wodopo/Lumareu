// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sprites Default"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Color ("Tint", Color) = (1,1,1,1)
		_Color0("Color 0", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		_Radius("Radius", Range( 0 , 10)) = 0
		_Float0("Float 0", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
			
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		

		
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
			};
			
			uniform fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float4 _Color0;
 uniform float4 _MainTex_ST;
 uniform float _Radius;
 uniform float _Float0;
			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				float3 worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
				OUT.ase_texcoord1.xyz = worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				OUT.ase_texcoord1.w = 0;
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 worldPos = IN.ase_texcoord1.xyz;
				float clampResult41 = clamp( distance( worldPos , float3(0,0,0) ) , 0.0 , _Radius );
				float4 lerpResult44 = lerp( _Color0 , tex2D( _MainTex, uv_MainTex ) , ( 1.0 - (_Float0 + (clampResult41 - 0.0) * (1.0 - _Float0) / (_Radius - 0.0)) ));
				
				fixed4 c = lerpResult44;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13801
0;92;1203;926;1421.863;400.3159;1.3;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;38;-925.1395,268.3821;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;37;-893.9395,112.3819;Float;False;Constant;_Vector0;Vector 0;1;0;0,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;40;-632.6392,202.0819;Float;False;2;0;FLOAT3;0.0,0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;39;-751.5401,334.4731;Float;False;Property;_Radius;Radius;1;0;0;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;45;-741.2649,444.1609;Float;False;Property;_Float0;Float 0;2;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;41;-424.2577,228.379;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;34;-585.3863,-80.61962;Float;False;_MainTex;0;5;SAMPLER2D;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCRemapNode;42;-215.3395,381.8573;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;36;-613.9805,26.66866;Float;False;Property;_Color0;Color 0;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-389.8001,-78.70001;Float;True;Property;_Sampler1;Sampler1;0;1;[PerRendererData];None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;46;-182.1233,179.0887;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;44;19.81443,113.9804;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TemplateMasterNode;33;189.8,76.70001;Float;False;True;2;Float;ASEMaterialInspector;0;4;Sprites Default;0f8ba0101102bb14ebf021ddadce9b49;Sprites Default;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;40;0;38;0
WireConnection;40;1;37;0
WireConnection;41;0;40;0
WireConnection;41;2;39;0
WireConnection;42;0;41;0
WireConnection;42;2;39;0
WireConnection;42;3;45;0
WireConnection;2;0;34;0
WireConnection;46;0;42;0
WireConnection;44;0;36;0
WireConnection;44;1;2;0
WireConnection;44;2;46;0
WireConnection;33;0;44;0
ASEEND*/
//CHKSM=8715912323808AE74D7EF58A4AB098171D803D5A