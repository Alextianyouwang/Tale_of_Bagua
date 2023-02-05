using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public BoxCollider[] boxs;
    bool colliderToggleState = false;
    void Start()
    {
        GetBoxs();
    }

    void GetBoxs() 
    {
        boxs = GetComponents<BoxCollider>();
        foreach (BoxCollider b in boxs)
        {
            b.isTrigger = true;
        }
    }

    public void ToggleBoxesRigidCollider(bool value) 
    {
        if (colliderToggleState == value)
            return;
        colliderToggleState = value;

        foreach (BoxCollider b in boxs)
        {
            b.isTrigger = !value;
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ToggleBoxesRigidCollider(false);
        }
    }
}
