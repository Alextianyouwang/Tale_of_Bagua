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
        if (currentInteract == null || !currentInteract.IsVisible() || !currentInteract.IsActive())
            return;
        if (Input.GetKeyDown(KeyCode.Space))
            currentInteract.Interact();
        else if (Input.GetKey(KeyCode.Space))
            currentInteract.Hold();
        else if (Input.GetKeyUp(KeyCode.Space))
            currentInteract.Disengage();
      
    }
    void CheckObjectSelection()
    {
        currentInteract = GetCurrentInteractiveObject();

        if (currentInteract != null && currentInteract.IsVisible() && currentInteract.IsActive())
            OnDetactPlayer?.Invoke((currentInteract as MonoBehaviour).transform.position + Vector3.forward * 0.5f, currentInteract.GetIconType());


        if (currentInteract == null && previousInteract != null)
        {
            OnLostPlayer?.Invoke();
            previousInteract.Disengage();
        }
        else if (currentInteract != null && !currentInteract.IsVisible()) 
        {
            OnLostPlayer?.Invoke();
            currentInteract.Disengage();
        }
        else if (currentInteract != previousInteract && currentInteract != null && previousInteract != null) 
        {
            OnLostPlayer?.Invoke();
            previousInteract.Disengage();
        }
        previousInteract = currentInteract;
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
