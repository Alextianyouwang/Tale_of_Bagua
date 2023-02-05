using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour
{
    public GameObject NextItem;
    public GameObject NewMirror;
    public GameObject Panel;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            NextItem.SetActive(true);
            NewMirror.SetActive(true);
            Panel.SetActive(true);

            gameObject.SetActive(false);
        }

        
    }
}
