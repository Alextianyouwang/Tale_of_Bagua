using System.Collections.Generic;
using UnityEngine;
public class LazerEmitter : RationalObject,IInteractable
{
    public enum Oriantations { Top, Bot, Left, Right}
    public Oriantations OriantationOptions;
    public bool Reflector = false;

    public Material[] RayVisualMaterial;
    private List<Vector3> _rayCastPositionTracker = new List<Vector3>();
    private RaycastHit _hitReceiverObject;
    private LineRenderer[] _rayVisual;

    public GameObject VisualCueUI;
    private LazerEmitter _chainedEmitter = null;

    private void Awake()
    {
        PrepareLineVisual();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        OnReceive += BranchReceived;
        
    }
    protected override void OnDisable()
    {
        base.OnDisable();
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
        RecursivelyStop();
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
    public bool IsActive() 
    {
        return !Reflector;
    }
    private void RecursivelyStop() 
    {
        LazerEmitter current = this;
        while (current != null)
        {
            current.ReceiveStopCommand();
            current = current._chainedEmitter;
        }
    }
    private void ReceiveStopCommand() 
    {
        foreach (LineRenderer r in _rayVisual)
            r.enabled = false;
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

            if (HandShake(currentPosition, increment * direction, out _hitReceiverObject))
            {
                if (CheckVisibility(_hitReceiverObject))
                {
                    LazerEmitter chain = _hitReceiverObject.transform.GetComponent<LazerEmitter>();
                    if (chain != null && chain.Reflector)
                        _chainedEmitter = chain;

                    _hitReceiverObject.transform.GetComponent<RationalObject>().Receive();
                    break;
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
    public void BranchReceived()
    {
        if (Reflector)
            ReceiveShootCommand();
    }

public void Editor_ChangeOriantationUI() 
    {
        if (!VisualCueUI)
            return;
        Vector3 eularAngle = Vector3.zero;
        switch (OriantationOptions)
        {
            case Oriantations.Top:
                eularAngle = new Vector3(90, 180, 0);
                break;
            case Oriantations.Bot:
                eularAngle = new Vector3(90, 0, 0);
                break;
            case Oriantations.Left:
                eularAngle = new Vector3(90, 90, 0);
                break;
            case Oriantations.Right:
                eularAngle = new Vector3(90, 270, 0);
                break;
        }
        VisualCueUI.transform.eulerAngles = eularAngle;
        VisualCueUI.transform.localScale = Reflector? Vector3.one * 0.5f : Vector3.one;
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
