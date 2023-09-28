using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ReplacementShader : MonoBehaviour
{
    public Material screenmat;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (screenmat != null)
            Graphics.Blit(source, destination,screenmat);
        else
            Graphics.Blit(source, destination);

    }
}
