using UnityEngine;
using System;
using System.Linq;

public class MirrorManager : MonoBehaviour
{

    public static Action<Mirror> OnSharingCurrentMirror;
    public static Action<float, float> OnChargingCollapse;
    public static Action<float, float> OnChargingRelease;
    public static Action<float, float> OnChargedCollapse;
    public static Action<float, float, bool> OnAbortCollapse;
    public static Action OnCollapsing;
    public static Action<bool> OnExpand;
    public static bool CanUseRightClick = true, CanUseLeftClick = true;

    private Mirror _currentMirror;
    private LayerMask _mirrorMask;
    private Vector3 _offset, _finalWorldPos, _screenCenter_WorldSpace;
    private bool _firstMirrorHasBeenClicked = false, _isClicking = false, _isCollapsed = false, _isCharged = false, _canChargeAgain = true;

    private float _collapseTimer = 0,_chargeTime = 0.01f;
    private int _previousHoodMirrorCount = 0,_currentHoodMirrorCount = 0;

    public AnimationCurve mirrorMoveCurve;
    [ColorUsage(true,true)]
    public Color normalCol;
    [ColorUsage(true, true)]
    public Color selectedCol;
    [ColorUsage(true, true)]
    public Color hoodCol;
    

    private void OnEnable()
    {
        _mirrorMask = LayerMask.GetMask("MirrorPlane");
        _screenCenter_WorldSpace = Utility.GetScreenCenterPosition_WorldSpace();
        LevelManager.OnFixUpdate += FollowFixUpdate;
    }
    private void OnDisable()
    {
        LevelManager.OnFixUpdate -= FollowFixUpdate;

        CanUseRightClick = true;
        CanUseLeftClick= true;
    }

