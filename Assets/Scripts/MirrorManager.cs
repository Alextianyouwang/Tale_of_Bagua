using UnityEngine;
using System;
using System.Linq;

public class MirrorManager : MonoBehaviour
{
    private Mirror currentMirror;
    public LayerMask mirrorMask;
    private Vector3 offset, finalWorldPos;
    private bool hasBeenClicked = false, startDraging = false;
    private Mirror[] hoodMirrors;
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
                finalWorldPos = singleHit.point + Vector3.up * 0.1f;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(currentMirror)
        Gizmos.DrawLine(finalWorldPos, currentMirror.transform.position - offset);
    }
}
