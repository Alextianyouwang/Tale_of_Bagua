using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
public class PlayerInteract : MonoBehaviour
{
    public float InteractionDistance = 0.5f;
    public static NPC_Controller currentNPC,previousNPC;
    public static Action<Vector3> OnDetactPlayer;
    public static Action OnLostPlayer;
    public static Action<TextAsset, Sprite> OnPlayDialogue;

    private void Update()
    {
        CheckObjectSelection();
        if (currentNPC)
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                OnPlayDialogue?.Invoke(currentNPC.InkDialogueAsset, currentNPC.IconImage);
                
            }
               
        }
    }
    void CheckObjectSelection()
    {
        currentNPC = GetCurrentNPC();
        if (currentNPC)
            OnDetactPlayer?.Invoke(currentNPC.transform.position + Vector3.forward * 0.5f);
        if (currentNPC == null && previousNPC != null)
            OnLostPlayer?.Invoke();
        else if (currentNPC != previousNPC && currentNPC != null && previousNPC != null)
            OnLostPlayer?.Invoke();
        previousNPC = currentNPC;
    }
    NPC_Controller GetCurrentNPC()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, InteractionDistance);
        NPC_Controller[] npcs = objs.Where(x => x.tag.Equals("NPC")).Select(x => x.GetComponent<NPC_Controller>()).ToArray();
        if (npcs.Length == 0)
            return null;
        float dist = float.MaxValue;
        NPC_Controller selected = null;
        foreach (NPC_Controller m in npcs)
        {
            float current = Vector3.Distance(transform.position, m.transform.position);
            if (current < dist)
            {
                dist = current;
                selected = m;
            }
        }
        return selected;
    }
}
