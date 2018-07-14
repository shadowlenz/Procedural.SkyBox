// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Skybox/ProceduralGradient"
{
	Properties
	{
		_SkyColor1("Top Color", Color) = (0.37, 0.52, 0.73, 0)
		_SkyExponent1("Top Exponent", Float) = 0.5

		_SkyColor2("Horizon Color", Color) = (0.89, 0.96, 1, 0)
		_SkyColor3("Ground Color", Color) = (0.89, 0.96, 1, 0)
		_FogExp("Fog Exponent", Float) = 0.5
		_SkyIntensity("Sky Intensity", Float) = 1.75

		//_Moon ("Moon", 2D) = "white" {}
		_MoonColor("Moon Color", Color) = (1, 0.99, 0.87, 1)
		_MoonIntensity("Moon Intensity", Range(0.0,20.0)) = 10.0
		_MoonScale ("Moon Scale", Range(0.0,1.0)) = 1.0
		_MoonTex ("Texture", 2D) = "black" {}

		_SunColor("Sun Color", Color) = (1, 0.99, 0.87, 1)
		_SunIntensity("Sun Intensity", Range(0.0,20.0)) = 10.0
		_SunScale ("Sun Scale", Range(0.0,1.0)) = 1.0

		_NightStars ("Night Stars", Cube) = "starstexture" {}
		_NightOpacity ("Night Opacity", Range(0.0,1.0)) = 1.0
		_NightSkySpeed("NightSky Speed", Float) = 2

				

	}

		CGINCLUDE

#include "UnityCG.cginc"

		struct appdata
	{
		float4 position : POSITION;
		float3 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float3 texcoord : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
				float3 texcoord2 : TEXCOORD2;
	};

	half3 _SkyColor1;
	half _SkyExponent1;

	half3 _SkyColor2;
	half3 _SkyColor3;
	half _FogExp;

	half _SkyIntensity;

	half3 _MoonColor;
		half _MoonIntensity;
			half _MoonScale;
			 sampler2D _MoonTex;
			 uniform float4 _MoonTex_ST; 

	half3 _SunColor;
	half _SunIntensity;
	half _SunScale;

	samplerCUBE _NightStars;
 	half _NightOpacity;
		half _NightSkySpeed;

	v2f vert(appdata v)
	{
		v2f o;
		o.position = UnityObjectToClipPos(v.position);
		o.texcoord = v.texcoord;

		o.texcoord1 = v.texcoord;

                float s = sin ( _NightSkySpeed * _Time );
                float c = cos ( _NightSkySpeed * _Time );
                float2x2 rotationMatrix = float2x2( c, -s, s, c);
                rotationMatrix *=0.5;
                rotationMatrix +=0.5;
                rotationMatrix = rotationMatrix * 2-1;
                o.texcoord1.xy = mul ( o.texcoord1.xy, rotationMatrix );
  




			 // o.texcoord2.xy = ((v.texcoord) * _MoonTex_ST.xy + _MoonTex_ST.zw);

		return o;
	}



	half4 frag(v2f i) : COLOR
	{
		float3 v = normalize(i.texcoord);

		float p = v.y;
		float p1 = 1 - pow(min(1, 1 - p), pow(0.5, v.x*v.z));
		float p2 = 1 - p1;

		//half3 c_sky = _SkyColor1 * p1 + _SkyColor2 * p2;
		//half3 c_sky =  lerp (_SkyColor2 ,_SkyColor1 ,clamp (	p +_SkyExponent1,0,1) ) ;
			half3 c_sky =  lerp (_SkyColor2 ,_SkyColor1 ,clamp (	p *_SkyExponent1,0,1) ) ;
		//	half3 c_sky = clamp( (_SkyColor1 /(	p+0.5f *_SkyExponent1))*2 ,0,1) ;
		//fog
		c_sky = lerp(_SkyColor3,c_sky,clamp(( v.y*(_FogExp/unity_FogParams.r)),0,1) );

		half3 sun = _SunColor * min(pow(max(0, dot(v, _WorldSpaceLightPos0.xyz)),  550/ _SunScale), 1);
		half3 c_sun =  lerp (0,sun, clamp (	p *(_SkyExponent1+5)+0.6,0,1) ); //fade below horizon

		half3 moon =_MoonColor* pow(max(0, dot(v, -_WorldSpaceLightPos0.xyz)), 550/ _MoonScale) ;
		half3 c_moon = lerp (0,moon, clamp (	p *(_SkyExponent1+5)+0.6,0,1) ); //fade below horizon

		float3 nightSky = 	lerp (0 ,texCUBE (_NightStars, i.texcoord1 ).rgb	 ,clamp (	p *_SkyExponent1,0,1) *_NightOpacity)  ;

		//fixed4 moonCol = tex2D(_MoonTex, i.texcoord2);
;
		half4 skycol = half4((c_sky * _SkyIntensity) +( c_sun * _SunIntensity )  +nightSky, 0);
		//skycol = lerp(skycol, skycol+moonCol*_MoonIntensity, moonCol.a);

		return skycol;
	}

		ENDCG

		SubShader
	{
		Tags{ "RenderType" = "Skybox" "Queue" = "Background" }
			Pass
		{
			ZWrite Off
			Cull Off
			Fog { Mode Off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}
	}
}