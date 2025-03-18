using UnityEngine;

public class EventBlock : RationalObject, IInteractable
{
    [SerializeField] private EventObject _eventBlock;
    public void DetactPlayer() 
    {
        _eventBlock.Raise();
    }

    public void Disengage()
    {

    }

    public IconType GetIconType()
    {
        return IconType.space;
    }

    public void Hold()
    {

    }

    public void Interact(Vector3 pos)
    {
     
    }
    public Orientation GetIconOrientation() => Orientation.Top;
    public bool IsActive()
    {
        return true;
    }

    public bool IsVisible()
    {
        return IsObjectVisibleAndSameLevelWithPlayer();
    }
}
