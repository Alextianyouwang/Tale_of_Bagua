using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
     List<GameObject> levelObject = new List<GameObject>();

    bool colliderToggleState = false;
    //Transform parent;
    private void Awake()
    {
        //parent = transform.Find("Collider");
        GetLevelObject();
    }
    public void GetLevelObject() 
    {
        foreach (Transform t in transform)
        {
            t.GetComponent<Collider>().isTrigger = true;
            levelObject.Add(t.gameObject);
        }
    }
    public void ToggleRigidColliders(bool value) 
    {
        if (colliderToggleState == value)
        return;
        colliderToggleState = value;
        foreach (GameObject t in levelObject) 
        {
            Collider c = t.GetComponent<Collider>();
            c.isTrigger =!value;
        }
    }
}
