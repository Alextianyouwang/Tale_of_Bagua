using System.Collections;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    private Collider[] boxs;
    private bool colliderToggleState = false;
    [HideInInspector]
    public float distanceToCurrentMirror;
    private bool isMovingTowards = false;
    private Coroutine movingCo;
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
    private IEnumerator MoveTowards(float time,Vector3 targetPos) 
    {
        float percent = 0;
        Vector3 originalPos = transform.position;
        GetComponent<Rigidbody>().isKinematic = true;
        isMovingTowards = true;
        while ( percent < time) 
        {
            percent += Time.deltaTime;
            transform.position = Vector3.Lerp(originalPos, targetPos, percent);
            yield return null;
        }
        isMovingTowards = false;
        GetComponent<Rigidbody>().isKinematic = false;

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
