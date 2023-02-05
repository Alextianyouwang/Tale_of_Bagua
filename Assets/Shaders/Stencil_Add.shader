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
	
        Pass
        {
			Blend One Zero
			ZWrite Off
			ColorMask 0

			Stencil
			{
				Ref [_StencilID]
				Comp Always
				Pass IncrSat
				Fail Keep
			}
        }
    }
}
