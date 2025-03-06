using System.Collections.Generic;
using UnityEngine;
public abstract class Lazer : RationalObject
{
    public GameObject VisualCueUI;
    public Material[] RayVisualMaterial;

    private List<Vector3> _rayCastPositionTracker = new List<Vector3>();
    private RaycastHit _hitReceiverObject;
    private LineRenderer[] _rayVisual;
    private Lazer _chainedEmitter = null;
    protected static List<Lazer> _path = new List<Lazer>();

    private void Awake()
    {
        PrepareLineVisual();
    }
    protected void OnEnable()
    {
        _path = new List<Lazer>();
    }
    protected  void OnDisable()
    {
        _path = null;
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
    public bool IsActive() 
    {
        return true;
    }
    protected void RecursivelyStop() 
    {
        Lazer current = this;
        while (current != null)
        {
            current.ReceiveStopCommand();
            current = current._chainedEmitter;
        }
    }
    protected void ReceiveStopCommand() 
    {
        foreach (LineRenderer r in _rayVisual)
            r.enabled = false;
        _rayCastPositionTracker.Clear();
    }

    protected void ProcessChain(RationalObject hit) 
    {
        if (hit == null)
            return;
        Lazer chain = hit.GetComponent<Lazer>();
        if (chain == null) 
        {
            hit.Receive(this);
            return;
        }
        if (chain != null && !_path.Contains(chain)) 
        {
            _chainedEmitter = chain;
            _path.Add(chain);
            hit.Receive(this);
        }
    }
    protected void ShootLazer(int steps, float increment, Vector3 direction, out RationalObject hit) 
    {
        _rayCastPositionTracker.Clear();

        Vector3 currentPosition = transform.position;
        hit = null;
        for (int i = 0; i < steps; i++) 
        {
            _rayCastPositionTracker.Add(currentPosition);
            if (HandShake(currentPosition, increment * direction, out _hitReceiverObject))
            {
                if (CheckVisibility(_hitReceiverObject))
                {
                    hit = _hitReceiverObject.transform.GetComponent<RationalObject>();
                    if (hit != null)
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
