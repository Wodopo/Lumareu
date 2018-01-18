// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fire"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Color ("Tint", Color) = (1,1,1,1)
		
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
			float3 mod289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
 float2 mod289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
 float3 permute( float3 x ) { return mod289( ( ( x * 34.0 ) + 1.0 ) * x ); }
 float snoise( float2 v )
 {
 	const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
 	float2 i = floor( v + dot( v, C.yy ) );
 	float2 x0 = v - i + dot( i, C.xx );
 	float2 i1;
 	i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
 	float4 x12 = x0.xyxy + C.xxzz;
 	x12.xy -= i1;
 	i = mod289( i );
 	float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
 	float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
 	m = m * m;
 	m = m * m;
 	float3 x = 2.0 * frac( p * C.www ) - 1.0;
 	float3 h = abs( x ) - 0.5;
 	float3 ox = floor( x + 0.5 );
 	float3 a0 = x - ox;
 	m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
 	float3 g;
 	g.x = a0.x * x0.x + h.x * x0.y;
 	g.yz = a0.yz * x12.xz + h.yz * x12.yw;
 	return 130.0 * dot( m, g );
 }
 
			
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
				float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
				float2 uv5 = IN.texcoord.xy * ( float3( float2( 1,0.69 ) ,  0.0 ) * ase_objectScale ).xy + ( float2( 0,-1 ) * _Time.y );
				float simplePerlin2D2 = snoise( uv5 );
				float clampResult17 = clamp( simplePerlin2D2 , 0.0 , 1.0 );
				float2 uv43 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult49 = clamp( ( ( 1.0 - abs( (-1.0 + (uv43.x - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) ) * 2.82 ) , 0.0 , 1.0 );
				float clampResult54 = clamp( ( ( 1.0 - uv43.y ) * 1.47 ) , 0.0 , 1.0 );
				float2 uv71 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1_0_g1 = uv71;
				float temp_output_6_0_g1 = _Time.y;
				float temp_output_16_0_g1 = (temp_output_1_0_g1).y;
				float YVal31_g1 = ( ( 0.19 * cos( ( ( UNITY_PI * (temp_output_1_0_g1).x ) + ( UNITY_PI * temp_output_6_0_g1 ) ) ) * sin( ( ( temp_output_16_0_g1 * UNITY_PI ) + ( 0.52 / 3.0 ) + ( temp_output_6_0_g1 * UNITY_PI ) ) ) ) + temp_output_16_0_g1 );
				float2 uv100 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,-0.28 );
				float mulTime4_g3 = _Time.y * 2.0;
				float3 appendResult11_g3 = (float3(float3( uv100 ,  0.0 ).x , ( float3( uv100 ,  0.0 ).y + ( sin( ( ( float3( uv100 ,  0.0 ).x * 0.21 ) + mulTime4_g3 ) ) * 0.21 ) ) , float3( uv100 ,  0.0 ).z));
				float clampResult99 = clamp( ( 1.0 - (appendResult11_g3).y ) , 0.0 , 1.0 );
				
				fixed4 c = ( float4(1,0.8154159,0.3308824,1) * ( clampResult17 * clampResult49 * clampResult54 * abs( ( 1.0 / ( ( YVal31_g1 * 1.8 ) / 1.0 ) ) ) ) * clampResult99 );
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
0;92;1203;926;2080.353;365.1687;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;43;-1818.571,455.0923;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ObjectScaleNode;29;-1427.322,-285.4482;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCRemapNode;31;-1454.784,275.019;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-1.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;7;-1398.166,27.88506;Float;False;Constant;_Vector1;Vector 1;2;0;0,-1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;11;-1408.288,163.3956;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;102;-480.486,676.3946;Float;False;Constant;_Vector3;Vector 3;10;0;0,-0.28;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;6;-1418.134,-123.3904;Float;False;Constant;_Vector0;Vector 0;1;0;1,0.69;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;93;-253.5104,660.939;Float;False;Constant;_Float6;Float 6;8;0;0.21;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1188.166,45.88505;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.AbsOpNode;34;-1278.851,266.5201;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1232.322,-100.8483;Float;False;2;2;0;FLOAT2;0,0,0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.TextureCoordinatesNode;100;-260.089,537.5604;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1073.54,-115.849;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;35;-1096.952,266.5523;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;52;-1301.753,492.9497;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;90;46.52151,622.0944;Float;True;Waving Vertex;-1;;3;872b3757863bb794c96291ceeebfb188;3;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleTimeNode;75;-1629.109,883.1523;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;76;-1610.109,956.1523;Float;False;Constant;_Float4;Float 4;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;71;-1678.618,1040.931;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;77;-1599.309,1165.752;Float;False;Constant;_Float5;Float 5;6;0;0.52;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;72;-1620.109,723.1523;Float;False;Constant;_Float0;Float 0;3;0;0.19;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;73;-1663.109,807.1523;Float;False;Constant;_Float3;Float 3;3;0;1.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-929.5297,283.0657;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.82;False;1;FLOAT
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-830.5402,-92.84903;Float;False;Simplex2D;1;0;FLOAT2;0.54,1.36;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-1086.831,503.7631;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;1.47;False;1;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;96;299.6769,575.4697;Float;False;False;True;False;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;69;-1330.618,961.9307;Float;True;CoolWave;-1;;1;a4ec317493edf3b439fcd463a40eca0d;6;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;98;546.0226,486.5363;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;54;-906.2092,502.2262;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;49;-709.5296,263.0657;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;17;-615.3571,-89.17387;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;15;-702.8683,-320.9499;Float;False;Constant;_Color0;Color 0;3;0;1,0.8154159,0.3308824,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;99;717.6926,382.9249;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;103.0293,144.285;Float;False;4;4;0;FLOAT;0.0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;951.9167,117.7489;Float;True;3;3;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TemplateMasterNode;0;1243.554,109.8774;Float;False;True;2;Float;ASEMaterialInspector;0;4;Fire;0f8ba0101102bb14ebf021ddadce9b49;Sprites Default;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;31;0;43;1
WireConnection;12;0;7;0
WireConnection;12;1;11;0
WireConnection;34;0;31;0
WireConnection;30;0;6;0
WireConnection;30;1;29;0
WireConnection;100;1;102;0
WireConnection;5;0;30;0
WireConnection;5;1;12;0
WireConnection;35;0;34;0
WireConnection;52;0;43;2
WireConnection;90;0;100;0
WireConnection;90;1;93;0
WireConnection;90;2;93;0
WireConnection;48;0;35;0
WireConnection;2;0;5;0
WireConnection;53;0;52;0
WireConnection;96;0;90;0
WireConnection;69;0;72;0
WireConnection;69;1;73;0
WireConnection;69;2;75;0
WireConnection;69;3;76;0
WireConnection;69;4;71;0
WireConnection;69;5;77;0
WireConnection;98;0;96;0
WireConnection;54;0;53;0
WireConnection;49;0;48;0
WireConnection;17;0;2;0
WireConnection;99;0;98;0
WireConnection;18;0;17;0
WireConnection;18;1;49;0
WireConnection;18;2;54;0
WireConnection;18;3;69;0
WireConnection;58;0;15;0
WireConnection;58;1;18;0
WireConnection;58;2;99;0
WireConnection;0;0;58;0
ASEEND*/
//CHKSM=3775ECBCD813B8F626DD4349BB01B989B264393A