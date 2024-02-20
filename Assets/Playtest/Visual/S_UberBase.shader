Shader "Main/UberBase"
{
	Properties
	{
		_BaseMap("Albedo", 2D) = "white" {}
		[Toggle(MAIN_LIGHT_CALCULATE_SHADOWS)] _MainLightShadow("Main Light Shadow", Float) = 1
		[Toggle(_MAIN_LIGHT_SHADOWS_CASCADE)] _ShadowCascade("Shadow Cascade", Float) = 1
		[Toggle(_SHADOWS_SOFT)] _ShadowSoft("Soft Shadow", Float) = 1
	}

	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" }
		Pass 
		{
			Name "ForwardLit"
			Tags{"LightMode" = "UniversalForward"}
			HLSLPROGRAM
			#pragma target 4.5

			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment
			//#pragma multi_compile_local MAIN_LIGHT_CALCULATE_SHADOWS
			//#pragma multi_compile_local _MAIN_LIGHT_SHADOWS_CASCADE
			//#pragma multi_compile_local _SHADOWS_SOFT

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "HL_Lighting.hlsl"

			struct Attributes {
				float3 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};
	        struct Interpolators {
	        	float4 positionCS : SV_POSITION;
	        	float3 positionOS : TEXCOORD1;
	        	float3 positionWS : TEXCOORD2;
	        	float3 viewDirectionOS :TEXCOORD5;
	        	float3 cameraDirectionWS :TEXCOORD6;
				float3 normalOS : TEXCOORD7;
	        	float2 uv : TEXCOORD0;
	        };
			sampler2D _BaseMap;
			float4 _BaseMap_ST;
			Interpolators LitPassVertex(Attributes input) {
				Interpolators output;
				VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS);
				output.positionOS = input.positionOS;
				output.positionCS = posInputs.positionCS;
				output.positionWS = posInputs.positionWS;
				output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
				output.cameraDirectionWS = _WorldSpaceCameraPos - output.positionWS;
				float3 objectSpaceCameraPos = mul(unity_WorldToObject, float4 (_WorldSpaceCameraPos, 1)).xyz;
				output.viewDirectionOS = output.positionOS - objectSpaceCameraPos;
				output.normalOS = input.normal;
				return output;
			};

			half4 LitPassFragment(Interpolators input) : SV_Target
			{
				float3 mainLightDir;
				float3 mainLightColor;
				half mainLightDistAtten;
				half mainLightShadowAtten;
				CalculateMainLight_float(input.positionWS, mainLightDir, mainLightColor, mainLightDistAtten, mainLightShadowAtten);
				float nDotL = dot(mainLightDir, input.normalOS);
				half3 mainLight = nDotL * mainLightColor * mainLightDistAtten * mainLightShadowAtten;
				return half4(mainLight,1);
			};
			ENDHLSL

		}

			 /* Pass
		{
			Name "Meta"
			Tags{"LightMode" = "Meta"}

			Cull Off

			HLSLPROGRAM
			#pragma exclude_renderers gles gles3 glcore
			#pragma target 4.5

			#pragma vertex UniversalVertexMeta
			#pragma fragment UniversalFragmentMetaLit

			#pragma shader_feature EDITOR_VISUALIZATION
			#pragma shader_feature_local_fragment _SPECULAR_SETUP
			#pragma shader_feature_local_fragment _EMISSION
			#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

			#pragma shader_feature_local_fragment _SPECGLOSSMAP

			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

			ENDHLSL
		}*/
	}
}
