using UnityEngine;
using System;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static Action OnPlayerSwitchLevel;
    public static Action OnFixUpdate;

    public static Mirror[] _Mirrors;
    public static Mirror[] _HoodMirrors;
    public static Level[] _Levels;
    public static int AllActivatedMirrors;

    [SerializeField] private bool _enableAllMirrorAtStart = false;
    [SerializeField] private LayerMask _mirrorRayCastMask,_obstacleSphereCollideMask;
    [SerializeField] private Level[] _levels;
    [SerializeField] private Mirror[] _mirrors;

    private Level _currentLevel,_lastLevel,_nextLevel;
    private Mirror[] _hoodMirrors;
    private Collider[] _overlappedCollider;
    private RaycastHit[] _allHitMirrors;
    private int _allActivatedMirrors_prev;

    private void OnEnable()
    {
        AllActivatedMirrors = 0;
        _Mirrors = _mirrors;
        _Levels = _levels;
    }
    private void OnDisable()
    {
        AllActivatedMirrors = 0;
        _Mirrors = null;
        _Levels = null;
        _HoodMirrors = null;
    }
    private void Start()
    {
        _mirrors = FindObjectsByType<Mirror>(FindObjectsSortMode.InstanceID);
        foreach (var mirror in _mirrors) 
            mirror.gameObject.SetActive(_enableAllMirrorAtStart);

    }

    void DisableOtherLevels(Level current) 
    {
        current.ToggleRigidColliders(true);
        foreach (Level l in _levels) 
        {
            if (l != current)
                l.ToggleRigidColliders(false);
        }
    }

    private void FixedUpdate()
    {
        CheckLayers();
        OnFixUpdate?.Invoke();
    }
   
   
    private void CheckLayers()
    {
        UpdateLevelInfo();
        DisableOtherLevels(_currentLevel);
        LevelColliderControl();
    }
    private void UpdateLevelInfo() 
    {
        // Somehow overlaps sphere cannot get all colliders..
        //_overlappedCollider = Physics.OverlapSphere(transform.position, 0.4f * transform.localScale.x, _obstacleSphereCollideMask);
        _overlappedCollider = Physics.RaycastAll(transform.position - Vector3.up, Vector3.up, 10, _obstacleSphereCollideMask).Select(x=>x.collider).ToArray();
        _allHitMirrors = Physics.RaycastAll(transform.position - Vector3.up * 3f, Vector3.up, 20f, _mirrorRayCastMask);
        RaycastHit[] mirrorHits = _allHitMirrors.Where(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        _hoodMirrors = mirrorHits.Select(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        _HoodMirrors =  _hoodMirrors;

        AllActivatedMirrors = _hoodMirrors.Length;
        _currentLevel = _levels[_allHitMirrors.Length - 1];
        _lastLevel = _levels[_allHitMirrors.Length - 2 <= 0 ? 0 : _allHitMirrors.Length - 2];
        _nextLevel = _levels[_allHitMirrors.Length >= _levels.Length - 1 ? _levels.Length - 1 : _allHitMirrors.Length];
        if (AllActivatedMirrors != _allActivatedMirrors_prev)
            OnPlayerSwitchLevel?.Invoke();
        _allActivatedMirrors_prev = AllActivatedMirrors;

    }
    private void LevelColliderControl() 
    {
        foreach (Mirror m in _hoodMirrors)
            m.ToggleBoxesRigidCollider(CheckHoodeMirrorSliable());

        foreach (Mirror m in _mirrors.Where(x => !_hoodMirrors.Contains(x)).ToArray())
            m.ToggleBoxesRigidCollider(CheckFreeMirrorEnterable());
    }

    private bool CheckFreeMirrorEnterable()
    {
        foreach (Collider c in _overlappedCollider)
        {
            if (c.gameObject.GetComponentInParent<Level>())
            {
                Level localLevel = c.gameObject.GetComponentInParent<Level>();
                if (localLevel == _nextLevel)
                    return true;
            }
        }
        return false;
    }
    private bool CheckHoodeMirrorSliable()
    {
        foreach (Collider c in _overlappedCollider)
        {
            if (c.gameObject.GetComponentInParent<Level>())
            {
                Level localLevel = c.gameObject.GetComponentInParent<Level>();
                if (_currentLevel == _nextLevel ? localLevel == _lastLevel || localLevel == _nextLevel : localLevel == _lastLevel)
                    return true;
            }
        }
        return false;
    }


   /* private bool IsPlayerUnderMirror(int rayCount, float radius, float passThreshold)
    {
        circularRayCasts = new RaycastHit[rayCount];
        int notPassed = 0;
        float incement = (Mathf.PI * 2) / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float x = Mathf.Cos(i * incement) * radius;
            float y = Mathf.Sin(i * incement) * radius;
            Vector3 castingPos = transform.position - Vector3.up * 3f + new Vector3(x, 0, y);
            RaycastHit[] hits = Physics.RaycastAll(castingPos, Vector3.up, 20f, _mirrorRayCastMask);
            foreach (RaycastHit h in hits)
                if (h.collider.tag.Equals("Mirror"))
                    circularRayCasts[i] = h;
        }
        foreach (RaycastHit hit in circularRayCasts)
            if (hit.collider == null)
                notPassed++;

        return (notPassed / (float)rayCount) < 1 - passThreshold;
    }
    private void OnDrawGizmos()
    {
        if (circularRayCasts != null)
            foreach (RaycastHit r in circularRayCasts)
            {
                Gizmos.DrawSphere(r.point, 0.01f);
            }
    }*/
}
