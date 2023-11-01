using System.Collections;
using UnityEngine;
using System;
public class Mirror : MonoBehaviour
{
    private BoxCollider[] boxs;
    private Coroutine movingCo;

    public Action OnFinishMoving;
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public Material[] material;

    public GetCollider crossCollider;
    public GetCollider boxCollider;
    [HideInInspector]
    public MeshRenderer frameRenderer;

    private void Awake()
    {
        frameRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        material = frameRenderer.materials;

        if (material == null)
            Debug.LogWarning("Emissive Material for Mirror not found");
        rb = GetComponent<Rigidbody>();
       
    }
    private void OnEnable()
    {
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

    public void RigidBodyAddForce(Vector3 direction, float intensity) 
    {
        rb.AddForce(direction * intensity, ForceMode.Force);
    }
    void GetBoxs() 
    {
        BoxCollider[] colliders = new BoxCollider[boxCollider.colliders.Length + crossCollider.colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i] =(i < boxCollider.colliders.Length ? boxCollider.colliders[i] : crossCollider.colliders[i - boxCollider.colliders.Length]);
        }
        
        boxs = colliders;
        
        foreach (BoxCollider b in boxs)
            b.isTrigger = true;
    }
   
    public void ToggleBoxesRigidCollider(bool value) 
    {
        foreach (BoxCollider b in boxs)
            b.isTrigger = !value;
    }

    
}
