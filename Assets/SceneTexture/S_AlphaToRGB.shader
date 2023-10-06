Shader "Utility/AlphaToRGB"
{
    Properties
    {
        [NoScaleOffset]_MainTex("Main Tex",2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline"}

      
        Pass
        {
            Name "Alpha"
            Tags {"LightMode" = "UniversalForward"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                VertexPositionInputs posInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = posInputs.positionCS;
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                return half4(col.a, col.a, col.a, 1);
            }
            ENDHLSL
        }
    }
}
