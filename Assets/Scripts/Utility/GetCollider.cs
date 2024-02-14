
using UnityEngine;

public class GetCollider : MonoBehaviour
{
    [HideInInspector]
    public BoxCollider[] colliders;

    public void GetColliders() 
    {
        colliders = GetComponents<BoxCollider>();
    }
}
