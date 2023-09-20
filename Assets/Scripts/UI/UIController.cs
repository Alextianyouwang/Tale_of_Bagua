using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;


public class UIController : MonoBehaviour
{

    private Mirror[] hoodMirrors;


    private Camera cam;


    [Range(0,50)]
    public float screenMargin;


    public GameObject arrowObj;
    [HideInInspector]
    public GameObject[] arrows = new GameObject[4];
    private Material[] arrowMaterial = new Material[4];


    private Coroutine easeInCo;
    private float mirrorMargin;

    private float focusedMargin = 0.2f;
    private float transitionMargin = 0.5f;
    private float expandedMargin = 1.2f;

    private float fullMirrorAlpha = 0.15f;

    public static bool canDoUIAnime = true;

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
        Tutorial.OnRequestTutorialMasterSupport += GetSelf;
        Tutorial.OnRequestMovementTutorial += GetSelf;

        
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
        Tutorial.OnRequestTutorialMasterSupport -= GetSelf;
        Tutorial.OnRequestMovementTutorial -= GetSelf;



    }

    private void Start()
    {
        InitializeArrow();
        
    }
    
    UIController GetSelf() 
    {
        return this;
    }
    void ReceiveHoodMirror(Mirror[] hoodMirror)
    {
        hoodMirrors = hoodMirror;
    }

    private void StartUIEaseInOut(float time, bool easein,bool onlyEaseInMaterial)
    {
        if (!canDoUIAnime)
            return;
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


            Vector3[] corners = GetHoodMirrorCorner(2, mirrorMargin);
            mirrorMargin = Mathf.Lerp(originalMargin, targetMargin, percent);

            if (!onlyAnimateMaterial)
            for (int i = 0; i < 4; i++)
                    arrows[i].transform.position = ClampToScreenBound(corners[i],screenMargin);
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

    public void StartControlledUIEaseInOut(float time, float startMargin, float targetMargin, float startAlpha, float finishAlpha, bool remainActive, Action next)  
    {
        StartCoroutine(ControlledUI_EaseInOut(time, startMargin, targetMargin, startAlpha, finishAlpha,remainActive, next));
     }
    public IEnumerator ControlledUI_EaseInOut(float time, float startMargin, float targetMargin, float startAlpha, float finishAlpha,bool remainActive, Action next)
    {
        float percent = 0;
        for (int i = 0; i < 4; i++)
        {
            arrows[i].SetActive(true);
        }
        while (percent < 1)
        {
            percent += Time.deltaTime / time;
            mirrorMargin = Mathf.Lerp(startMargin, targetMargin, percent);
            Vector3[] corners = GetHoodMirrorCorner(2, mirrorMargin);
            for (int i = 0; i < 4; i++)
            {
            
                arrows[i].transform.position = ClampToScreenBound(corners[i], screenMargin);

                float alpha = Mathf.Lerp(startAlpha, finishAlpha, percent);
                Color matCol = new Color(arrowMaterial[i].color.r, arrowMaterial[i].color.g, arrowMaterial[i].color.b, alpha);
                arrowMaterial[i].color = matCol;

            }
            yield return null;

        }

        for (int i = 0; i < 4; i++)
            {
                arrows[i].SetActive(remainActive);
            }

        next?.Invoke();
    }
        public Vector3[] GetHoodMirrorCorner(float yValue,float margin)
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

    public Vector3 ClampToScreenBound(Vector3 target,float margin) 
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
        if (!canDoUIAnime)
            return;
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
        if (!canDoUIAnime)
            return;
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
        if (!canDoUIAnime)
            return;
        if (hoodMirrors.Length <= 1 && !disableOverrite)
            return;

            for (int i = 0; i < 4; i++)
            {

                arrows[i].gameObject.SetActive(false);
            }

           
    }

    private void UpdateUIOnCollapsing() 
    {
        if (!canDoUIAnime)
            return;
        if (hoodMirrors.Length <= 1)
            return;


        ConstantlyUpdatingUIPos(mirrorMargin,screenMargin);
    }

    public void ConstantlyUpdatingUIPos(float mirrorMargin, float screenMargin) 
    {
        Vector3[] corners = GetHoodMirrorCorner(2, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[i].transform.position = ClampToScreenBound(corners[i], screenMargin);

        }
    }

    private void UpdateUIOnExpand(bool onlyUpdateMaterial) 
    {
        if (!canDoUIAnime)
            return;
        if (hoodMirrors.Length <= 1)
            return;

        for (int i = 0; i < 4; i++)
        {
            StartUIEaseInOut(0.2f, false,onlyUpdateMaterial);
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

    public class ArrowData 
    {
        public float radiusOffset = 0;
        public float alphaOffset = 0;

        public async void RadiusOffsetIncreaseTo(float value, float speed) 
        {
            while (radiusOffset < value) 
            {
                radiusOffset += Time.deltaTime * speed;
                await Task.Yield();
            }
        }

        public async void AlphaOffsetDecreaseTo(float value, float speed)
        {
            while (alphaOffset > value)
            {
                alphaOffset -= Time.deltaTime * speed;
                await Task.Yield();
            }
        }
    }



    public void GroupControlArrows(Vector3 centerPosition, float radius, float offsetDegree,float materialAlpha, bool setActive, ArrowData[] arrowDatas)
    {
        float initialOffset = offsetDegree;
        for (int i = 0; i < 4; i++) 
        {
            float sin = Mathf.Sin(Mathf.Deg2Rad* initialOffset);
            float cos = Mathf.Cos(Mathf.Deg2Rad* initialOffset);
            Vector3 targetPos = centerPosition +  new Vector3 (sin, 0, cos);
            Vector3 outwardDir = (centerPosition - targetPos).normalized;
            arrows[i].transform.position = centerPosition + new Vector3(sin, 0, cos) * (radius + arrowDatas[i].radiusOffset);
            arrows[i].transform.forward = outwardDir;
            arrows[i].transform.Rotate(new Vector3(0, setActive? 45f:0f, 0));
            arrows[i].SetActive(setActive);

            initialOffset += 90f;

            Color matCol = new Color(arrowMaterial[i].color.r, arrowMaterial[i].color.g, arrowMaterial[i].color.b, materialAlpha + arrowDatas[i].alphaOffset);
            arrowMaterial[i].color = matCol;
        }
    }

    

    public IEnumerator MoveArrowsAsGroup(Vector3 privousCenterPosition, Transform targetTransform, float previousRadius, float targetRadius, float offsetDegree, float previousMaterialAlpha, float targetMaterialAlpha, float timeToComplete, ArrowData[] arrowDatas, AnimationCurve curve, Action<UIController> next) 
    {
        float time = 0;
        while (time < timeToComplete) 
        {
            time += Time.deltaTime;
            float percentage = time / timeToComplete;
            float interpolate = curve.Evaluate(percentage);
            float alpha = Mathf.Lerp(previousMaterialAlpha, targetMaterialAlpha,interpolate) * fullMirrorAlpha;
            Vector3 centerPosition = Vector3.Lerp(privousCenterPosition, targetTransform.position,interpolate);
            float radius = Mathf.Lerp(previousRadius, targetRadius, interpolate);
            GroupControlArrows(centerPosition, radius, offsetDegree,alpha,true,arrowDatas);

            yield return null;

        }
        next?.Invoke(this);
    }

    public IEnumerator ArrowsFollowObject(Transform targetTransform , float targetRadius, float offsetDegree, float targetMaterialAlpha, ArrowData[] arrowDatas, Func<bool> condition, Action<UIController> next)
    {
 
        while (condition.Invoke())
        {
            GroupControlArrows(targetTransform.position, targetRadius, offsetDegree, targetMaterialAlpha, true, arrowDatas);

            yield return null;

        }
        next?.Invoke(this);

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
