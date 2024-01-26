
using UnityEngine;

public class GetCollider : MonoBehaviour
{
    public BoxCollider[] colliders;


    public void GetColliders() 
    {
        colliders = GetComponents<BoxCollider>();
    }
}
