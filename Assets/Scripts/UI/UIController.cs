using System.Collections;

using UnityEngine;


public class UIController : MonoBehaviour
{

    private Mirror[] hoodMirrors;


    private Camera cam;


    [Range(0,50)]
    public float screenMargin;


    public GameObject arrowObj;
    private GameObject[] arrows = new GameObject[4];
    private Material[] arrowMaterial = new Material[4];


    private Coroutine easeInCo;
    private float mirrorMargin;

    private float focusedMargin = 0.2f;
    private float transitionMargin = 0.5f;
    private float expandedMargin = 1.2f;

    private float fullMirrorAlpha = 0.15f;

    private void OnEnable()
    {
        cam = Camera.main;
        MirrorManager.OnChargingCollapse += UpdateUICharge;
        MirrorManager.OnChargedCollapse += UpdateUIFinishedCharge;
        MirrorManager.OnAbortCollapse += UpdateUIAbortCharge;
        MirrorManager.OnCollapsing += UpdateUIOnCollapsing;
        MirrorManager.OnExpand += UpdateUIOnExpand;
        //MirrorManager.OnChargingRelease += UpdateUIChargeRelease;
        LayerCheck.OnShareHoodMirror += ReceiveHoodMirror;

        
    }

    private void OnDisable()
    {
        MirrorManager.OnChargingCollapse -= UpdateUICharge;
        MirrorManager.OnChargedCollapse -= UpdateUIFinishedCharge;
        MirrorManager.OnAbortCollapse -= UpdateUIAbortCharge;
        MirrorManager.OnCollapsing -= UpdateUIOnCollapsing;
        MirrorManager.OnExpand -= UpdateUIOnExpand;
        //MirrorManager.OnChargingRelease -= UpdateUIChargeRelease;

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

    private void StartUIEaseInOut(float time, bool easein,bool onlyEaseInMaterial)
    {
        if (easeInCo != null)
            StopCoroutine(easeInCo);
        easeInCo = StartCoroutine(UI_EaseInOut(time,easein, onlyEaseInMaterial));
    }

    private IEnumerator UI_EaseInOut(float time,bool easein, bool onlyAnimateMaterial )
    {
        float percent = 0;

        float originalMargin = easein ? transitionMargin : focusedMargin;
        float targetMargin = easein ? focusedMargin : expandedMargin + 0.4f;

        float originalAlpha = easein ? 0f : fullMirrorAlpha;
        float targetAlpha = easein ? fullMirrorAlpha: 0f;

        while (percent < 1)
        {
            percent += Time.deltaTime / time;

        

            if (!onlyAnimateMaterial)
            for (int i = 0; i < 4; i++)
            {
                    mirrorMargin = Mathf.Lerp(originalMargin, targetMargin, percent);
                    Vector3[] corners = GetHoodMirrorCorner(2, mirrorMargin);
                    arrows[i].transform.position = ClampToScreenBound(corners[i],screenMargin);
            }
            if (!easein)
            {
                for (int i = 0; i < 4; i++)
                {
                    float alpha = Mathf.Lerp(originalAlpha, targetAlpha, percent);
                    Color matCol = new Color(arrowMaterial[i].color.r, arrowMaterial[i].color.g, arrowMaterial[i].color.b, alpha);
                    arrowMaterial[i].color = matCol;
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
        // topLeft
        corners[0] = new Vector3(boundsRegion.w, yValue, boundsRegion.x);
        // topRight
        corners[1] = new Vector3(boundsRegion.y,yValue, boundsRegion.x);
     
      
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
        if (hoodMirrors.Length <= 1)
            return;

        mirrorMargin = Mathf.Lerp(expandedMargin, transitionMargin, timer / target) ;
       Vector3[]  corners = GetHoodMirrorCorner(2, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[ i].transform.position = ClampToScreenBound(corners[i],screenMargin);
            float alpha = Mathf.Lerp(0, 1, timer / target) * fullMirrorAlpha;
            Color matCol = new Color(arrowMaterial[i].color.r, arrowMaterial[i].color.g, arrowMaterial[i].color.b, alpha);
            arrowMaterial[i].color = matCol;

        }


    }
    private void UpdateUIFinishedCharge(float timer, float target)
    {

        if (hoodMirrors.Length <= 1)
            return;

        Vector3[] corners = GetHoodMirrorCorner(2, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[i].transform.position = ClampToScreenBound(corners[i], screenMargin) ;

           
            StartUIEaseInOut(1f, true,false);

        }
    }

    private void UpdateUIAbortCharge(float timer, float target, bool disableOverrite) 
    {
        if (hoodMirrors.Length <= 1 && !disableOverrite)
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


        Vector3[] corners = GetHoodMirrorCorner(2, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[i].transform.position = ClampToScreenBound(corners[i], screenMargin);

        }
    }

    private void UpdateUIOnExpand(bool onlyUpdateMaterial) 
    {
        if (hoodMirrors.Length <= 1)
            return;

        for (int i = 0; i < 4; i++)
        {
            StartUIEaseInOut(1f, false,onlyUpdateMaterial);
        }

    }

    private void InitializeArrow() 
    {
        for (int i = 0; i < 4; i++) 
        {
            arrows[i] = Instantiate(arrowObj,Vector3.zero,Quaternion.Euler(new Vector3(0,i*90f,0)));
            
            arrows[i].gameObject.SetActive(false);
            arrowMaterial[i] = arrows[i].GetComponent<MeshRenderer>().material;
        }
    }

    private void OnDrawGizmos()
    {
        if (!cam)
            return;
        Gizmos.color = Color.yellow;
        Vector3[] corners = GetHoodMirrorCorner(2,mirrorMargin); 

        foreach (Vector3 corner in corners) 
        {
            Gizmos.DrawSphere( ClampToScreenBound(corner, screenMargin) , 0.1f);
        }
    }
}
