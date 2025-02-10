using System.Collections;
using UnityEngine;
using System;
public class Mirror : MonoBehaviour
{
    private BoxCollider[] _boxs;
    private Coroutine _movingCo;

    private Rigidbody _rigidBody;
    private Material[] _materials;
    private MeshRenderer _frameRenderer;

    public Action OnFinishMoving;
    public GetCollider CrossCollider;
    public GetCollider BoxCollider;
    public Rigidbody RigidBody => _rigidBody;
    public Material[] Materials => _materials;
    public MeshRenderer FrameRenderer => _frameRenderer;
    private void Awake()
    {
        _frameRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _materials = FrameRenderer.materials;
        _rigidBody = GetComponent<Rigidbody>();

        if (Materials == null)
            Debug.LogWarning("Emissive Material for Mirror not found");
    }
    private void OnEnable()
    {
        CrossCollider.GetColliders();
        BoxCollider.GetColliders();
        GetBoxs();
    }

    public void ToggleColliderSize(bool enlarge) 
    {
        if (enlarge)
        {
            CrossCollider.transform.localScale = Vector3.one * 1.01f;
            BoxCollider.transform.localScale = Vector3.one * 1.01f;
        }
        else 
        {
            CrossCollider.transform.localScale = Vector3.one * 0.99f;
            BoxCollider.transform.localScale = Vector3.one * 0.99f;
        }
    }

    public void MoveMirrorTowards(float time, Vector3 targetPos, AnimationCurve curve) 
    {
        AbortMovement();
        _movingCo = StartCoroutine(MoveTowards(time, targetPos, curve));

    }

    public void AbortMovement() 
    {

        RigidBody.isKinematic = false;
        RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        if (_movingCo != null)
            StopCoroutine(_movingCo);
    }
    private IEnumerator MoveTowards(float time,Vector3 targetPos, AnimationCurve curve) 
    {
        float percent = 0;
        Vector3 originalPos = transform.position;
        RigidBody.isKinematic = true;
        RigidBody.constraints = RigidbodyConstraints.FreezeAll;

        while ( percent < 1) 
        {
            percent += Time.deltaTime/time;
            float interpolate = curve.Evaluate(percent);
            transform.position = Vector3.Lerp(originalPos, targetPos, interpolate);
      
            yield return null;
        }

        RigidBody.isKinematic = false;
        RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            
        OnFinishMoving?.Invoke();

    }

    public void RigidBodyAddForce(Vector3 direction, float intensity) 
    {
        RigidBody.AddForce(direction * intensity, ForceMode.Force);
    }
    void GetBoxs() 
    {
        BoxCollider[] colliders = new BoxCollider[BoxCollider.colliders.Length + CrossCollider.colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i] =(i < BoxCollider.colliders.Length ? BoxCollider.colliders[i] : CrossCollider.colliders[i - BoxCollider.colliders.Length]);
        }
        
        _boxs = colliders;
        
        foreach (BoxCollider b in _boxs)
            b.isTrigger = true;
    }
   
    public void ToggleBoxesRigidCollider(bool value) 
    {
        foreach (BoxCollider b in _boxs)
            b.isTrigger = !value;
    }

    
}
