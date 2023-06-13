using UnityEngine;
using System;
<<<<<<< HEAD

=======
using System.Linq;
>>>>>>> BeforeJamBuild

public class MirrorManager : MonoBehaviour
{
    private Mirror currentMirror;
    public LayerMask mirrorMask;
<<<<<<< HEAD
    private Vector3 offset;

=======
    private Vector3 offset, finalWorldPos;
>>>>>>> BeforeJamBuild
    private bool hasBeenClicked = false, startDraging = false;
    private Mirror[] hoodMirrors;
    public static Func<bool> OnCheckingSlidable;
<<<<<<< HEAD

=======
    public static Action<Mirror> OnSharingCurrentMirror;
>>>>>>> BeforeJamBuild
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
<<<<<<< HEAD

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
=======
        RaycastHit[] allHits = Physics.RaycastAll(ray, Mathf.Infinity, mirrorMask);
        Mirror[] selecetedMirrors = allHits.Select(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        foreach (RaycastHit singleHit in allHits) 
        {
            if (singleHit.transform.gameObject.tag == "MirrorPlane")
                finalWorldPos = singleHit.point + Vector3.up * 0.1f;

>>>>>>> BeforeJamBuild
            if (singleHit.transform.gameObject.tag == "Mirror") 
            {
                if (!hasBeenClicked)
                {
                    hasBeenClicked = true;
                    currentMirror = singleHit.transform.gameObject.GetComponent<Mirror>();
<<<<<<< HEAD
                    offset = singleHit.transform.position - singleHit.point;
                    hitPoint = singleHit.point;
=======
                    offset = currentMirror.transform.position - singleHit.point;
>>>>>>> BeforeJamBuild
                }
            }
        }
<<<<<<< HEAD
        if (currentMirror) 
        {
            Vector3 direction = Vector3.Normalize(finalWorldPos - (currentMirror.transform.position - offset));
            float distance = (finalWorldPos - (currentMirror.transform.position - offset)).magnitude;
            
            currentMirror.GetComponent<Rigidbody>().AddForce(direction*50 * distance, ForceMode.Force);

            bool canUpdateCurrentMirror = true;
            foreach (Mirror m in hoodMirrors) 
            {
                if (currentMirror == m)
                    canUpdateCurrentMirror = false;
            }
            if (hoodMirrors.Length == 5)
                canUpdateCurrentMirror = true;

            if (canUpdateCurrentMirror)
            if (OnCheckingSlidable.Invoke())
            {
                currentMirror.ToggleBoxesRigidCollider(true);
            }
            else 
            {
                currentMirror.ToggleBoxesRigidCollider(false);
            }
        }
=======
>>>>>>> BeforeJamBuild
    }
    private void FixedUpdate()
    {
        UpdateMirrorPhysics();
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
        if (currentMirror && hasBeenClicked)
        {
            Vector3 direction = Vector3.Normalize(finalWorldPos - (currentMirror.transform.position - offset));
            float distance = (finalWorldPos - (currentMirror.transform.position - offset)).magnitude;
            currentMirror.GetComponent<Rigidbody>().AddForce(direction * 10 * distance, ForceMode.Force);
            if (!hoodMirrors.Contains(currentMirror) ||LayerCheck.isPlayerOnLastLevel)
                currentMirror.ToggleBoxesRigidCollider(OnCheckingSlidable.Invoke());
        }
        OnSharingCurrentMirror?.Invoke(currentMirror);
    }
<<<<<<< HEAD
=======

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(currentMirror)
        Gizmos.DrawLine(finalWorldPos, currentMirror.transform.position - offset);
    }
>>>>>>> BeforeJamBuild
}
