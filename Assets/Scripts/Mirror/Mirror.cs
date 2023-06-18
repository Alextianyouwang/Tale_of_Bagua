using System.Collections;
using UnityEngine;
using System;
using System.Linq;

public class Mirror : MonoBehaviour
{
    private Collider[] boxs;
   //private bool colliderToggleState = false;

    private Coroutine movingCo;

    public Action OnFinishMoving;
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public Material[] material;
    [HideInInspector]

    public GetCollider crossCollider;
    [HideInInspector]
    public MeshRenderer frameRenderer;

    void Start()
    {
        frameRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        material = frameRenderer .materials;
        
        if (material == null)
            Debug.LogWarning("Emissive Material for Mirror not found");
        rb = GetComponent<Rigidbody>(); 
        GetBoxs();
    }

    public void MoveMirrorTowards(float time, Vector3 targetPos, AnimationCurve curve) 
    {
        AbortMovement();
        movingCo = StartCoroutine(MoveTowards(time, targetPos, curve));

    }

    public void AbortMovement() 
    {

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (movingCo != null)
            StopCoroutine(movingCo);
    }
    private IEnumerator MoveTowards(float time,Vector3 targetPos, AnimationCurve curve) 
    {
        float percent = 0;
        Vector3 originalPos = transform.position;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        while ( percent < 1) 
        {
            percent += Time.deltaTime/time;
            float interpolate = curve.Evaluate(percent);
            transform.position = Vector3.Lerp(originalPos, targetPos, interpolate);
            yield return null;
        }

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
            
        OnFinishMoving?.Invoke();

    }
    void GetBoxs() 
    {
        boxs = GetComponents<Collider>();
        Collider[] colliders = new Collider[boxs.Length + crossCollider.colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i] = i < boxs.Length ? boxs[i] : crossCollider.colliders[i - boxs.Length];
        }
        boxs = colliders;
        foreach (Collider b in boxs)
            b.isTrigger = true;
    }
   
    public void ToggleBoxesRigidCollider(bool value) 
    {
        /*if (colliderToggleState == value)
            return;
        colliderToggleState = value;*/
        foreach (Collider b in boxs)
            b.isTrigger = !value;
    }

    
}
