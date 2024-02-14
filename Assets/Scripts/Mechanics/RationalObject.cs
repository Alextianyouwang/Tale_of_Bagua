using UnityEngine;
using UnityEngine.UIElements;

public class RationalObject : MonoBehaviour
{
    protected Collider[] _overlappingColliders;
    protected RaycastHit[] _allHitsMirrors;
    protected Level[] _levels;
    public LayerMask ObstacleMask, MirrorMask;
    public int _levelIndex = 0;

    private void OnEnable()
    {
        LevelManager.OnShareAllLevels += ReceiveAllLevels;
    }
    private void OnDisable()
    {
        LevelManager.OnShareAllLevels -= ReceiveAllLevels;

    }
    private void ReceiveAllLevels(Level[] level)
    {
        _levels = level;
    }

    protected bool FreeToProceed(Vector3 position, float objectRadius)
    {
        _overlappingColliders = Physics.OverlapSphere(position, objectRadius, ObstacleMask);
        _allHitsMirrors = Physics.RaycastAll(position - Vector3.up * 3f, Vector3.up, 20f, MirrorMask);
        if (_allHitsMirrors.Length == 0)
            return false;
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

    protected bool HandShake(string tagName, Vector3 position, Vector3 distance, out RaycastHit hit)
    {
        Ray handshake = new Ray(position, distance.normalized);
        Physics.Raycast(handshake, out hit, distance.magnitude);
        if (hit.transform != null && hit.transform.tag.Equals(tagName))
            return true;
        else
            return false;
    }

    public bool IsObjectVisibleAndSameLevelWithPlayer()
    {
        return
            LevelManager.allMirrorOnTop == Physics.RaycastAll(transform.position - Vector3.up * 3f, Vector3.up, 20f, MirrorMask).Length - 1
            &&
            LevelManager.allMirrorOnTop == _levelIndex;
    }
    public bool IsObjectAtCorrectLevel() 
    {
        return Physics.RaycastAll(transform.position - Vector3.up * 3f, Vector3.up, 20f, MirrorMask).Length - 1 == _levelIndex;
    }

    protected bool CheckVisibility(RaycastHit hit)
    {
        if (hit.transform.GetComponent<RationalObject>())
            return hit.transform.GetComponent<RationalObject>().IsObjectAtCorrectLevel();
        return false;
    }


    protected void Connect(RaycastHit hit)
    {
        hit.transform.GetComponent<RationalObject>().Receive();
    }

    protected void Receive()
    {
        print(name);
    }

}
