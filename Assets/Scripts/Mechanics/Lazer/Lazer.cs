using System.Collections.Generic;
using UnityEngine;
public abstract class Lazer : RationalObject
{
    public GameObject VisualCueUI;
    public Material[] RayVisualMaterial;

    private List<Vector3> _rayCastPositionTracker = new List<Vector3>();
    private RaycastHit _hitReceiverObject;
    private  List<LineRenderer> _rayVisual = new List<LineRenderer>();
    protected Lazer _upstreamEmitter, _downstreamEmitter;
    protected static List<Lazer> _path = new List<Lazer>();
    private int _rayThroughCount = 0;
    public int RayThroughCount => _rayThroughCount;

    protected Lazer_Helper.Orientation _orientation;
    public Lazer_Helper.Orientation Orientation => _orientation;
    protected void OnEnable()
    {
        _path = new List<Lazer>();
    }
    protected  void OnDisable()
    {
        _path = null;
    }
    public bool IsActive() => true;

    private void PrepareLineVisual() 
    {
        for (int i = 0; i < 5; i++) 
        {
            GameObject g = new GameObject();
            g.transform.parent = transform;
            g.layer = 6 + i;
            g.AddComponent<LineRenderer>();
            LineRenderer lr = g.GetComponent<LineRenderer>();
            _rayVisual.Add(lr) ;
            lr.material = RayVisualMaterial[i];
            lr.enabled = false;
        }
    }
    protected static void StopAllLazerInChain() 
    {
         foreach (Lazer l in _path) 
            l.ReceiveStopCommand();
        _path?.Clear();
    }
    protected void ReceiveStopCommand() 
    {
        for (int i = 0 ; i < _rayVisual.Count; i++) 
            Destroy(_rayVisual[i].gameObject);
        _rayVisual.Clear();

        _rayCastPositionTracker.Clear();
        _upstreamEmitter = null;
        _downstreamEmitter = null;
        _rayThroughCount = 0;
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
        else
        {
            if ( chain.RayThroughCount < 2) 
            {
                chain._upstreamEmitter = this;
                _downstreamEmitter = chain;
                _path.Add(chain);
                hit.Receive(this);
            }
        }
    }
    protected void ShootLazer(int steps, float increment,
        Lazer_Helper.Orientation orientaion,  out RationalObject hit) 
    {
        PrepareLineVisual();
        _rayCastPositionTracker.Clear();
        _orientation = orientaion;
        Vector3 direction=  Lazer_Helper.GetDirection(orientaion);
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
        for (int i = _rayThroughCount * 5; i < 5 + _rayThroughCount * 5; i++) 
        {
            LineRenderer r = _rayVisual[i];
            r.enabled = true;
            r.SetPosition(0, transform.position);
            r.SetPosition(1, _hitReceiverObject.transform == null ? currentPosition : _hitReceiverObject.point);
            r.startWidth = 0.1f;
            r.endWidth = 0.1f;
        }
        _rayThroughCount++;
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
