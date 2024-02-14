using UnityEngine;

public interface IInteractable
{
    public void Interact();
    public void Hold();
    public void Disengage();

    public bool IsVisible();
    public IconType GetIconType();
}

