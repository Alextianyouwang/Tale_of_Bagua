using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.GridLayoutGroup;

public class UIController : MonoBehaviour
{

    public Slider slider;

    private Mirror[] hoodMirrors;


    private Camera cam;

    [Range(0,1)]
    public float mirrorMargin;

    [Range(0,50)]
    public float screenMargin;


    public GameObject arrowObj;
    private GameObject[] arrows = new GameObject[4];
    private Material[] arrowMaterial = new Material[4];

    private Vector3[] corners;

    private Coroutine easeInCo;


    private void OnEnable()
    {
        cam = Camera.main;
        MirrorManager.OnChargingCollapse += UpdateUICharge;
        MirrorManager.OnChargedCollapse += UpdateUIFinishedCharge;
        MirrorManager.OnAbortCollapse += UpdateUIAbortCharge;
        MirrorManager.OnCollapsing += UpdateUIOnCollapsing;
        MirrorManager.OnExpand += UpdateUIOnExpand;
        LayerCheck.OnShareHoodMirror += ReceiveHoodMirror;

        
    }

    private void OnDisable()
    {
        MirrorManager.OnChargingCollapse -= UpdateUICharge;
        MirrorManager.OnChargedCollapse -= UpdateUIFinishedCharge;
        MirrorManager.OnAbortCollapse -= UpdateUIAbortCharge;
        MirrorManager.OnCollapsing -= UpdateUIOnCollapsing;
        MirrorManager.OnExpand -= UpdateUIOnExpand;

        LayerCheck.OnShareHoodMirror -= ReceiveHoodMirror;


    }

    private void Start()
    {
        InitializeArrow();
    }
    void ReceiveHoodMirror(Mirror[] hoodMirror)
    {
        hoodMirrors = hoodMirror;
    }

    private void StartUIEaseInOut(float time, bool easein) 
    {
        easeInCo = StartCoroutine(UI_EaseInOut(time,easein));
    }

    private IEnumerator UI_EaseInOut(float time,bool easein)
    {
        float percent = 0;

        float originalMargin = easein ? 1 : 0;
        float targetMargin = easein ? 0 : 1;

  

        while (percent < 1)
        {
            percent += Time.deltaTime / time;


            mirrorMargin = Mathf.Lerp(originalMargin, targetMargin, percent);
            Vector3[] corners = GetHoodMirrorCorner(transform.position.y + 3f, mirrorMargin);


            for (int i = 0; i < 4; i++)
            {
                arrows[i].transform.position = ClampToScreenBound(corners[i],screenMargin);
            }
            if (!easein)
            {
                for (int i = 0; i < 4; i++)
                {
                    arrowMaterial[i].color = Color.Lerp(Color.white, Color.red, 1- percent);
                }
            }


            yield return null;
        }

;       if (!easein) 
        {
            for (int i = 0; i < 4; i++)
            {
                arrows[i].SetActive(false);
            }
        }

    }

    private Vector3[] GetHoodMirrorCorner(float yValue,float margin)
    {
        Vector3[] corners = new Vector3[4];
        // x:top, y:right, z: bottom , w: left 
        Vector4 boundsRegion = new Vector4(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
        foreach (Mirror m in hoodMirrors)
        {
            Bounds frameBounds = m.frameRenderer.bounds;
            float topValue = frameBounds.center.z + frameBounds.extents.z + margin;
            float botValue = frameBounds.center.z - frameBounds.extents.z - margin;
            float rightValue = frameBounds.center.x + frameBounds.extents.x+ margin;
            float leftValue = frameBounds.center.x - frameBounds.extents.x-margin;
            boundsRegion.x = Mathf.Max(boundsRegion.x, topValue);
            boundsRegion.y = Mathf.Max(boundsRegion.y, rightValue);
            boundsRegion.z = Mathf.Min(boundsRegion.z, botValue);
            boundsRegion.w = Mathf.Min(boundsRegion.w, leftValue);
        }
        // topRight
        corners[0] = new Vector3(boundsRegion.y,yValue, boundsRegion.x);
     
        // topLeft
        corners[1] = new Vector3(boundsRegion.w, yValue, boundsRegion.x);
        // botRight
        corners[2] = new Vector3(boundsRegion.y, yValue, boundsRegion.z);
        // botLeft
        corners[3] = new Vector3(boundsRegion.w, yValue, boundsRegion.z);

        return corners;
    }

    Vector3 ClampToScreenBound(Vector3 target,float margin) 
    { 
        Vector3 screenpPoint = cam.WorldToScreenPoint(target);

        if (screenpPoint.x >= cam.pixelWidth - margin)
            screenpPoint.x = cam.pixelWidth - margin; 
        else if (screenpPoint.x <= margin)
            screenpPoint.x = margin;

        if (screenpPoint.y >= cam.pixelHeight - margin)
            screenpPoint.y = cam.pixelHeight - margin;
        else if (screenpPoint.y <= margin)
            screenpPoint.y = margin;

        return cam.ScreenToWorldPoint(screenpPoint);

    }
    private void UpdateUICharge(float timer, float target) 
    {
        if (!slider)
            return;
        if (hoodMirrors.Length <= 1)
            return;

        slider.value = timer / target;

        corners = GetHoodMirrorCorner(transform.position.y + 3f, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[ i].transform.position = ClampToScreenBound(corners[i],screenMargin);

            arrowMaterial[i].color = Color.Lerp(Color.white, Color.red, timer / target);

        }


    }

    private void UpdateUIFinishedCharge(float timer, float target)
    {
        if (!slider)
            return;

        if (hoodMirrors.Length <= 1)
            return;

        corners = GetHoodMirrorCorner(transform.position.y + 3f, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[i].transform.position = ClampToScreenBound(corners[i], screenMargin) ;


            arrowMaterial[i].color = Color.Lerp(Color.white, Color.red, timer / target);

            StartUIEaseInOut(0.5f, true);

        }
        slider.value = 0;
    }

    private void UpdateUIAbortCharge(float timer, float target) 
    {
        if (!slider)
            return;
        if (hoodMirrors.Length <= 1)
            return;

        for (int i = 0; i < 4; i++)
        {
           
            arrows[i].gameObject.SetActive(false);
        }

    }

    private void UpdateUIOnCollapsing() 
    {
        if (hoodMirrors.Length <= 1)
            return;


        corners = GetHoodMirrorCorner(transform.position.y + 3f, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[i].transform.position = ClampToScreenBound(corners[i], screenMargin);


            arrowMaterial[i].color = Color.Lerp(Color.white, Color.red,1);

        }
    }

    private void UpdateUIOnExpand() 
    {
        if (hoodMirrors.Length <= 1)
            return;

        for (int i = 0; i < 4; i++)
        {
            StartUIEaseInOut(0.5f, false);
        }

    }

    private void InitializeArrow() 
    {
        for (int i = 0; i < 4; i++) 
        {
            arrows[i] = Instantiate(arrowObj,Vector3.zero,Quaternion.identity);
            arrows[i].gameObject.SetActive(false);
            arrowMaterial[i] = arrows[i].GetComponent<MeshRenderer>().material;
        }
    }

    private void OnDrawGizmos()
    {
        if (!cam)
            return;
        Gizmos.color = Color.yellow;
        Vector3[] corners = GetHoodMirrorCorner(transform.position.y + 3f,mirrorMargin); 

        foreach (Vector3 corner in corners) 
        {
            Gizmos.DrawSphere( ClampToScreenBound(corner, screenMargin) , 0.1f);
        }
    }
}
