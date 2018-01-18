// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fog"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Color ("Tint", Color) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		_Vector0("Vector 0", Vector) = (1.43,0,0,0)
		_Float0("Float 0", Range( 1 , 2)) = 0
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
			uniform float4 _MainTex_ST;
 uniform float4 _Vector0;
 uniform float _Float0;
 float3 mod289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
 float4 mod289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
 float4 permute( float4 x ) { return mod289( ( x * 34.0 + 1.0 ) * x ); }
 float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
 float snoise( float3 v )
 {
 	const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
 	float3 i = floor( v + dot( v, C.yyy ) );
 	float3 x0 = v - i + dot( i, C.xxx );
 	float3 g = step( x0.yzx, x0.xyz );
 	float3 l = 1.0 - g;
 	float3 i1 = min( g.xyz, l.zxy );
 	float3 i2 = max( g.xyz, l.zxy );
 	float3 x1 = x0 - i1 + C.xxx;
 	float3 x2 = x0 - i2 + C.yyy;
 	float3 x3 = x0 - 0.5;
 	i = mod289( i);
 	float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
 	float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
 	float4 x_ = floor( j / 7.0 );
 	float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
 	float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
 	float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
 	float4 h = 1.0 - abs( x ) - abs( y );
 	float4 b0 = float4( x.xy, y.xy );
 	float4 b1 = float4( x.zw, y.zw );
 	float4 s0 = floor( b0 ) * 2.0 + 1.0;
 	float4 s1 = floor( b1 ) * 2.0 + 1.0;
 	float4 sh = -step( h, 0.0 );
 	float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
 	float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
 	float3 g0 = float3( a0.xy, h.x );
 	float3 g1 = float3( a0.zw, h.y );
 	float3 g2 = float3( a1.xy, h.z );
 	float3 g3 = float3( a1.zw, h.w );
 	float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
 	g0 *= norm.x;
 	g1 *= norm.y;
 	g2 *= norm.z;
 	g3 *= norm.w;
 	float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
 	m = m* m;
 	m = m* m;
 	float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
 	return 42.0 * dot( m, px);
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
				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 _baseColor54 = ( tex2D( _MainTex, uv_MainTex ) * IN.color );
				float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
				float2 uv20 = IN.texcoord.xy * ( ase_objectScale * float3( (_Vector0).xy ,  0.0 ) ).xy + ( (_Vector0).zw * _Time.y );
				float simplePerlin3D32 = snoise( float3( uv20 ,  0.0 ) );
				float _fog51 = abs( simplePerlin3D32 );
				float2 uv47 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				
				fixed4 c = ( _baseColor54 * step( _fog51 , ( 1.0 - pow( uv47.y , _Float0 ) ) ) * ( 1.0 - uv47.y ) * sin( ( uv47.x * UNITY_PI ) ) );
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
0;92;1171;926;1066.929;1171.598;1.3;True;False
Node;AmplifyShaderEditor.CommentaryNode;50;-1857.532,-1696.317;Float;False;1181.983;492.9221;Fog;11;40;38;42;44;43;46;20;32;45;51;67;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;40;-1808.532,-1498.395;Float;False;Property;_Vector0;Vector 0;0;0;1.43,0,0,0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ObjectScaleNode;38;-1503.12,-1646.317;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;44;-1542.532,-1407.395;Float;False;False;False;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleTimeNode;45;-1514.532,-1313.395;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;42;-1540.532,-1498.395;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1323.532,-1405.395;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1316.532,-1551.395;Float;False;2;2;0;FLOAT3;0.0,0;False;1;FLOAT2;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.CommentaryNode;53;-600.514,-1666.485;Float;False;791.7999;444.3079;Base Color;5;3;7;5;6;54;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;20;-1178.045,-1548.33;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;3;-550.514,-1611.885;Float;False;_MainTex;0;5;SAMPLER2D;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-892.4003,-867.9606;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NoiseGeneratorNode;32;-908.5483,-1522.859;Float;False;Simplex3D;1;0;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;94;-936.8062,-674.3815;Float;False;Property;_Float0;Float 0;1;0;0;1;2;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;5;-371.7141,-1616.485;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PiNode;100;-685.4141,-318.3569;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;67;-877.1199,-1439.438;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TemplateFragmentDataNode;7;-206.4908,-1424.177;Float;False;color;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;96;-613.8062,-681.3815;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-441.4141,-314.3569;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-43.71416,-1525.485;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;52;-672.1552,-975.7818;Float;False;51;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-884.4707,-1363.458;Float;False;_fog;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;97;-358.8062,-655.3815;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;102;-148.4141,-295.3569;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-54.99292,-1310.469;Float;False;_baseColor;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.OneMinusNode;48;-420.3412,-829.8389;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;55;-497.0284,-1054.62;Float;False;54;0;1;COLOR
Node;AmplifyShaderEditor.StepOpNode;86;-421.5309,-958.5869;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;95;-804.8062,-1081.381;Float;False;SphereMask;-1;;2;988803ee12caf5f4690caee3c8c4a5bb;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-200.6827,-901.0247;Float;False;4;4;0;COLOR;0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0,0,0,0;False;3;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TemplateMasterNode;0;-51.84009,-890.322;Float;False;True;2;Float;ASEMaterialInspector;0;4;Fog;0f8ba0101102bb14ebf021ddadce9b49;Sprites Default;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;44;0;40;0
WireConnection;42;0;40;0
WireConnection;46;0;44;0
WireConnection;46;1;45;0
WireConnection;43;0;38;0
WireConnection;43;1;42;0
WireConnection;20;0;43;0
WireConnection;20;1;46;0
WireConnection;32;0;20;0
WireConnection;5;0;3;0
WireConnection;67;0;32;0
WireConnection;96;0;47;2
WireConnection;96;1;94;0
WireConnection;99;0;47;1
WireConnection;99;1;100;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;51;0;67;0
WireConnection;97;0;96;0
WireConnection;102;0;99;0
WireConnection;54;0;6;0
WireConnection;48;0;47;2
WireConnection;86;0;52;0
WireConnection;86;1;97;0
WireConnection;33;0;55;0
WireConnection;33;1;86;0
WireConnection;33;2;48;0
WireConnection;33;3;102;0
WireConnection;0;0;33;0
ASEEND*/
//CHKSM=45B70E82B9DEA7A88C571F8E576B782BEEBD6489