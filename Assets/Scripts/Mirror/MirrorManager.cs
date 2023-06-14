using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using TMPro.EditorUtilities;

public class MirrorManager : MonoBehaviour
{
    private Mirror currentMirror;
    public LayerMask mirrorMask;
    private Vector3 offset, finalWorldPos;
    private bool hasBeenClicked = false, startDraging = false, isCollapsed = false;
    private Mirror[] hoodMirrors;
    private List<Mirror> closestToCurrent = new List<Mirror>();
    public static Func<bool> OnCheckingSlidable;
    public static Action<Mirror> OnSharingCurrentMirror;

    private void OnEnable()
    {
        LayerCheck.OnShareHoodMirror += ReceiveHoodMirror;
    }
    private void OnDisable()
    {
        LayerCheck.OnShareHoodMirror -= ReceiveHoodMirror;
    }
    void ReceiveHoodMirror(Mirror[] hoodMirror)
    {
        hoodMirrors = hoodMirror;
    }
    void UpdateMirrorPhysics()
    {
        if (!startDraging) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] allHits = Physics.RaycastAll(ray, Mathf.Infinity, mirrorMask);
        Mirror[] selecetedMirrors = allHits.Select(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        foreach (RaycastHit singleHit in allHits)
        {
            if (singleHit.transform.gameObject.tag == "MirrorPlane")
                finalWorldPos = singleHit.point;

            if (singleHit.transform.gameObject.tag == "Mirror")
            {
                if (!hasBeenClicked)
                {
                    hasBeenClicked = true;
                    currentMirror = singleHit.transform.gameObject.GetComponent<Mirror>();
                    offset = currentMirror.transform.position - singleHit.point;
                    offset.y = 0;
                }
                else
                    continue;
            }
        }
    }
    public void UpdateMirrorPosition(Mirror targetMirror)
    {
        Vector3 posXZ = new Vector3(targetMirror.transform.position.x, finalWorldPos.y, targetMirror.transform.position.z);
        Vector3 direction = Vector3.Normalize(finalWorldPos - (posXZ - offset));
        float distance = (finalWorldPos - (posXZ - offset)).magnitude;
        targetMirror.GetComponent<Rigidbody>().AddForce(direction * Mathf.Min(distance * 2, 5), ForceMode.Force);

    }
    private void FixedUpdate()
    {

        UpdateMirrorPhysics();
        if (!currentMirror || !hasBeenClicked)
            return;
        if (isCollapsed)
        {
            foreach (Mirror m in hoodMirrors)
            {
                UpdateMirrorPosition(m);
                m.ToggleBoxesRigidCollider(true);
            }
        }
        else
        {
            UpdateMirrorPosition(currentMirror);
        }
        
        //HoodMirrorStick();

    }

    public void CollapseHoodMirror()
    {
        if (hoodMirrors.Length <=1)
            return;
        isCollapsed = true;
        Vector3 playerPos = PlayerMove.playerTransform.position;
        Vector3 targetPos = new Vector3(playerPos.x, hoodMirrors[0].transform.position.y, playerPos.z);
        foreach (Mirror m in hoodMirrors)
        {
            m.MoveMirrorTowards(1f, targetPos);

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

            hoodMirrors[i].MoveMirrorTowards(1f, targetPos);

        }

    }
    void Update()
    {
        startDraging = Input.GetMouseButton(0);
        if (Input.GetMouseButtonUp(0))
        {
            currentMirror = null;
            hasBeenClicked = false;
            offset = Vector3.zero;
            
        }
        OnSharingCurrentMirror?.Invoke(currentMirror);
        // Maybebug
        if (!currentMirror || !hasBeenClicked)
            return;
        if (!hoodMirrors.Contains(currentMirror) || LayerCheck.isPlayerOnLastLevel)
            currentMirror.ToggleBoxesRigidCollider(OnCheckingSlidable.Invoke());
     
    }

    private void HoodMirrorStick()
    {
        if (!currentMirror || !hasBeenClicked) 
            return;

        //closestToCurrent.Clear();
        //float closest = float.MaxValue;
        foreach (Mirror m in hoodMirrors)
        {
            m.distanceToCurrentMirror = Vector3.Distance(m.transform.position, currentMirror.transform.position);
            if (m.distanceToCurrentMirror < 0.5f)
            {
                if (currentMirror != m)
                    UpdateMirrorPosition(m);
            }

        }

        /* float closest = float.MaxValue;
         Mirror closestMirror = null;
         foreach (Mirror m in closestToCurrent) 
         {
            m.distanceToCurrentMirror = Vector3.Distance(m.transform.position, currentMirror.transform.position);
             if (m.distanceToCurrentMirror < closest && m != currentMirror)
             {
                 closest = m.distanceToCurrentMirror;
                 closestToCurrent.Remove(closestMirror) ;
             }

         }
         if (!closestMirror)
             return;*/
        // closestToCurrent.Add(closestMirror);

        /*if (closestToCurrent[0]?.distanceToCurrentMirror < 0.5f)
        {
            if (currentMirror != closestToCurrent[0])
                UpdateMirrorPosition(closestToCurrent[0]);
        }*/
        /*foreach (Mirror m in closestToCurrent)
        {
            if (!m)
                continue;
            if (m.distanceToCurrentMirror < 0.5f)
            {
                if (currentMirror != m)
                    UpdateMirrorPosition(m);
            }
        }*/

        // print(closestToCurrent?.distanceToCurrentMirror);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(currentMirror)
        Gizmos.DrawLine(finalWorldPos, currentMirror.transform.position - offset);
    }
}
