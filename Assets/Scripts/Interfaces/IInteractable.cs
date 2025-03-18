using UnityEngine;

public interface IInteractable
{
    public void Interact(Vector3 pos);

    public Orientation GetIconOrientation();
    public void DetactPlayer();
    public void Hold();
    public void Disengage();

    public bool IsVisible();
    public bool IsActive();
    public IconType GetIconType();
}

