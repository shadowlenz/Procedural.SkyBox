// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Skybox/NewProceduralGradient"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[HDR]_top("top", Color) = (1,1,1,0)
		[HDR]_horizon("horizon", Color) = (0.1014151,0.2819172,0.5,0)
		[HDR]_ground("ground", Color) = (0,0.1241035,0.2830189,0)
		_offSet("offSet", Float) = 0
		_GradientPow("GradientPow", Float) = 1
		_groundPow("groundPow", Float) = 15
		[HDR]_cubeMap("cubeMap", CUBE) = "white" {}
		[Toggle(_USECUBEMAP_ON)] _useCubeMap("useCubeMap", Float) = 0
		_NightOpacity("_NightOpacity", Float) = 0
		[Header(Rotation)][Toggle(_ENABLEROTATION_ON)] _EnableRotation("Enable Rotation", Float) = 0
		[IntRange]_cubeMapRot("cubeMapRot", Range( 0 , 360)) = 0
		_CubeMapRotSpeed("CubeMapRotSpeed", Float) = 1
		_sunFade("sunFade", Range( 0.15 , 1)) = 0.159
		_sunRadius("sunRadius", Float) = 0.2
		[Toggle(_SUN_ON)] _Sun("Sun", Float) = 0
		[HDR]_SunColor("SunColor", Color) = (1,1,1,0)
		_MoonOffset("MoonOffset", Range( -2 , 2)) = 0.2
		[Toggle(_FOG_ON)] _Fog("Fog", Float) = 0

	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
	LOD 100
		Cull Off

		
		Pass
		{
			CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif

			#pragma target 3.0 
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_POSITION
			#pragma shader_feature_local _FOG_ON
			#pragma shader_feature_local _SUN_ON
			#pragma shader_feature_local _USECUBEMAP_ON
			#pragma shader_feature_local _ENABLEROTATION_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
			};

			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform float4 _ground;
			uniform float4 _horizon;
			uniform float4 _top;
			uniform float4 _SunColor;
			uniform float _MoonOffset;
			uniform float _sunRadius;
			uniform float _sunFade;
			uniform float _offSet;
			uniform float _GradientPow;
			uniform samplerCUBE _cubeMap;
			uniform half _cubeMapRot;
			uniform half _CubeMapRotSpeed;
			uniform float _NightOpacity;
			uniform float _groundPow;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.texcoord.xy = v.texcoord.xy;
				o.texcoord.zw = v.texcoord1.xy;
				
				// ase common template code
				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float lerpResult219 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
				half CAMERA_MODE220 = lerpResult219;
				float3 appendResult207 = (float3(ase_worldPos.x , ( ase_worldPos.y * CAMERA_MODE220 ) , ase_worldPos.z));
				float3 normalizeResult210 = normalize( appendResult207 );
				float3 appendResult201 = (float3(cos( radians( ( _cubeMapRot + ( _Time.y * _CubeMapRotSpeed ) ) ) ) , 0.0 , ( sin( radians( ( _cubeMapRot + ( _Time.y * _CubeMapRotSpeed ) ) ) ) * -1.0 )));
				float3 appendResult203 = (float3(0.0 , CAMERA_MODE220 , 0.0));
				float3 appendResult204 = (float3(sin( radians( ( _cubeMapRot + ( _Time.y * _CubeMapRotSpeed ) ) ) ) , 0.0 , cos( radians( ( _cubeMapRot + ( _Time.y * _CubeMapRotSpeed ) ) ) )));
				float3 normalizeResult206 = normalize( ase_worldPos );
				#ifdef _ENABLEROTATION_ON
				float3 staticSwitch212 = mul( float3x3(appendResult201, appendResult203, appendResult204), normalizeResult206 );
				#else
				float3 staticSwitch212 = normalizeResult210;
				#endif
				float3 vertexToFrag213 = staticSwitch212;
				o.ase_texcoord2.xyz = vertexToFrag213;
				
				o.ase_texcoord1 = v.vertex;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				fixed4 myColorVar;
				// ase common template code
				float4 temp_cast_0 = (0.0).xxxx;
				float4 appendResult140 = (float4(( _MoonOffset + i.ase_texcoord1.xyz.x ) , i.ase_texcoord1.xyz.y , i.ase_texcoord1.xyz.z , 0.0));
				float clampResult133 = clamp( ( 1.0 - saturate( distance( appendResult140 , float4( _WorldSpaceLightPos0.xyz , 0.0 ) ) ) ) , 0.0 , 1.0 );
				float temp_output_254_0 = ( 8.0 - _sunRadius );
				float clampResult142 = clamp( ( 1.0 - (0.0 + (clampResult133 - ( 0.15 * temp_output_254_0 )) * (1.0 - 0.0) / (( _sunFade * temp_output_254_0 ) - ( 0.15 * temp_output_254_0 ))) ) , 0.0 , 1.0 );
				float clampResult107 = clamp( ( 1.0 - saturate( distance( i.ase_texcoord1.xyz , _WorldSpaceLightPos0.xyz ) ) ) , 0.0 , 1.0 );
				float clampResult109 = clamp( (0.0 + (clampResult107 - ( 0.15 * temp_output_254_0 )) * (1.0 - 0.0) / (( _sunFade * temp_output_254_0 ) - ( 0.15 * temp_output_254_0 ))) , 0.0 , 1.0 );
				float temp_output_143_0 = ( clampResult142 * clampResult109 );
				#ifdef _SUN_ON
				float4 staticSwitch110 = ( _SunColor * temp_output_143_0 );
				#else
				float4 staticSwitch110 = temp_cast_0;
				#endif
				float4 lerpResult127 = lerp( _top , ( staticSwitch110 + _top ) , temp_output_143_0);
				float clampResult16 = clamp( ( ( _offSet + i.ase_texcoord1.xyz.y ) * _GradientPow ) , 0.0 , 1.0 );
				float4 lerpResult11 = lerp( _horizon , lerpResult127 , clampResult16);
				float4 temp_cast_2 = (0.0).xxxx;
				float3 vertexToFrag213 = i.ase_texcoord2.xyz;
				#ifdef _USECUBEMAP_ON
				float4 staticSwitch35 = texCUBE( _cubeMap, vertexToFrag213 );
				#else
				float4 staticSwitch35 = temp_cast_2;
				#endif
				float4 lerpResult242 = lerp( float4( 0,0,0,0 ) , staticSwitch35 , _NightOpacity);
				float clampResult24 = clamp( ( i.ase_texcoord1.xyz.y * _groundPow ) , 0.0 , 1.0 );
				float4 lerpResult17 = lerp( _ground , ( lerpResult11 + lerpResult242 ) , clampResult24);
				#ifdef _FOG_ON
				float4 staticSwitch155 = ( unity_FogColor + lerpResult17 );
				#else
				float4 staticSwitch155 = lerpResult17;
				#endif
				
				
				myColorVar = staticSwitch155;
				return myColorVar;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18909
