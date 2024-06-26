
using UnityEngine;

public class Door : RationalObject, IInteractable
{
    private BoxCollider _doorCollider;
    private MeshRenderer _doorRenderer;
    public enum OpenDirection { Top, Bot, Left, Right }
    public OpenDirection ActivateDirection;
    protected override void OnEnable()
    {
        base.OnEnable();
        _doorCollider = GetComponent<BoxCollider>();
        _doorRenderer = GetComponent<MeshRenderer>();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public void Interact(Vector3 pos)
    {
        switch (ActivateDirection) 
        {
            case OpenDirection.Top:
                if (pos.y > transform.position.x)
                    Open();
                break;
            
            case OpenDirection.Bot:
                if (pos.y < transform.position.x)
                    Open();
                break;
            
            case OpenDirection.Left:
                if (pos.x < transform.position.x)
                    Open();
                break;
            
            case OpenDirection.Right:
                if (pos.x > transform.position.x)
                    Open();
                break;
        }
    
    }

    public void Open() 
    {
        _doorCollider.enabled = false;
        _doorRenderer.enabled = false;
        foreach (Transform t in transform) 
        {
            t.GetComponent<MeshRenderer>().enabled = false;
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
        
        return IconType.door;
    }



}
