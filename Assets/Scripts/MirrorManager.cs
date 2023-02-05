using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.AI;
using System;
using System.Diagnostics.Tracing;

public class MirrorManager : MonoBehaviour
{
    public Mirror mirror;

    private Mirror currentMirror,hoodMirror;
    public LayerMask mirrorMask;
    private Vector3 offset, futurePosition, currentPosition,closestBoundPos;

    private bool hasBeenClicked = false,canDrag = true;

    public static Func<bool> OnCheckingSlidable;

    private Bounds bound;

    SpringJoint joint;


    private void OnEnable()
    {
    }
    private void OnDisable()
    {


    }
    void CheckMouseDown()
    {

        if (!Input.GetMouseButton(0)) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] allHits;
        allHits = Physics.RaycastAll(ray, Mathf.Infinity, mirrorMask);

        Vector3 finalWorldPos = Vector3.zero;
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
                    bound = currentMirror. GetComponent<Collider>().bounds;
                   // joint = new SpringJoint();
                    offset = singleHit.transform.position - singleHit.point;

                     
                }
            }
        }
        if (currentMirror) 
        {
            currentPosition = currentMirror.transform.position;
            futurePosition = finalWorldPos + offset;
            Vector3 direction = Vector3.Normalize( futurePosition - currentPosition);
            float distance = (futurePosition - currentPosition).magnitude;
            /*joint.connectedBody = currentMirror.GetComponent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = true;
            joint.connectedAnchor = futurePosition;*/
            currentMirror.GetComponent<Rigidbody>().AddForce(direction *30 * distance, ForceMode.Force);

            //BoxCollider[] boxs = currentMirror. GetComponents<BoxCollider>();

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

    void CheckMouseUp() 
    {
        if (Input.GetMouseButtonUp(0))
        {
            currentMirror = null;
            hasBeenClicked = false;
        }
    }
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        if (canDrag)
        {
            CheckMouseDown();

        }
     

    }

    void Update()
    {
        CheckMouseUp();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(closestBoundPos, 0.1f);
    }
}