1920;6;1920;1133;1200.446;1373.228;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;128;-1221.987,-2104.92;Inherit;False;1831.384;818.4923;sun;10;137;136;135;133;132;131;130;129;139;140;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;183;-3557.994,751.8007;Inherit;False;2411;608;Cubemap Coordinates;26;210;208;207;206;205;204;203;202;201;200;199;198;197;196;195;194;193;192;191;190;189;188;187;186;185;184;CUBEMAP;0,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-1093.385,-2070.761;Float;False;Property;_MoonOffset;MoonOffset;16;0;Create;True;0;0;0;False;0;False;0.2;-2;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;137;-1164.544,-1979.965;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;184;-3507.994,1057.801;Half;False;Property;_CubeMapRotSpeed;CubeMapRotSpeed;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;185;-3507.994,929.8007;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;138;-932.962,-2018.315;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;214;-3963.645,1651.879;Inherit;False;860;219;Switch between Perspective / Orthographic camera;4;219;217;216;215;CAMERA MODE;1,0,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;-3251.994,929.8007;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;108;-1254.931,-1192.91;Inherit;False;1831.384;818.4923;sun;17;94;100;92;97;95;96;104;106;102;103;107;105;101;248;249;254;256;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;136;-1058.486,-1729.544;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;186;-3507.994,801.8007;Half;False;Property;_cubeMapRot;cubeMapRot;10;1;[IntRange];Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;140;-945.3023,-1877.945;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;94;-1091.43,-817.534;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;188;-3123.994,801.8007;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-686.7826,-693.418;Float;False;Property;_sunRadius;sunRadius;13;0;Create;True;0;0;0;False;0;False;0.2;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OrthoParams;215;-3915.645,1699.879;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;100;-1084.884,-1021.679;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode;129;-685.0546,-1998.481;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;256;-656.4459,-765.228;Inherit;False;Constant;_Float2;Float 2;18;0;Create;True;0;0;0;False;0;False;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;217;-3611.646,1699.879;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;131;-241.8134,-1780.578;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;254;-510.4459,-712.2281;Inherit;False;2;0;FLOAT;5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-604.0518,-565.8795;Float;False;Property;_sunFade;sunFade;12;0;Create;True;0;0;0;False;0;False;0.159;0.18;0.15;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-3467.645,1699.879;Half;False;Constant;_Float9;Float 9;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;92;-717.9988,-1086.471;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;189;-2995.994,801.8007;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-210.8869,-1888.82;Float;False;Constant;_Float6;Float 6;9;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-357.0922,-786.8735;Float;False;Constant;_Float5;Float 5;8;0;Create;True;0;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-192.0924,-783.8735;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;190;-2835.994,1057.801;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;218;-3067.645,1651.879;Inherit;False;305;165;CAMERA MODE OUTPUT;1;220;;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-169.0518,-633.8794;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;219;-3275.645,1699.879;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;132;-16.89346,-1878.98;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-364.7576,-922.5676;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-335.8311,-1024.81;Float;False;Constant;_Float4;Float 4;9;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;220;-3019.645,1699.879;Half;False;CAMERA_MODE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;248;27.68677,-654.604;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;133;139.1659,-1869.612;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;249;28.34656,-786.34;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;96;-179.8378,-991.9696;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;192;-2483.994,865.8007;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;191;-2483.994,929.8007;Half;False;Constant;_Float7;Float 7;50;0;Create;True;0;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;135;315.3981,-1711.592;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.41;False;2;FLOAT;0.47;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;200;-1843.995,1025.801;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CosOpNode;196;-2483.994,1249.801;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;195;-2483.994,1105.801;Half;False;Constant;_Float8;Float 8;50;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;107;-23.7784,-990.6013;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;-1843.995,1201.801;Inherit;False;220;CAMERA_MODE;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;199;-2483.994,801.8007;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;193;-2291.994,865.8007;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;-2483.994,1025.801;Inherit;False;220;CAMERA_MODE;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;197;-2483.994,1185.801;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;203;-2099.994,993.8007;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;101;282.4538,-799.5815;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.41;False;2;FLOAT;0.47;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;204;-2099.994,1185.801;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;201;-2099.994,801.8007;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;202;-1587.995,1185.801;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;141;620.3661,-1648.109;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;207;-1459.995,1057.801;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.MatrixFromVectors;205;-1843.995,801.8007;Inherit;False;FLOAT3x3;True;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.ClampOpNode;109;619.7206,-731.0997;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;142;810.0975,-1552.472;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;206;-1587.995,929.8007;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;209;-1059.994,929.8007;Inherit;False;394;188;Enable Clouds Rotation;1;212;;0,1,0.4980392,1;0;0
Node;AmplifyShaderEditor.ColorNode;115;549.9652,-1031.697;Float;False;Property;_SunColor;SunColor;15;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1,0.9899999,0.87,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;790.0444,-802.8021;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-1331.994,881.8007;Inherit;False;2;2;0;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;210;-1331.994,1057.801;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-778,211;Float;False;Property;_offSet;offSet;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;10;-833,293;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;1175.692,-760.3525;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;211;-613.9944,943.8007;Inherit;False;265;160;Per Vertex;1;213;;1,0,1,1;0;0
Node;AmplifyShaderEditor.StaticSwitch;212;-1011.994,993.8007;Float;False;Property;_EnableRotation;Enable Rotation;9;0;Create;True;0;0;0;False;1;Header(Rotation);False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;111;481.7579,-199.7303;Float;False;Constant;_Float1;Float 1;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-563,425;Float;False;Property;_GradientPow;GradientPow;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;110;-383.6395,-319.5914;Float;False;Property;_Sun;Sun;14;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-572,295;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;213;-563.9944,993.8007;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;12;-868.9878,-169.9111;Float;False;Property;_top;top;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;0.260509,0.725118,0.828064,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;36;130.4346,678.0221;Float;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;31;12.78816,773.537;Inherit;True;Property;_cubeMap;cubeMap;6;1;[HDR];Create;True;0;0;0;False;0;False;-1;None;41bd1f2c511c9f6428bc0f7efe330511;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-390,368;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-170.7676,-187.8695;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;241;361.8454,824.8358;Inherit;False;Property;_NightOpacity;_NightOpacity;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;16;-254,372;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;35;350.585,661.7147;Float;False;Property;_useCubeMap;useCubeMap;7;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;13;-1027.746,-3.670361;Float;False;Property;_horizon;horizon;1;1;[HDR];Create;True;0;0;0;False;0;False;0.1014151,0.2819172,0.5,0;1,0.9992654,0.9643078,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;23;-353.1854,606.6943;Float;False;Property;_groundPow;groundPow;5;0;Create;True;0;0;0;False;0;False;15;15.67;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;127;116.9985,-141.5681;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-134.1853,561.6943;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;242;603.6453,668.836;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;11;303.6293,-29.70636;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;-135,190;Float;False;Property;_ground;ground;2;1;[HDR];Create;True;0;0;0;False;0;False;0,0.1241035,0.2830189,0;1,0.9992654,0.9643078,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;24;36.81464,514.6943;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;449.922,-27.3164;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;154;488.7086,115.8915;Inherit;False;unity_FogColor;0;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;17;356.6333,215.0787;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;147;721.0098,203.799;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;155;844.4562,282.3719;Float;False;Property;_Fog;Fog;17;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;240;1174.19,123.9631;Float;False;True;-1;2;ASEMaterialInspector;100;5;Skybox/NewProceduralGradient;6e114a916ca3e4b4bb51972669d463bf;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;138;0;139;0
WireConnection;138;1;137;1
WireConnection;187;0;185;0
WireConnection;187;1;184;0
WireConnection;140;0;138;0
WireConnection;140;1;137;2
WireConnection;140;2;137;3
WireConnection;188;0;186;0
WireConnection;188;1;187;0
WireConnection;129;0;140;0
WireConnection;129;1;136;1
WireConnection;217;0;215;2
WireConnection;217;1;215;1
WireConnection;131;0;129;0
WireConnection;254;0;256;0
WireConnection;254;1;106;0
WireConnection;92;0;100;0
WireConnection;92;1;94;1
WireConnection;189;0;188;0
WireConnection;105;0;102;0
WireConnection;105;1;254;0
WireConnection;190;0;189;0
WireConnection;103;0;104;0
WireConnection;103;1;254;0
WireConnection;219;0;216;0
WireConnection;219;1;217;0
WireConnection;219;2;215;4
WireConnection;132;0;130;0
WireConnection;132;1;131;0
WireConnection;97;0;92;0
WireConnection;220;0;219;0
WireConnection;248;0;103;0
WireConnection;133;0;132;0
WireConnection;249;0;105;0
WireConnection;96;0;95;0
WireConnection;96;1;97;0
WireConnection;192;0;190;0
WireConnection;135;0;133;0
WireConnection;135;1;249;0
WireConnection;135;2;248;0
WireConnection;196;0;190;0
WireConnection;107;0;96;0
WireConnection;199;0;190;0
WireConnection;193;0;192;0
WireConnection;193;1;191;0
WireConnection;197;0;190;0
WireConnection;203;0;195;0
WireConnection;203;1;194;0
WireConnection;203;2;195;0
WireConnection;101;0;107;0
WireConnection;101;1;249;0
WireConnection;101;2;248;0
WireConnection;204;0;197;0
WireConnection;204;1;195;0
WireConnection;204;2;196;0
WireConnection;201;0;199;0
WireConnection;201;1;195;0
WireConnection;201;2;193;0
WireConnection;202;0;200;2
WireConnection;202;1;198;0
WireConnection;141;0;135;0
WireConnection;207;0;200;1
WireConnection;207;1;202;0
WireConnection;207;2;200;3
WireConnection;205;0;201;0
WireConnection;205;1;203;0
WireConnection;205;2;204;0
WireConnection;109;0;101;0
WireConnection;142;0;141;0
WireConnection;206;0;200;0
WireConnection;143;0;142;0
WireConnection;143;1;109;0
WireConnection;208;0;205;0
WireConnection;208;1;206;0
WireConnection;210;0;207;0
WireConnection;113;0;115;0
WireConnection;113;1;143;0
WireConnection;212;1;210;0
WireConnection;212;0;208;0
WireConnection;110;1;111;0
WireConnection;110;0;113;0
WireConnection;6;0;7;0
WireConnection;6;1;10;2
WireConnection;213;0;212;0
WireConnection;31;1;213;0
WireConnection;14;0;6;0
WireConnection;14;1;15;0
WireConnection;144;0;110;0
WireConnection;144;1;12;0
WireConnection;16;0;14;0
WireConnection;35;1;36;0
WireConnection;35;0;31;0
WireConnection;127;0;12;0
WireConnection;127;1;144;0
WireConnection;127;2;143;0
WireConnection;22;0;10;2
WireConnection;22;1;23;0
WireConnection;242;1;35;0
WireConnection;242;2;241;0
WireConnection;11;0;13;0
WireConnection;11;1;127;0
WireConnection;11;2;16;0
WireConnection;24;0;22;0
WireConnection;37;0;11;0
WireConnection;37;1;242;0
WireConnection;17;0;18;0
WireConnection;17;1;37;0
WireConnection;17;2;24;0
WireConnection;147;0;154;0
WireConnection;147;1;17;0
WireConnection;155;1;17;0
WireConnection;155;0;147;0
WireConnection;240;0;155;0
ASEEND*/
//CHKSM=9E3ACA86353672CC11837B3E90005F9DE8A078F0