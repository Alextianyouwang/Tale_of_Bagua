using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.AI;
using System;

public class MirrorManager : MonoBehaviour
{
    public GameObject mirror;

    private GameObject currentMirror;
    public LayerMask mirrorMask;
    private Vector3 offset;

    private bool hasBeenClicked = false;

    public static Func<bool> OnCheckingSlidable;
    
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
                    currentMirror = singleHit.transform.gameObject;
                    offset = singleHit.transform.position - singleHit.point;
                }
                
            }
        }
        if (currentMirror && hasBeenClicked) 
        {
            if (!OnCheckingSlidable.Invoke()) 
                currentMirror.transform.position = finalWorldPos + offset;
           
        }
           
    }

    void CheckMouseUp() 
    {
        if (Input.GetMouseButtonUp(0))
        {
            if(currentMirror != null) currentMirror = null;
            hasBeenClicked = false;
        }
    }
    void Start()
    {
        
    }


    void Update()
    {
        CheckMouseDown();
        CheckMouseUp();

    }
}
