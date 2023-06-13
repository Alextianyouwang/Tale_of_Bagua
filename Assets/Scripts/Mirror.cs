using UnityEngine;

public class Mirror : MonoBehaviour
{
    public Collider[] boxs;
    bool colliderToggleState = false;
    void Start()
    {
        GetBoxs();
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