    void UpdateMirrorPhysics()
    {
        if (!_isClicking) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] allHits = Physics.RaycastAll(ray, Mathf.Infinity, _mirrorMask);
        _finalWorldPos = allHits.Where(x => x.transform.tag.Equals("MirrorPlane")).FirstOrDefault().point;
        if (!_firstMirrorHasBeenClicked)
        {
            _firstMirrorHasBeenClicked = true;
            _currentMirror = allHits.Where(x => x.transform.tag.Equals("Mirror")).Select(x => x.transform.gameObject.GetComponent<Mirror>()).FirstOrDefault();
            if (_currentMirror != null) 
            _offset =_currentMirror.transform.position - _finalWorldPos;
        }
    }
 

    public void MoveMirrorTo(Mirror m, Vector3 target, float speed ) 
    {
        Vector3 mirrorCenter = m.transform.position - _offset;
        Vector3 direction = Vector3.Normalize(target - mirrorCenter);
        float distance = (_finalWorldPos - mirrorCenter).magnitude;
        m.RigidBodyAddForce(direction, Mathf.Min(distance * speed, 15));
     
    }

    private void SetMirrorYPos() 
    {
        foreach(Mirror m in LevelManager._Mirrors) 
            m.RigidBody.position = new Vector3(m.RigidBody.position.x, _screenCenter_WorldSpace.y, m.RigidBody.position.z);
    }

    private void CageMirrorWhenCollapsed() 
    {
        foreach (Mirror m in LevelManager._HoodMirrors)
            if (_isCollapsed)
                m.ToggleBoxesRigidCollider(true);
    }

    private void AdjustMirrorColliderSize() 
    {
        foreach (Mirror m in LevelManager._HoodMirrors)
            m.ToggleColliderSize(true);
        foreach (Mirror m in LevelManager._Mirrors.Where(x => !LevelManager._HoodMirrors.Contains(x))) 
            m.ToggleColliderSize(false);
    }


    private void FollowFixUpdate() 
    {
        UpdateMirrorPhysics();
        SetMirrorYPos();
        CageMirrorWhenCollapsed();
        AdjustMirrorColliderSize();

        if (!_currentMirror || !_firstMirrorHasBeenClicked) 
            return;
 
        if (_isCollapsed && LevelManager._HoodMirrors.Contains(_currentMirror))
            for (int i = 0; i < LevelManager._HoodMirrors.Length; i++)
                MoveMirrorTo(LevelManager._HoodMirrors[i], _finalWorldPos, 7);
        else
             MoveMirrorTo(_currentMirror, _finalWorldPos, 7);
    }

    public void CollapseHoodMirror()
    {
        if (LevelManager._HoodMirrors.Length <=1)
            return;
        _isCollapsed = true;
        Vector3 averagePos = Vector3.zero;
        foreach (Mirror m in LevelManager._HoodMirrors)
        {
            averagePos += m.transform.position;

        }
        averagePos /= LevelManager._HoodMirrors.Length;
        foreach (Mirror m in LevelManager._HoodMirrors) 
        {
            m.ToggleBoxesRigidCollider(true);
            m.MoveMirrorTowards(0.4f, averagePos,mirrorMoveCurve);

        }
    }

    public void ExpandHoodMirror() 
    {
        if (LevelManager._HoodMirrors.Length <= 1)
            return;
        _isCollapsed = false;
        Vector2[] offsets = Utility.RadiusPosition(LevelManager._HoodMirrors.Length);
        Vector3 playerPos = PlayerMove.playerTransform.position;

        for (int i = 0; i < LevelManager._HoodMirrors.Length; i++)
        {
            Vector2 offset = offsets[i];
            Vector3 targetPos = new Vector3(playerPos.x + offset.x, LevelManager._HoodMirrors[0].transform.position.y, playerPos.z+ offset.y);
            LevelManager._HoodMirrors[i].MoveMirrorTowards(0.4f, targetPos, mirrorMoveCurve);
        }
    }

    void CheckIfNewHoodMirrorAdded() 
    {
        if (LevelManager._HoodMirrors == null) return;
        if( LevelManager._HoodMirrors.Length == 0 )return;
        _currentHoodMirrorCount = LevelManager._HoodMirrors.Length;
        if (_previousHoodMirrorCount != _currentHoodMirrorCount && _isCollapsed) 
        {
            CollapseHoodMirror();
        }
        _previousHoodMirrorCount = _currentHoodMirrorCount;
    }

    void LeftClick() {
        if (!CanUseLeftClick)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            _isClicking = true;

        }
        if (Input.GetMouseButtonUp(0))
        {

            _isClicking = false;
            _currentMirror = null;
            _firstMirrorHasBeenClicked = false;
            _offset = Vector3.zero;
        }
    }
    void RightClick() 
    {
        if (!CanUseRightClick)
            return;

        if (_isCollapsed && !Input.GetMouseButton(1))
            OnCollapsing?.Invoke();
        if (Input.GetMouseButtonDown(1))
        {

            if (_isCollapsed)
            {
                _canChargeAgain = false;
                OnExpand?.Invoke(false);
                ExpandHoodMirror();

            }

        }
        if (Input.GetMouseButton(1))
        {
            if (!_canChargeAgain)
                return;
            _collapseTimer += Time.deltaTime;
            if (!_isCollapsed)
                OnChargingCollapse?.Invoke(_collapseTimer, _chargeTime);


        }
        if (Input.GetMouseButtonUp(1))
        {
            if (!_canChargeAgain)
            {
                _isCharged = false;
                _canChargeAgain = true;
                return;
            }
            if (_collapseTimer >= _chargeTime)
            {

                OnChargedCollapse?.Invoke(_collapseTimer, _chargeTime);
                if (!_isCollapsed)
                {
                    CollapseHoodMirror();
                    _isCharged = true;
                }
            }
            else
            {
                if (!_isCharged)
                    OnAbortCollapse?.Invoke(_collapseTimer, _chargeTime, false);
            }
            _collapseTimer = 0;
        }

    }
    void UpdateInput() 
    {
        LeftClick();
        RightClick();
    }

    void SetMirrorColor(Mirror m, Color color) 
    {
        m.Materials[2].color = color;  
    }

  
    void UpdateMaterial() 
    {
        if (LevelManager._Mirrors != null)
        foreach (Mirror m in LevelManager._Mirrors)
                    if (m && m == _currentMirror)
                        SetMirrorColor(m, selectedCol);
                    else if (m && m != _currentMirror)
                        SetMirrorColor(m, normalCol);
        if (LevelManager._HoodMirrors != null && LevelManager._HoodMirrors.Length > 0) 
        {
            foreach (Mirror mr in LevelManager._HoodMirrors)
                if (mr)
                    SetMirrorColor(mr, hoodCol);
            foreach (Mirror m in LevelManager._HoodMirrors)
                if (m)
                    if (m == _currentMirror)
                        SetMirrorColor(m, selectedCol);
                    else if (m && m != _currentMirror)
                        SetMirrorColor(m, hoodCol);
            if (_isCollapsed && _currentMirror)
                foreach (Mirror m in LevelManager._HoodMirrors)
                    if (m)
                        SetMirrorColor(m, selectedCol);
        }
           
   
    }
    void Update()
    {

 
        UpdateInput();
        UpdateMaterial();
        CheckIfNewHoodMirrorAdded();

        OnSharingCurrentMirror?.Invoke(_currentMirror);


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(_currentMirror)
        Gizmos.DrawLine(_finalWorldPos, _currentMirror.transform.position - _offset);
    }
}
