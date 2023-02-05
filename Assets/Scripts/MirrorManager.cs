using UnityEngine;
using System;


public class MirrorManager : MonoBehaviour
{
    public Mirror mirror;

    private Mirror currentMirror;
    public LayerMask mirrorMask;
    private Vector3 offset;

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

        Vector3 finalWorldPos = Vector3.zero;
        Vector3 hitPoint = Vector3.zero;
        foreach (RaycastHit singleHit in allHits) 
        {
           
            if (singleHit.transform.gameObject.tag != "Mirror")
            {
                finalWorldPos = singleHit.point + Vector3.up * 0.1f;
            }
            if (singleHit.transform.gameObject.tag == "Mirror") 
            {
                if (!hasBeenClicked)
                {
                    hasBeenClicked = true;
                    currentMirror = singleHit.transform.gameObject.GetComponent<Mirror>();
                    offset = singleHit.transform.position - singleHit.point;
                    hitPoint = singleHit.point;
                }
            }
        }
        if (currentMirror) 
        {
            Vector3 direction = Vector3.Normalize(finalWorldPos - (currentMirror.transform.position - offset));
            float distance = (finalWorldPos - (currentMirror.transform.position - offset)).magnitude;
            
            currentMirror.GetComponent<Rigidbody>().AddForce(direction*50 * distance, ForceMode.Force);

            foreach (Mirror m in hoodMirrors) 
            {
                if (currentMirror == m)
                    return;
            }

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
}
