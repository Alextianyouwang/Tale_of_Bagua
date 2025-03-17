using UnityEngine;

public class MoveGateTrigger : RationalObject, IInteractable
{
    public MoveGate[] Gates;

    private bool _stage0 = true;
    private bool _stage1 = false;
    public void DetactPlayer() { }

    public void Interact(Vector3 pos)
    {
        if (_stage0 == true) 
        {
            _stage0 = false;
            _stage1 = true;
            foreach (var gate in Gates) 
            {
                gate.StartMove(true);
            }
        }
        else if (_stage1 == true) 
        {
            _stage1 = false;
            _stage0 = true;
            foreach (var gate in Gates)
            {
                gate.StartMove(false);
            }
        }
            

    }

    public void Hold() { }

    public void Disengage()
    {

    }

    public bool IsVisible()
    {
        return IsObjectVisibleAndSameLevelWithPlayer();
    }

    public bool IsActive()
    {
        return true;
    }
    public IconType GetIconType()
    {

        return IconType.exclamation;
    }
}
