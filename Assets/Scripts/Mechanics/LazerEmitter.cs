using System.Collections.Generic;
using UnityEngine;

public class LazerEmitter : RationalObject,IInteractable
{
    public enum Oriantations { Top, Bot, Left, Right}
    public Oriantations OriantationOptions;

    public Material[] RayVisualMaterial;
    private List<Vector3> _rayCastPositionTracker = new List<Vector3>();
    private RaycastHit _hitReceiverObject;
    private LineRenderer[] _rayVisual;


    private void Awake()
    {
        PrepareLineVisual();
    }

    private void PrepareLineVisual() 
    {
        _rayVisual = new LineRenderer[5];
       
       
        for (int i = 0; i < _rayVisual.Length; i++) 
        {
            GameObject g = new GameObject();
            g.transform.parent = transform;
            g.layer = 6 + i;
            g.AddComponent<LineRenderer>();
            _rayVisual[i] = g.GetComponent<LineRenderer>();
            _rayVisual[i].material = RayVisualMaterial[i];
            _rayVisual[i].enabled = false;
        }
        
    }
    public void Interact() 
    {

        ReceiveShootCommand();
    }
    public void Hold() { }

    public void Disengage() 
    {
        ReceiveStopCommand();
    }
    public bool IsVisible()
    {
        return IsObjectVisibleAndSameLevelWithPlayer();
    }

    public IconType GetIconType() 
    {
        return IconType.kavaii;
    }
    private void ReceiveShootCommand() 
    {
        ShootLazer(40, 0.47f);
    }
    private void ReceiveStopCommand() 
    {
        foreach (LineRenderer r in _rayVisual)
        {
            r.enabled = false;
        }
        _rayCastPositionTracker.Clear();
    }
    private void ShootLazer(int steps, float increment) 
    {
        _rayCastPositionTracker.Clear();

        Vector3 currentPosition = transform.position;
        Vector3 direction = Vector3.zero;
        switch (OriantationOptions) 
        {
            case Oriantations.Top:
                direction = Vector3.forward;
                break;
            case Oriantations.Bot:
                direction = Vector3.back;
                break;
            case Oriantations.Left:
                direction = Vector3.left;
                break;
            case Oriantations.Right:
                direction = Vector3.right;
                break;
        }
        
        for (int i = 0; i < steps; i++) 
        {
            _rayCastPositionTracker.Add(currentPosition);
            if (!FreeToProceed(currentPosition, 0.01f))
                break;
            else 
            {
                if (HandShake("LazerReceiver",currentPosition, increment * direction, out _hitReceiverObject)) 
                {
                    if (CheckVisibility(_hitReceiverObject)) 
                    {
                        Connect(_hitReceiverObject);
                        break;
                    }
                }
            }    
            currentPosition += increment * direction;
        }
        foreach (LineRenderer r in _rayVisual) 
        {
            r.enabled = true;
            r.SetPosition(0, transform.position);
            r.SetPosition(1, _hitReceiverObject.transform == null ? currentPosition : _hitReceiverObject.point);
            r.startWidth = 0.1f;
            r.endWidth = 0.1f;
        }
      
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Vector3 v in _rayCastPositionTracker)
            Gizmos.DrawSphere(v,0.1f);
        if (_hitReceiverObject.transform?.GetComponent<LazerReceiver>()) 
        {
            if (_hitReceiverObject.transform.GetComponent<LazerReceiver>().tag.Equals("LazerReceiver")) 
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_hitReceiverObject.point, 0.2f);
            }
        }
     
    }
}
