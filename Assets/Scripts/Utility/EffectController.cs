using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EffectController : MonoBehaviour
{
    public Camera overlayCamera;
    public Camera mainCamera;
    public Material blitScreenMaterial;

    private void OnEnable()
    {
        Tutorial.OnToggleDarkenScreen += StartScreenDarkenAnimation;
    }
    private void OnDisable()
    {
        Tutorial.OnToggleDarkenScreen -= StartScreenDarkenAnimation;
        blitScreenMaterial.SetColor("_Tint", new Color(1, 1, 1, 1));
    }

    void StartScreenDarkenAnimation(bool value) 
    {
        StartCoroutine(EaseInOut(1f,1f,0.2f,value,value?TurnOnCamera : null,SetMaterial,value? null :TurnOffCamera));
    }

    IEnumerator EaseInOut(float timeToComplete , float start, float end,bool startToEnd, Action whenStart, Action<float> toDo, Action whenFinsihed) 
    {
        float process = 0;
        whenStart?.Invoke();
        while (process < timeToComplete) 
        {
            process += Time.deltaTime;
            toDo?.Invoke(Mathf.Lerp(startToEnd ? start : end, startToEnd ? end : start, process / timeToComplete));
            yield return null;
        }
        whenFinsihed?.Invoke();
    }

    void SetMaterial(float value) 
    {
        blitScreenMaterial.SetColor("_Tint", new Color(value,value,value,1));

    }

    void TurnOnCamera() 
    {
        overlayCamera.gameObject.SetActive(true);
        overlayCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
        mainCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;

    }
    void TurnOffCamera() 
    {
        overlayCamera.gameObject.SetActive(false);
        overlayCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;
        mainCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;

    }
}
