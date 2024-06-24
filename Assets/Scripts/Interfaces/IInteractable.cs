using UnityEngine;

public interface IInteractable
{
    public void Interact(Vector3 pos);
    public void Hold();
    public void Disengage();

    public bool IsVisible();
    public bool IsActive();
    public IconType GetIconType();
}

