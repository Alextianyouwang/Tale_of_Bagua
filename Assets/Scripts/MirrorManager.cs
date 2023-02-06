using UnityEngine;
using System;
using System.Linq;
using System.Net.NetworkInformation;

public class MirrorManager : MonoBehaviour
{
    public Mirror mirror;

    private Mirror currentMirror;
    public LayerMask mirrorMask;
    private Vector3 offset,finalPos;
    

    private bool hasBeenClicked = false, startDraging = false;

    private Mirror[] hoodMirrors;
    public static Func<bool> OnCheckingSlidable;


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
    void CheckMouseDown()
    {

        if (!startDraging) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] allHits;
        allHits = Physics.RaycastAll(ray, Mathf.Infinity, mirrorMask);
        Mirror[] selecetedMirrors = new Mirror[allHits.Length];
        Vector3 finalWorldPos = Vector3.zero;
        selecetedMirrors = allHits.Select(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        foreach (RaycastHit singleHit in allHits) 
        {
            if (singleHit.transform.gameObject.tag == "MirrorPlane")
            {
               
                finalWorldPos = singleHit.point + Vector3.up * 0.1f;
                finalPos = finalWorldPos;
            }
            if (singleHit.transform.gameObject.tag == "Mirror") 
            {
                if (!hasBeenClicked)
                {
                    hasBeenClicked = true;
                    currentMirror = singleHit.transform.gameObject.GetComponent<Mirror>();
                    
                    offset = currentMirror.transform.position - singleHit.point;
                }
                else
                    continue;
            }
        }
        if (currentMirror && hasBeenClicked) 
        {
            Vector3 direction = Vector3.Normalize(finalWorldPos-(currentMirror.transform.position - offset));
            float distance = (finalWorldPos - (currentMirror.transform.position - offset)).magnitude;
            
            currentMirror.GetComponent<Rigidbody>().AddForce(direction*20 * distance, ForceMode.Force);

            bool canPerformControl = true;
            foreach (Mirror m in hoodMirrors) 
            {
                if (currentMirror == m)
                    canPerformControl = false;
            }
            if (canPerformControl) 
            {
                if (OnCheckingSlidable.Invoke())
                {
                    currentMirror.ToggleBoxesRigidCollider(true);
                }
                else
                {
                    currentMirror.ToggleBoxesRigidCollider(false);
                }
            }
          
        }
    }
    private void FixedUpdate()
    {
            CheckMouseDown();
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(finalPos, 0.5f);
    }
}
