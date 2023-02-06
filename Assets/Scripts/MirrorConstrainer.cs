using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorConstrainer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Mirror") 
        {
            print(other.name);
            other.GetComponent<Rigidbody>().Sleep();
        }
    }
}
