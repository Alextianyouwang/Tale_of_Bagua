using System.Linq;
using UnityEngine;
using System;
public class PlayerInteract : MonoBehaviour
{
    public float InteractionDistance = 0.5f;
    public static Action<Vector3,IconType> OnDetactPlayer;
    public static Action OnLostPlayer;
    private IInteractable currentInteract, previousInteract;

    private void Update()
    {
        CheckObjectSelection();
        if (currentInteract != null)
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
                currentInteract.Interact();
        }
    }
    void CheckObjectSelection()
    {
        currentInteract = GetCurrentInteractiveObject();
        if (currentInteract != null)
            OnDetactPlayer?.Invoke((currentInteract as MonoBehaviour).transform.position + Vector3.forward * 0.5f,currentInteract.GetIconType());
        if (currentInteract == null && previousInteract != null)
            OnLostPlayer?.Invoke();
        else if (currentInteract != previousInteract && currentInteract  != null && previousInteract != null)
            OnLostPlayer?.Invoke();
    }
    IInteractable GetCurrentInteractiveObject() 
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, InteractionDistance);
        IInteractable[] interactables = objs.Select(x => x.GetComponent<IInteractable>()).Where(x => x != null).ToArray();
        if  (interactables.Length == 0) return null;

        float dist = float.MaxValue;
        IInteractable selected = null;
        foreach (IInteractable m in interactables)
        {
            float current = Vector3.Distance(transform.position, (m as MonoBehaviour).transform.position);
            if (current < dist)
            {
                dist = current;
                selected = m;
            }
        }
        return selected;
    }
 
}
