Shader "Custom/Stencil_Add"
{
   Properties
    {
		[IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
    }
	SubShader
    {
        Tags 
		{ 
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"RenderPipeline" = "UniversalPipeline"
		}
		Blend One Zero
			ZTest Always Cull Off ZWrite Off

			Stencil
			{
				Ref [_StencilID]
				Comp Always
				Pass IncrSat
				Fail Keep
			}
        Pass
        {
			
        }
    }
}
