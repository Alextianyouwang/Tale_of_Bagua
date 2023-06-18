using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GetCollider : MonoBehaviour
{
    public Collider[] colliders;

    private void OnEnable()
    {
        colliders= GetComponents<Collider>();
    }
}
