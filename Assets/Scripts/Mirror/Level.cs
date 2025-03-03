using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    private List<GameObject> _levelObjects = new List<GameObject>();
    private List<BoxCollider> _boxColliders = new List<BoxCollider> ();
    private bool _colliderToggleState = false;

    private void Awake()
    {
        GetLevelObject();
    }
    private void GetLevelObjectRecursive(Transform transform) 
    {
        foreach (Transform t in transform)
        {
            GetLevelObjectRecursive(t);
            if (!t.GetComponent<Collider>())
                continue;
            t.GetComponent<Collider>().isTrigger = true;
            _levelObjects.Add(t.gameObject);
        }
    }
    private void GetLevelObject() 
    {
        GetLevelObjectRecursive(transform);
        _boxColliders = GetComponents<BoxCollider>().ToList();
        foreach (BoxCollider c in _boxColliders) 
        {
            c.isTrigger = true;
        }
    }
    public void ToggleRigidColliders(bool value) 
    {
        if (_colliderToggleState == value)
        return;
        _colliderToggleState = value;
        foreach (GameObject t in _levelObjects) 
        {
            if (!t.GetComponent<Collider>())
                continue;
            Collider c = t.GetComponent<Collider>();
            c.isTrigger =!value;
        }
        foreach (BoxCollider c in _boxColliders)
        {
            c.isTrigger = !value;
        }
    }
}
