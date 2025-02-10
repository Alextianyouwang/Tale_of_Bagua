using System.Collections.Generic;
using UnityEngine;
public class LazerEmitter : RationalObject,IInteractable
{
    public enum Orientation { Top, Right ,Bot, Left}
    public Orientation OriantationOptions;
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
    protected void OnEnable()
    {
        OnReceive += BranchReceived;
        
    }
    protected  void OnDisable()
    {
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
    public void Interact(Vector3 pos) 
    {

        if (!Reflector)
            ReceiveShootCommand();
        else 
        {
            int currentOriaintation = (int)OriantationOptions;
            currentOriaintation += 1;
            currentOriaintation %= 4 ;
            OriantationOptions = (Orientation)currentOriaintation;
            Editor_ChangeOriantationUI();
        }
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
        ShootLazer(80, 0.23f);
    }
    public bool IsActive() 
    {
        return true;
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
            case Orientation.Top:
                direction = Vector3.forward;
                break;
            case Orientation.Bot:
                direction = Vector3.back;
                break;
            case Orientation.Left:
                direction = Vector3.left;
                break;
            case Orientation.Right:
                direction = Vector3.right;
                break;
        }
        
        for (int i = 0; i < steps; i++) 
        {
            _rayCastPositionTracker.Add(currentPosition);
            if (HandShake(currentPosition, increment * direction, out _hitReceiverObject))
            {
                if (CheckVisibility(_hitReceiverObject))
                {
                    LazerEmitter chain = _hitReceiverObject.transform.GetComponent<LazerEmitter>();
                    if (chain != null && chain.Reflector)
                        _chainedEmitter = chain;

                   
                    _hitReceiverObject.transform.GetComponent<RationalObject>().Receive(this);
                    break;
                }
            }
            if (!FreeToProceed(currentPosition, 0.01f))
                break;

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
    public void BranchReceived(RationalObject ro)
    {
        LazerEmitter l = ro.GetComponent<LazerEmitter>();
        if (l == null)
            return;

        if ((int)l.OriantationOptions == ((int)OriantationOptions + 2) % 4)
            return;
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
            case Orientation.Top:
                eularAngle = new Vector3(90, 180, 0);
                break;
            case Orientation.Bot:
                eularAngle = new Vector3(90, 0, 0);
                break;
            case Orientation.Left:
                eularAngle = new Vector3(90, 90, 0);
                break;
            case Orientation.Right:
                eularAngle = new Vector3(90, 270, 0);
                break;
        }
        VisualCueUI.transform.eulerAngles = eularAngle;
        VisualCueUI.transform.localScale = Reflector? Vector3.one * 0.5f : Vector3.one;
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
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
