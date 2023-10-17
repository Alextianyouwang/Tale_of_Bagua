using UnityEngine;
using System;
using System.Linq;
using System.Net.NetworkInformation;

public class MirrorManager : MonoBehaviour
{
    private Mirror currentMirror;
    public LayerMask mirrorMask;
    private Vector3 offset, finalWorldPos, screenCenter_WorldSpace;
    private bool firstMirrorHasBeenClicked = false, isClicking = false, isCollapsed = false, isCharged = false, canChargeAgain = true;
    private Mirror[] hoodMirrors,allMirrors;
    public static Action<Mirror> OnSharingCurrentMirror;

    public static Action<float,float> OnChargingCollapse;
    public static Action<float,float> OnChargingRelease;
    public static Action<float,float> OnChargedCollapse;
    public static Action<float,float,bool> OnAbortCollapse;
    public static Action OnCollapsing;
    public static Action<bool> OnExpand;

    private float collapseTimer = 0,chargeTime = 0.01f;

    public AnimationCurve mirrorMoveCurve;

    private int previousHoodMirrorCount = 0,currentHoodMirrorCount = 0; 

    public static bool canUseRightClick = true, canUseLeftClick = true;

    [ColorUsage(true,true)]
    public Color normalCol;
    [ColorUsage(true, true)]
    public Color selectedCol;
    [ColorUsage(true, true)]
    public Color hoodCol;
    

    private void OnEnable()
    {
        screenCenter_WorldSpace = Utility.GetScreenCenterPosition_WorldSpace();
        LevelManager.OnShareHoodMirror += ReceiveHoodMirror;
        LevelManager.OnShareAllMirror += ReceiveAllMirror;
        LevelManager.OnFixUpdate += FollowFixUpdate;
    }
    private void OnDisable()
    {
        LevelManager.OnShareHoodMirror -= ReceiveHoodMirror;
        LevelManager.OnFixUpdate -= FollowFixUpdate;
        LevelManager.OnShareAllMirror -= ReceiveAllMirror;

        canUseRightClick = true;
        canUseLeftClick= true;
    }
    void ReceiveHoodMirror(Mirror[] hoodMirror)
    {
        hoodMirrors = hoodMirror;
    }

    void ReceiveAllMirror(Mirror[] allMirror) 
    {
        allMirrors = allMirror;
        
    }

