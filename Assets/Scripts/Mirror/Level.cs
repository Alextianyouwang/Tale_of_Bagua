using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
     List<GameObject> levelObject = new List<GameObject>();

    List< BoxCollider> boxColliders = new List<BoxCollider> ();

    bool colliderToggleState = false;

    public GameObject[] AdditionalObjects;
    private void Awake()
    {
        GetLevelObject();
    }
    public void GetLevelObject() 
    {
        foreach (Transform t in transform)
        {
            if (!t.GetComponent<Collider>())
                continue;
            t.GetComponent<Collider>().isTrigger = true;
            levelObject.Add(t.gameObject);
        }

        boxColliders = GetComponents<BoxCollider>().ToList();
        if (AdditionalObjects != null) 
        {
            foreach (GameObject g in AdditionalObjects)
            {
                if (g == null)
                    continue;
                if (!g.GetComponent<BoxCollider>())
                    continue;
                boxColliders.Add(g.GetComponent<BoxCollider>());
            }
        }
        foreach (BoxCollider c in boxColliders) 
        {
            c.isTrigger = true;
        }

    }
    public void ToggleRigidColliders(bool value) 
    {
        if (colliderToggleState == value)
        return;
        colliderToggleState = value;
        foreach (GameObject t in levelObject) 
        {
            if (!t.GetComponent<Collider>())
                continue;
            Collider c = t.GetComponent<Collider>();
            c.isTrigger =!value;
        }
        foreach (BoxCollider c in boxColliders)
        {
            c.isTrigger = !value;
        }
    }
}
