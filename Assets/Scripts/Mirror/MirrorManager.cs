using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Experimental.AI;

public class MirrorManager : MonoBehaviour
{
    private Mirror currentMirror;
    public LayerMask mirrorMask;
    private Vector3 offset, finalWorldPos;
    private bool firstMirrorHasBeenClicked = false, isClicking = false, isCollapsed = false;
    private Mirror[] hoodMirrors,allMirrors;
    public static Func<bool> OnCheckingSlidable;
    public static Action<Mirror> OnSharingCurrentMirror;

    public static Action<float,float> OnChargingCollapse;
    public static Action<float,float> OnChargingRelease;
    public static Action<float,float> OnChargedCollapse;
    public static Action<float,float,bool> OnAbortCollapse;
    public static Action OnCollapsing;
    public static Action OnExpand;

    private float collapseTimer = 0,chargeTime = 0.5f;

    public AnimationCurve mirrorMoveCurve;

    [ColorUsage(true,true)]
    public Color normalCol;
    [ColorUsage(true, true)]
    public Color selectedCol;
    [ColorUsage(true, true)]
    public Color hoodCol;
    

    private void OnEnable()
    {
        LayerCheck.OnShareHoodMirror += ReceiveHoodMirror;
        LayerCheck.OnShareAllMirror += ReceiveAllMirror;
        LayerCheck.OnFixUpdate += FollowFixUpdate;
    }
    private void OnDisable()
    {
        LayerCheck.OnShareHoodMirror -= ReceiveHoodMirror;
        LayerCheck.OnFixUpdate -= FollowFixUpdate;
        LayerCheck.OnShareAllMirror -= ReceiveAllMirror;


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
        Mirror[] selecetedMirrors = allHits.Select(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        foreach (RaycastHit singleHit in allHits)
        {
            if (singleHit.transform.gameObject.tag == "MirrorPlane")
                finalWorldPos = singleHit.point;

            if (singleHit.transform.gameObject.tag == "Mirror")
            {
                if (!firstMirrorHasBeenClicked)
                {
                    firstMirrorHasBeenClicked = true;
                    currentMirror = singleHit.transform.gameObject.GetComponent<Mirror>();
                    offset = currentMirror.transform.position - singleHit.point;
                    offset.y = 0;
                }
                else
                    continue;
            }
        }
    }
    public void UpdateMirrorPosition(Mirror targetMirror,float speed)
    {
        Vector3 posXZ = new Vector3(targetMirror.transform.position.x, finalWorldPos.y, targetMirror.transform.position.z);
        Vector3 direction = Vector3.Normalize(finalWorldPos - (posXZ - offset));
        float distance = (finalWorldPos - (posXZ - offset)).magnitude;
        targetMirror.GetComponent<Rigidbody>().AddForce(direction * Mathf.Min(distance * speed, 5), ForceMode.Force);

    }

    private void FollowFixUpdate() 
    {
        UpdateMirrorPhysics();

        if (!currentMirror || !firstMirrorHasBeenClicked)
            return;

        float speedinc = 0;
        if (isCollapsed && hoodMirrors.Contains(currentMirror))
        {
           
            for (int i = 0; i < hoodMirrors.Length; i++)
            {
                Mirror m = hoodMirrors[i];
                //speedinc += 0.3f;
                m.ToggleBoxesRigidCollider(true);

                UpdateMirrorPosition(m, 2 - speedinc);
            }
        }
        else if (!isCollapsed)
        {

            UpdateMirrorPosition(currentMirror, 2 - speedinc);

        }
    }
    private void StopCollapseBuffer() 
    {
        foreach (Mirror m in hoodMirrors)
        {
            m.AbortMovement();
        }
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

    void UpdateInput() 
    {
        if (isCollapsed&& !Input.GetMouseButton(1))
            OnCollapsing?.Invoke();

        if (Input.GetMouseButtonDown(0))
        {
            isClicking = true;
            StopCollapseBuffer();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isClicking = false;
            currentMirror = null;
            firstMirrorHasBeenClicked = false;
            offset = Vector3.zero;
            if (isCollapsed)
                CollapseHoodMirror();
        }

        if (Input.GetMouseButton(1)) 
        {
            collapseTimer += Time.deltaTime;
            if (!isCollapsed)
                OnChargingCollapse?.Invoke(collapseTimer, chargeTime);
            else
                OnChargingRelease?.Invoke(collapseTimer, chargeTime);

        }
        if (Input.GetMouseButtonUp(1)) 
        {

            if (collapseTimer >= chargeTime)
            {
                OnChargedCollapse?.Invoke(collapseTimer, chargeTime);
                if (!isCollapsed)
                    CollapseHoodMirror();
                else 
                {
                    ExpandHoodMirror();
                    OnExpand?.Invoke();
                }
                
            }
            else 
            {
                OnAbortCollapse?.Invoke(collapseTimer, chargeTime,isCollapsed);
              
                   
            }
            collapseTimer = 0;
        }

        
        if (hoodMirrors.Length == 0 && isCollapsed)
        {
            isCollapsed = false;
        }
        OnSharingCurrentMirror?.Invoke(currentMirror);

        if (!currentMirror || !firstMirrorHasBeenClicked)
            return;
        if (!hoodMirrors.Contains(currentMirror) || LayerCheck.isPlayerOnLastLevel)
            currentMirror.ToggleBoxesRigidCollider(OnCheckingSlidable.Invoke());

    }

    void SetMirrorColor(Mirror m, Color color) 
    {
        m.material[2].color = color;  
    }

  
    void UpdateMaterial() 
    {
        foreach (Mirror m in allMirrors)
            if (m)
                SetMirrorColor(m, normalCol);

        foreach (Mirror m in hoodMirrors)
            if (m)
                SetMirrorColor(m, hoodCol);
        if (currentMirror)
            SetMirrorColor(currentMirror, selectedCol);
    }
    void Update()
    {
        UpdateInput();
         UpdateMaterial();
   
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(currentMirror)
        Gizmos.DrawLine(finalWorldPos, currentMirror.transform.position - offset);
    }
}
