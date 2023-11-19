using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC_Controller : MonoBehaviour
{
    public Action<Vector3,TextAsset,Sprite> OnDetactPlayer;
    public Action OnLostPlayer;
    public TextAsset InkDialogueAsset;
    public Sprite IconImage;

    private void OnTriggerEnter(Collider other)
    {
        if (!InkDialogueAsset)
        { print(gameObject.name + " does not have a ink story to play"); return; }
        if (!IconImage)
        { print(gameObject.name + " does not have a icon "); return; }
        if (other.tag.Equals("Player"))
            OnDetactPlayer?.Invoke(transform.position + Vector3.forward * 0.5f,InkDialogueAsset, IconImage);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
            OnLostPlayer?.Invoke();
    }
}
