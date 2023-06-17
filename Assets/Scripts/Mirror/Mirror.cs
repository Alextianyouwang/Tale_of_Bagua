using System.Collections;
using UnityEngine;
using System;
using System.Linq;

public class Mirror : MonoBehaviour
{
    private Collider[] boxs;
    private bool colliderToggleState = false;

    private Coroutine movingCo;

    public Action OnFinishMoving;

    private Rigidbody rb;

    public Material[] material;

    public GetCollider crossCollider;
   //public Material[] GetEmissiveMaterial() { return material; }
    public MeshRenderer frameRenderer;

    void Start()
    {
        material= transform.GetChild(0).GetComponent<MeshRenderer>() .materials;
        if (material == null)
            Debug.LogWarning("Emissive Material for Mirror not found");
        rb = GetComponent<Rigidbody>(); 
        GetBoxs();
    }

    public void MoveMirrorTowards(float time, Vector3 targetPos) 
    {
        AbortMovement();
        movingCo = StartCoroutine(MoveTowards(time, targetPos));

    }

    public void AbortMovement() 
    {

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (movingCo != null)
            StopCoroutine(movingCo);
    }
    private IEnumerator MoveTowards(float time,Vector3 targetPos) 
    {
        float percent = 0;
        Vector3 originalPos = transform.position;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        while ( percent < 1) 
        {
            percent += Time.deltaTime/time;
            transform.position = Vector3.Lerp(originalPos, targetPos, percent);
            yield return null;
        }

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
            
        OnFinishMoving?.Invoke();

    }
    void GetBoxs() 
    {
        boxs = GetComponentsInChildren<Collider>();
       /* Collider[] colliders = new Collider[boxs.Length + crossCollider.colliders.Length];
        for (int i = 0; i < colliders.Length; i++) 
        {
            colliders[i] = i < boxs.Length ? boxs[i] : crossCollider.colliders[i-boxs.Length];
        }
        boxs = colliders;*/
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