    void UpdateMirrorPhysics()
    {
        if (!isClicking) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] allHits = Physics.RaycastAll(ray, Mathf.Infinity, mirrorMask);
        finalWorldPos = allHits.Where(x => x.transform.tag.Equals("MirrorPlane")).FirstOrDefault().point;
        if (!firstMirrorHasBeenClicked)
        {
            firstMirrorHasBeenClicked = true;
            currentMirror = allHits.Where(x => x.transform.tag.Equals("Mirror")).Select(x => x.transform.gameObject.GetComponent<Mirror>()).FirstOrDefault();
            if (currentMirror != null) 
            offset =currentMirror.transform.position - finalWorldPos;
        }
    }
 

    public void MoveMirrorTo(Mirror m, Vector3 target, float speed ) 
    {
        Vector3 mirrorCenter = m.transform.position - offset;
        Vector3 direction = Vector3.Normalize(target - mirrorCenter);
        float distance = (finalWorldPos - mirrorCenter).magnitude;
        m.RigidBodyAddForce(direction, Mathf.Min(distance * speed, 5));
     
    }

    private void SetMirrorYPos() 
    {
        foreach(Mirror m in allMirrors) 
            m.rb.position = new Vector3(m.rb.position.x, screenCenter_WorldSpace.y, m.rb.position.z);
    }

    private void CageMirrorWhenCollapsed() 
    {
        foreach (Mirror m in hoodMirrors)
            if (isCollapsed)
                m.ToggleBoxesRigidCollider(true);
    }


    private void FollowFixUpdate() 
    {
        UpdateMirrorPhysics();
        SetMirrorYPos();
        CageMirrorWhenCollapsed();

        if (!currentMirror || !firstMirrorHasBeenClicked) 
            return;
 
        if (isCollapsed && hoodMirrors.Contains(currentMirror))
            for (int i = 0; i < hoodMirrors.Length; i++)
                MoveMirrorTo(hoodMirrors[i], finalWorldPos, 2);
        else
             MoveMirrorTo(currentMirror, finalWorldPos, 2);
    }

    public void CollapseHoodMirror()
    {
        if (hoodMirrors.Length <=1)
            return;
        isCollapsed = true;
        Vector3 averagePos = Vector3.zero;
        foreach (Mirror m in hoodMirrors)
        {
            averagePos += m.transform.position;

        }
        averagePos /= hoodMirrors.Length;
        foreach (Mirror m in hoodMirrors) 
        {
            m.ToggleBoxesRigidCollider(true);
            m.MoveMirrorTowards(0.4f, averagePos,mirrorMoveCurve);

        }
    }

    public Vector2[] RadiusPosition(int numSlices)
    {
        Vector2[] dirs = new Vector2[numSlices];
        float inc = Mathf.PI * 2 / numSlices;
        for (int i = 0; i < numSlices; i++) 
        {
            dirs[i] = new Vector2(Mathf.Sin(inc*i), Mathf.Cos(i*inc));
        }
        return dirs;
    }
    public void ExpandHoodMirror() 
    {
        if (hoodMirrors.Length <= 1)
            return;
        isCollapsed = false;
        Vector2[] offsets = RadiusPosition(hoodMirrors.Length);
        Vector3 playerPos = PlayerMove.playerTransform.position;

        for (int i = 0; i < hoodMirrors.Length; i++)
        {
            Vector2 offset = offsets[i];
            Vector3 targetPos = new Vector3(playerPos.x + offset.x, hoodMirrors[0].transform.position.y, playerPos.z+ offset.y);
            hoodMirrors[i].MoveMirrorTowards(0.4f, targetPos, mirrorMoveCurve);
        }
    }

    void CheckIfNewHoodMirrorAdded() 
    {
        currentHoodMirrorCount = hoodMirrors.Length;
        if (previousHoodMirrorCount != currentHoodMirrorCount && isCollapsed) 
        {
            CollapseHoodMirror();
        }
        previousHoodMirrorCount = currentHoodMirrorCount;
    }
    void UpdateInput() 
    {

        if (canUseLeftClick) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                isClicking = true;
                
            }
            if (Input.GetMouseButtonUp(0))
            {
    
                isClicking = false;
                currentMirror = null;
                firstMirrorHasBeenClicked = false;
                offset = Vector3.zero;
            }
        }
       


        if (canUseRightClick) 
        {
            if (isCollapsed && !Input.GetMouseButton(1))
                OnCollapsing?.Invoke();
            if (Input.GetMouseButtonDown(1))
            {

                if (isCollapsed)
                {
                    canChargeAgain = false;
                    OnExpand?.Invoke(false);
                    ExpandHoodMirror();

                }

            }
            if (Input.GetMouseButton(1))
            {
                if (!canChargeAgain)
                    return;
                collapseTimer += Time.deltaTime;
                if (!isCollapsed)
                    OnChargingCollapse?.Invoke(collapseTimer, chargeTime);


            }
            if (Input.GetMouseButtonUp(1))
            {
                if (!canChargeAgain)
                {
                    isCharged = false;
                    canChargeAgain = true;
                    return;
                }
                if (collapseTimer >= chargeTime)
                {

                    OnChargedCollapse?.Invoke(collapseTimer, chargeTime);
                    if (!isCollapsed)
                    {
                        CollapseHoodMirror();
                        isCharged = true;
                    }
                }
                else
                {
                    if (!isCharged)
                        OnAbortCollapse?.Invoke(collapseTimer, chargeTime, false);
                }
                collapseTimer = 0;
            }
        }
       

       

    }

    void SetMirrorColor(Mirror m, Color color) 
    {
        m.material[2].color = color;  
    }

  
    void UpdateMaterial() 
    {
        if (allMirrors != null)
        foreach (Mirror m in allMirrors)
                    if (m && m == currentMirror)
                        SetMirrorColor(m, selectedCol);
                    else if (m && m != currentMirror)
                        SetMirrorColor(m, normalCol);
        if (hoodMirrors != null && hoodMirrors.Length > 0) 
        {
            foreach (Mirror mr in hoodMirrors)
                if (mr)
                    SetMirrorColor(mr, hoodCol);
            foreach (Mirror m in hoodMirrors)
                if (m)
                    if (m == currentMirror)
                        SetMirrorColor(m, selectedCol);
                    else if (m && m != currentMirror)
                        SetMirrorColor(m, hoodCol);
            if (isCollapsed && currentMirror)
                foreach (Mirror m in hoodMirrors)
                    if (m)
                        SetMirrorColor(m, selectedCol);
        }
           
   
    }
    void Update()
    {

 
        UpdateInput();
        UpdateMaterial();
        CheckIfNewHoodMirrorAdded();

        OnSharingCurrentMirror?.Invoke(currentMirror);


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(currentMirror)
        Gizmos.DrawLine(finalWorldPos, currentMirror.transform.position - offset);
    }
}
