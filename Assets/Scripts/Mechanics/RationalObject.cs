using System;
using UnityEngine;
public class RationalObject : MonoBehaviour
{
    public LayerMask ObstacleMask, MirrorMask;
    public int _levelIndex = 0;

    protected Collider[] _overlappingColliders;
    protected RaycastHit[] _allHitsMirrors;
    protected Level[] _levels;

    public Action<LazerEmitter.Oriantations> OnReceive;
    

    protected bool FreeToProceed(Vector3 position, float objectRadius)
    {
        _overlappingColliders = Physics.OverlapSphere(position, objectRadius, ObstacleMask);
        _allHitsMirrors = Physics.RaycastAll(position - Vector3.up * 3f, Vector3.up, 20f, MirrorMask);
        if (_allHitsMirrors.Length == 0)
            return false;
        Level currentLevel =LevelManager._Levels[_allHitsMirrors.Length - 1];
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

    protected bool HandShake( Vector3 position, Vector3 distance, out RaycastHit hit)
    {
        Ray handshake = new Ray(position, distance.normalized);
        Physics.Raycast(handshake, out hit, distance.magnitude);
        if (hit.transform != null && hit.transform.GetComponent<RationalObject>())
            return true;
        else
            return false;
    }

    public bool IsObjectVisibleAndSameLevelWithPlayer()
    {
        return
           IsObjectAtCorrectLevel()
            &&
            LevelManager.AllActivatedMirrors == _levelIndex;
    }
    public bool IsObjectAtCorrectLevel() 
    {
        Bounds b = GetComponent<MeshRenderer>().bounds;
        Vector3 tl = b.center + new Vector3(-b.extents.x, 0, b.extents.z);
        Vector3 tr = b.center + new Vector3(b.extents.x, 0, b.extents.z);
        Vector3 br = b.center + new Vector3(b.extents.x, 0, -b.extents.z);
        Vector3 bl = b.center + new Vector3(-b.extents.x, 0, -b.extents.z);
        int tlHit = Physics.RaycastAll(tl - Vector3.up * 3f, Vector3.up, 20f, MirrorMask).Length - 1;
        int trHit = Physics.RaycastAll(tr - Vector3.up * 3f, Vector3.up, 20f, MirrorMask).Length - 1;
        int brHit = Physics.RaycastAll(br - Vector3.up * 3f, Vector3.up, 20f, MirrorMask).Length - 1;
        int blHit = Physics.RaycastAll(bl - Vector3.up * 3f, Vector3.up, 20f, MirrorMask).Length - 1;
        if (tlHit != _levelIndex
            || trHit != _levelIndex
            || brHit != _levelIndex
            || blHit != _levelIndex)
            return false;
        else return true;

    }

    protected bool CheckVisibility(RaycastHit hit)
    {
        if (hit.transform.GetComponent<RationalObject>())
            return hit.transform.GetComponent<RationalObject>().IsObjectAtCorrectLevel();
        return false;
    }

    protected virtual void OnDrawGizmos()
    {

    }
    public void Receive(LazerEmitter.Oriantations oriantation)
    {
        OnReceive?.Invoke(oriantation);
    }

}
