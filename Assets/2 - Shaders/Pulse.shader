// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Pulse"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Color ("Tint", Color) = (1,1,1,1)
		_Float0("Float 0", Range( 1 , 256)) = 0
		_Color1("Color 1", Color) = (0,1,0.9254903,1)
		_Radius("Radius", Range( 0.01 , 1)) = 0
		_Hardness("Hardness", Range( 0 , 1)) = 1
		_Vector0("Vector 0", Vector) = (0.6,0.6,0,0)
		_Float1("Float 1", Range( 0.01 , 0.5)) = 0
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
			#include "UnityShaderVariables.cginc"


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
			};
			
			uniform fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float _Float0;
 uniform float4 _Color1;
 uniform float _Radius;
 uniform float _Hardness;
 uniform float2 _Vector0;
 uniform float _Float1;
			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				
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
				float2 uv24_g10 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_27_0_g10 = ( (float2( -1,-1 ) + (uv24_g10 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) / _Radius );
				float dotResult28_g10 = dot( temp_output_27_0_g10 , temp_output_27_0_g10 );
				float clampResult29_g10 = clamp( dotResult28_g10 , 0.0 , 1.0 );
				float temp_output_12_0_g10 = _Hardness;
				float2 temp_cast_1 = (( 1.0 - pow( clampResult29_g10 , (1.0 + (( temp_output_12_0_g10 * temp_output_12_0_g10 ) - 0.0) * (100.0 - 1.0) / (1.0 - 0.0)) ) )).xx;
				float cos1 = cos( ( _Time.y * 2.0 ) );
				float sin1 = sin( ( _Time.y * 2.0 ) );
				float2 rotator1 = mul( temp_cast_1 - float2( 0.5,0.5 ) , float2x2( cos1 , -sin1 , sin1 , cos1 )) + float2( 0.5,0.5 );
				float temp_output_5_0 = distance( rotator1 , _Vector0 );
				float2 uv24_g11 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_27_0_g11 = ( (float2( -1,-1 ) + (uv24_g11 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) / 1.0 );
				float dotResult28_g11 = dot( temp_output_27_0_g11 , temp_output_27_0_g11 );
				float clampResult29_g11 = clamp( dotResult28_g11 , 0.0 , 1.0 );
				float temp_output_12_0_g11 = 0.5;
				float4 appendResult96 = (float4((_Color1).rgb , ( ( 1.0 - ( temp_output_5_0 / ( temp_output_5_0 + _Float1 ) ) ) * ( 1.0 - pow( clampResult29_g11 , (1.0 + (( temp_output_12_0_g11 * temp_output_12_0_g11 ) - 0.0) * (100.0 - 1.0) / (1.0 - 0.0)) ) ) )));
				float div100=256.0/float((int)_Float0);
				float4 posterize100 = ( floor( appendResult96 * div100 ) / div100 );
				float4 smoothstepResult104 = smoothstep( float4( 0,0,0,0 ) , float4( 1,1,1,1 ) , posterize100);
				
				fixed4 c = smoothstepResult104;
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
0;92;1203;926;4452.688;1687.127;3.106603;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;72;-1745.886,19.02309;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;54;-2434.96,-95.15739;Float;False;Property;_Hardness;Hardness;1;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;62;-2443.116,-217.4769;Float;False;Property;_Radius;Radius;1;0;0;0.01;1;0;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;68;-2135.738,-210.0404;Float;True;SphereMask;-1;;10;988803ee12caf5f4690caee3c8c4a5bb;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-1566.666,-3.07811;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;7;-1755.415,-139.8032;Float;False;Constant;_Vector1;Vector 1;0;0;0.5,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RotatorNode;1;-1408.297,-195.7892;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;6;-1326.416,43.19678;Float;False;Property;_Vector0;Vector 0;0;0;0.6,0.6;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;116;-1001.054,130.1822;Float;False;Property;_Float1;Float 1;5;0;0;0.01;0.5;0;1;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;5;-1103.415,-148.8033;Float;True;2;0;FLOAT2;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-802.5823,-71.80444;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.05;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;114;-628.3416,-140.6107;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;14;-340.6176,-166.7552;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;78;-359.7584,-386.3882;Float;False;Property;_Color1;Color 1;4;0;0,1,0.9254903,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FunctionNode;118;-412.4334,102.7603;Float;True;SphereMask;-1;;11;988803ee12caf5f4690caee3c8c4a5bb;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-64.49105,-16.63601;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;99;-44.51789,-156.4474;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.DynamicAppendNode;96;224.1695,-79.25847;Float;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;101;188.8603,185.1354;Float;False;Property;_Float0;Float 0;5;0;0;1;256;0;1;FLOAT
Node;AmplifyShaderEditor.PosterizeNode;100;535.1091,40.86854;Float;False;12;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR
Node;AmplifyShaderEditor.SmoothstepOpNode;104;749.6192,41.22205;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR
Node;AmplifyShaderEditor.TemplateMasterNode;0;951.6671,40.85089;Float;False;True;2;Float;ASEMaterialInspector;0;4;Pulse;0f8ba0101102bb14ebf021ddadce9b49;Sprites Default;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;68;0;62;0
WireConnection;68;1;54;0
WireConnection;73;0;72;0
WireConnection;1;0;68;0
WireConnection;1;1;7;0
WireConnection;1;2;73;0
WireConnection;5;0;1;0
WireConnection;5;1;6;0
WireConnection;115;0;5;0
WireConnection;115;1;116;0
WireConnection;114;0;5;0
WireConnection;114;1;115;0
WireConnection;14;0;114;0
WireConnection;117;0;14;0
WireConnection;117;1;118;0
WireConnection;99;0;78;0
WireConnection;96;0;99;0
WireConnection;96;3;117;0
WireConnection;100;1;96;0
WireConnection;100;0;101;0
WireConnection;104;0;100;0
WireConnection;0;0;104;0
ASEEND*/
//CHKSM=CF9BF7496E803AD1662CC0210C2A0177C3D552A5