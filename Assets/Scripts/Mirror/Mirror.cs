using System.Collections;
using UnityEngine;
using System;

public class Mirror : MonoBehaviour
{
    private Collider[] boxs;
    private bool colliderToggleState = false;
    [HideInInspector]
    public float distanceToCurrentMirror;
    private bool isMovingTowards = false;
    private Coroutine movingCo;

    public Action OnFinishMoving;
    void Start()
    {
        GetBoxs();
    }

    public void MoveMirrorTowards(float time, Vector3 targetPos) 
    {
        if (isMovingTowards)
            return;
        movingCo = StartCoroutine(MoveTowards(time, targetPos));

    }

    public void AbortMovement() 
    {
        isMovingTowards = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        if (movingCo != null)
            StopCoroutine(movingCo);
    }
    private IEnumerator MoveTowards(float time,Vector3 targetPos) 
    {
        float percent = 0;
        Vector3 originalPos = transform.position;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        isMovingTowards = true;
        while ( percent < 1) 
        {
            percent += Time.deltaTime/time;
            transform.position = Vector3.Lerp(originalPos, targetPos, percent);
            yield return null;
        }
        isMovingTowards = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        OnFinishMoving?.Invoke();

    }
    void GetBoxs() 
    {
        boxs = GetComponents<Collider>();
        foreach (Collider b in boxs)
            b.isTrigger = true;
    }
   
    public void ToggleBoxesRigidCollider(bool value) 
    {
        if (colliderToggleState == value)
            return;
        colliderToggleState = value;
        foreach (Collider b in boxs)
            b.isTrigger = !value;
    }

    
}
