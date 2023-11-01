using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GetCollider : MonoBehaviour
{
    public BoxCollider[] colliders;

    private void Awake()
    {
        colliders= GetComponents<BoxCollider>();
    }
}
