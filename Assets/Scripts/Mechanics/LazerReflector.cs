using NUnit.Framework.Internal;
using UnityEngine;

public class LazerReflector : RationalObject, IInteractable
{
    private void OnEnable()
    {
        OnReceive += ReceiveLazer;
    }
    public void Interact(Vector3 pos)
    {

    }
    public void Hold() { }

    public void Disengage()
    {

    }
    public bool IsActive()
    {
        return true;
    }
    public bool IsVisible()
    {
        return IsObjectVisibleAndSameLevelWithPlayer();
    }
    public IconType GetIconType()
    {
        return IconType.kavaii;
    }

    public void ReceiveLazer(RationalObject ro)
    {
        LazerEmitter l = ro.GetComponent<LazerEmitter>();
        if (l == null)
            return;

        
    }

}
