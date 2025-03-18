using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class UIController : MonoBehaviour
{
    private Camera cam;


    [Range(0,60)]
    public float screenMargin;

    public Color UIColor = new Color(1f, 0.83f, 0.61f, 1f);

    public GameObject[] arrows = new GameObject[4];
    private MeshRenderer[] arrowRenders = new MeshRenderer[4];
    private MaterialPropertyBlock[] arrowMaterialPropertyBlock  = new MaterialPropertyBlock[4];

    private GameObject[] wasd = new GameObject[4];
    private MeshRenderer[] wasdRenderers = new MeshRenderer[4];
    private MaterialPropertyBlock[] wasdMPB = new MaterialPropertyBlock[4];


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
        MirrorManager.OnChargingCollapse += UI_Behavior_Lerp;
        MirrorManager.OnChargedCollapse += UI_Behavior_InitiateEaseIn;
        MirrorManager.OnAbortCollapse += UI_Behavior_Deactivate;
        MirrorManager.OnCollapsing += UI_Behavior_ConstantlyUpdatePosition;
        MirrorManager.OnExpand += UI_Behavior_InitiateEaseOut;

        Tutorial.OnRequestTutorialMasterSupport += GetSelf;
    }

    private void OnDisable()
    {
        MirrorManager.OnChargingCollapse -= UI_Behavior_Lerp;
        MirrorManager.OnChargedCollapse -= UI_Behavior_InitiateEaseIn;
        MirrorManager.OnAbortCollapse -= UI_Behavior_Deactivate;
        MirrorManager.OnCollapsing -= UI_Behavior_ConstantlyUpdatePosition;
        MirrorManager.OnExpand -= UI_Behavior_InitiateEaseOut;

        Tutorial.OnRequestTutorialMasterSupport -= GetSelf;
    }
    UIController GetSelf()
    {
        return this;
    }

    private void Awake()
    {
        InitializeArrow();
        InitializeWASD();
    }
    private void Start()
    {
     
        
    }
    private void InitializeWASD() 
    {
        wasd[0] = Instantiate(Resources.Load<GameObject>("UI/P_W"));
        wasd[1] = Instantiate(Resources.Load<GameObject>("UI/P_D"));
        wasd[2] = Instantiate(Resources.Load<GameObject>("UI/P_S"));
        wasd[3] = Instantiate(Resources.Load<GameObject>("UI/P_A"));

     

        for (int i = 0; i < 4; i++) 
        {
            wasdRenderers[i] = wasd[i].GetComponent<MeshRenderer>();
            wasd[i].SetActive(false);
            wasdMPB[i] = new MaterialPropertyBlock();
        }
          

    }

    private void InitializeArrow()
    {
        GameObject arrowObj = Resources.Load<GameObject>("UI/P_CornerUI");
        for (int i = 0; i < 4; i++)
        {
            arrows[i] = Instantiate(arrowObj, Vector3.zero, Quaternion.Euler(new Vector3(0, i * 90f, 0)));

            arrows[i].gameObject.SetActive(false);
            arrowRenders[i] = arrows[i].GetComponent<MeshRenderer>();
            arrowMaterialPropertyBlock[i] = new MaterialPropertyBlock();
        }
    }

    public IEnumerator UI_EaseInOut(float time,bool easein, bool onlyAnimateMaterial )
    {
        float percent = 0;

        float originalMargin = easein ? transitionMargin : focusedMargin;
        float targetMargin = easein ? focusedMargin : expandedMargin;

        float originalAlpha = easein ? 0f : fullMirrorAlpha;
        float targetAlpha = easein ? fullMirrorAlpha: 0f;
        if (easein)
            for (int i = 0; i < 4; i++)
                arrows[i].SetActive(true);


        while (percent < 1)
        {
            percent += Time.deltaTime / time;


            mirrorMargin = Mathf.Lerp(originalMargin, targetMargin, percent);
            Vector3[] corners = Util_GetMirrorCorner(LevelManager._HoodMirrors, 0, mirrorMargin);

            if (!onlyAnimateMaterial)
            for (int i = 0; i < 4; i++)
                    arrows[i].transform.position = Util_ClampToScreenBound(corners[i],screenMargin);
            if (!easein)
            {
                for (int i = 0; i < 4; i++)
                {
                    float alpha = Mathf.Lerp(originalAlpha, targetAlpha, percent);
                    arrowMaterialPropertyBlock[i].SetColor("_BaseColor", new Color(UIColor.r, UIColor.g, UIColor.b, alpha));
                    arrowRenders[i].SetPropertyBlock(arrowMaterialPropertyBlock[i]);
                }
            }


            yield return null;
        }

;       if (!easein) 
            for (int i = 0; i < 4; i++)
                arrows[i].SetActive(false);

    }
    public Vector3[] Util_GetMirrorCorner(Mirror[]mirrors, float yValue,float margin)
    {
        if (mirrors == null)
            return null;
        Vector3[] corners = new Vector3[4];
        // x:top, y:right, z: bottom , w: left 
        Vector4 boundsRegion = new Vector4(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
        foreach (Mirror m in  mirrors)
        {
            if (m == null)
                continue;
            Bounds frameBounds = m.FrameRenderer.bounds;
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
    public Vector3 Util_ClampToScreenBound(Vector3 target,float margin) 
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
    private void UI_Behavior_Lerp(float timer, float target) 
    {
        if (!canDoUIAnime)
            return;
        if (LevelManager._HoodMirrors.Length <= 1)
            return;

        mirrorMargin = Mathf.Lerp(expandedMargin, transitionMargin, timer / target) ;
       Vector3[]  corners = Util_GetMirrorCorner(LevelManager._HoodMirrors, 2, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[ i].transform.position = Util_ClampToScreenBound(corners[i],screenMargin);
            float alpha = Mathf.Lerp(0, 1, timer / target) * fullMirrorAlpha;
            arrowMaterialPropertyBlock[i].SetColor("_BaseColor", new Color(UIColor.r, UIColor.g, UIColor.b, alpha));
            arrowRenders[i].SetPropertyBlock(arrowMaterialPropertyBlock[i]);

        }
    }
    private void UI_Behavior_InitiateEaseIn(float timer, float target)
    {
        if (!canDoUIAnime)
            return;
        if (LevelManager._HoodMirrors.Length <= 1)
            return;

        Vector3[] corners = Util_GetMirrorCorner(LevelManager._HoodMirrors, 2, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[i].transform.position = Util_ClampToScreenBound(corners[i], screenMargin) ;

           
            StartUIEaseInOut(1f, true,false);

        }
    }

    private void UI_Behavior_Deactivate(float timer, float target, bool disableOverrite) 
    {
        if (!canDoUIAnime)
            return;
        if (LevelManager._HoodMirrors.Length <= 1 && !disableOverrite)
            return;
            for (int i = 0; i < 4; i++)
                arrows[i].gameObject.SetActive(false);

    }
    private void UI_Behavior_ConstantlyUpdatePosition() 
    {
        if (!canDoUIAnime)
            return;
        if (LevelManager._HoodMirrors.Length <= 1)
            return;
        ConstantlyUpdatingUIPos(mirrorMargin,screenMargin);
    }
    private void UI_Behavior_InitiateEaseOut(bool onlyUpdateMaterial)
    {
        if (!canDoUIAnime)
            return;
        if (LevelManager._HoodMirrors.Length <= 1)
            return;

        for (int i = 0; i < 4; i++)
        {
            StartUIEaseInOut(0.2f, false, onlyUpdateMaterial);
        }

    }
    private void StartUIEaseInOut(float time, bool easein, bool onlyEaseInMaterial)
    {
        if (!canDoUIAnime)
            return;
        if (easeInCo != null)
            StopCoroutine(easeInCo);
        easeInCo = StartCoroutine(UI_EaseInOut(time, easein, onlyEaseInMaterial));
    }

    public void ConstantlyUpdatingUIPos(float mirrorMargin, float screenMargin) 
    {
        Vector3[] corners = Util_GetMirrorCorner(LevelManager._HoodMirrors, 2, mirrorMargin);

        for (int i = 0; i < 4; i++)
        {

            arrows[i].gameObject.SetActive(true);
            arrows[i].transform.position = Util_ClampToScreenBound(corners[i], screenMargin);

        }
    }



    public class ArrowData 
    {
        public float lerpValue = 0;
        public float alphaMultiplier = 1;
        public MaterialPropertyBlock mpb;

   
        public async void RadiusOffsetIncreaseTo(float value, float speed) 
        {
            while (lerpValue < value) 
            {
                 lerpValue += Time.deltaTime * speed;
                await Task.Yield();
            }
        }
        

    }

    public void SyncOtherStuffWithArrow(Vector3[] arrowPosition, float[] arrowAlpha, Vector3 center) 
    {
        for (int i = 0; i < 4; i++) 
        {
            wasd[i].SetActive(true);
            wasd[i].transform.position = arrowPosition[i] + (center - arrowPosition[i]).normalized * 0.35f;

            wasdMPB[i].SetColor("_BaseColor", new Color(UIColor.r, UIColor.g, UIColor.b, arrowAlpha[i]));
            wasdRenderers[i].SetPropertyBlock(wasdMPB[i]);
        }
    }
    public void ResetArrowDatas(ArrowData[] arrowDatas)
    {
        for (int i = 0; i < 4; i++)
        {
            
            arrowDatas[i].mpb.SetColor("_BaseColor", UIColor);
            arrowRenders[i].SetPropertyBlock(arrowDatas[i].mpb);
            arrowDatas[i].lerpValue = 0;
            arrowDatas[i].alphaMultiplier = 1;
            arrows[i].SetActive(false);

        }

    }

    public void ResetWASD()
    {
        for (int i = 0; i < 4; i++)
        {
            wasd[i].SetActive(false);
           
            wasdMPB[i].SetColor("_BaseColor", UIColor);
            wasdRenderers[i].SetPropertyBlock(wasdMPB[i]);
        }
    }

    public void GroupControlArrows(Vector3 centerPosition, float radius, float offsetDegree,float materialAlpha, bool setActive, ArrowData[] arrowDatas, Action<Vector3[],float[], Vector3> moveOtherStuff)
    {
        float initialOffset = offsetDegree;
        float[] alphas = new float[4];
        for (int i = 0; i < 4; i++) 
        {
            float sin = Mathf.Sin(Mathf.Deg2Rad* initialOffset);
            float cos = Mathf.Cos(Mathf.Deg2Rad* initialOffset);
            Vector3 targetPos = centerPosition +  new Vector3 (sin, 0, cos);
            Vector3 outwardDir = (centerPosition - targetPos).normalized;
            
            arrows[i].transform.position = Util_ClampToScreenBound(centerPosition + new Vector3(sin, 0, cos) * (radius + arrowDatas[i].lerpValue),screenMargin) ;
            arrows[i].transform.forward = outwardDir;
            arrows[i].transform.Rotate(new Vector3(0, setActive? 45f:0f, 0));
            arrows[i].SetActive(setActive);

            initialOffset += 90f;
            alphas[i] = Mathf.Max((materialAlpha - arrowDatas[i].lerpValue * arrowDatas[i].alphaMultiplier) * fullMirrorAlpha,0) ;
            Color matCol = new Color(UIColor.r, UIColor.g, UIColor.b, alphas[i]);
            arrowDatas[i].mpb.SetColor("_BaseColor", matCol);
            arrowRenders[i].SetPropertyBlock(arrowDatas[i].mpb);
        }
        moveOtherStuff?.Invoke(arrows.Select(x => x.transform.position).ToArray(),alphas, centerPosition);
    }


    

    public IEnumerator MoveArrowsAsGroup(Vector3 privousCenterPosition, Transform targetTransform, float previousRadius, float targetRadius, float offsetDegree, float previousMaterialAlpha, float targetMaterialAlpha, float timeToComplete, 
        ArrowData[] arrowDatas, AnimationCurve curve, Action<Vector3[], float[], Vector3> MoveOtherStuff, Action<UIController> next) 
    {
        float time = 0;
        while (time < timeToComplete) 
        {
            time += Time.deltaTime;
            float percentage = time / timeToComplete;
            float interpolate = curve.Evaluate(percentage);
            float alpha = Mathf.Lerp(previousMaterialAlpha, targetMaterialAlpha,interpolate) ;
            Vector3 centerPosition = Vector3.Lerp(privousCenterPosition, targetTransform.position,interpolate);
            float radius = Mathf.Lerp(previousRadius, targetRadius, interpolate);
            GroupControlArrows(centerPosition, radius, offsetDegree,alpha,true,arrowDatas,MoveOtherStuff);

            yield return null;

        }
        next?.Invoke(this);
    }

    public IEnumerator ArrowsFollowObject(Transform targetTransform , float targetRadius, float offsetDegree, float targetMaterialAlpha, ArrowData[] arrowDatas, Func<bool> condition, Action<Vector3[], float[], Vector3> moveOtherStuff, Action<UIController> next)
    {
 
        while (condition.Invoke())
        {
            GroupControlArrows(targetTransform.position, targetRadius, offsetDegree, targetMaterialAlpha, true, arrowDatas,moveOtherStuff);

            yield return null;

        }
        next?.Invoke(this);

    }


    private void OnDrawGizmos()
    {
        if (!cam)
            return;
        Gizmos.color = Color.yellow;
        Vector3[] corners = Util_GetMirrorCorner(LevelManager._HoodMirrors, 2,mirrorMargin);
        if (corners == null)
            return;
        foreach (Vector3 corner in corners) 
        {
            Gizmos.DrawSphere( Util_ClampToScreenBound(corner, screenMargin) , 0.1f);
        }
    }
}
