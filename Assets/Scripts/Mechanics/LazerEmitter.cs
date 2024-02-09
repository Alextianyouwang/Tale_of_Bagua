using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LazerEmitter : MonoBehaviour
{
    public enum Oriantations { Top, Bot, Left, Right}
    public Oriantations OriantationOptions;
    public LayerMask ObstacleMask, MirrorMask;
    private bool _hasBeenActivated;
    private Level[] _levels;
    private List<Vector3> _rayCastPositionTracker = new List<Vector3>();
    public bool HasBeenActivated 
    {
        get { return _hasBeenActivated; }
        set { _hasBeenActivated = value; }
    }

    private Collider[]_overlappingColliders;
    private RaycastHit[] _allHitsMirrors;
    private RaycastHit _hitReceiverObject;
    [SerializeField] private int _levelIndex = 0;

    private void OnEnable()
    {
        LevelManager.OnFixUpdate += FixUpdate;
        LevelManager.OnShareAllLevels += ReceiveAllLevels;
    }
    private void OnDisable()
    {
        LevelManager.OnFixUpdate -= FixUpdate;
        LevelManager.OnShareAllLevels -= ReceiveAllLevels;

    }
    private void ReceiveAllLevels(Level[] level) 
    {
        _levels = level;
    }

    
    private void ReceiveShootCommand() 
    {
        ShootLazer(20, 0.5f);
    }
    private void FixUpdate() 
    {
        
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
                if (HandShake(currentPosition, increment * direction, out _hitReceiverObject)) 
                {
                    Connect(_hitReceiverObject);
                    break;
                }
            }    
            currentPosition += increment * direction;
        }
    }

    private bool FreeToProceed(Vector3 position, float objectRadius) 
    {
        _overlappingColliders = Physics.OverlapSphere(position, objectRadius, ObstacleMask);
        _allHitsMirrors = Physics.RaycastAll(position - Vector3.up * 3f, Vector3.up, 20f, MirrorMask);
        Level currentLevel = _levels[_allHitsMirrors.Length - 1];
        if (_overlappingColliders.Length == 0)
            return true;
        foreach (Collider c in _overlappingColliders)
        {
            Level level = c.gameObject.GetComponentInParent<Level>();
            if (level == null || c == GetComponent<Collider>())
                continue;
            if (level == currentLevel)
                return false;
        }
        return true;
    }

    private bool HandShake(Vector3 position,Vector3 increment, out RaycastHit hit) 
    {
        Ray handshake = new Ray(position, increment.normalized);
        Physics.Raycast(handshake, out hit, increment.magnitude);
        if (hit.transform != null && hit.transform.tag.Equals("LazerReceiver"))
            return true;
        else
            return false;
    }

    private void Connect(RaycastHit hit) 
    {
        if (hit.transform.GetComponent<LazerReceiver>())
            hit.transform.GetComponent<LazerReceiver>().Receive();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Vector3 v in _rayCastPositionTracker)
            Gizmos.DrawSphere(v,0.1f);
        Gizmos.color = (_hitReceiverObject.transform?.GetComponent<LazerReceiver>() ? Color.green: Color.red);
        Gizmos.DrawSphere(_hitReceiverObject.point, 0.2f);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            ReceiveShootCommand();
        }
    }
}
