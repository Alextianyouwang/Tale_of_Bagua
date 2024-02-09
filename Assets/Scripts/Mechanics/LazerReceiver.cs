using UnityEngine;

public class LazerReceiver : MonoBehaviour
{
    private RaycastHit[] _allHitsMirrors;
    public LayerMask  MirrorMask;
    [SerializeField] private int _levelIndex = 0;

    public bool IsObjectVisible() 
    {
        _allHitsMirrors = Physics.RaycastAll(transform.position - Vector3.up * 3f, Vector3.up, 20f, MirrorMask);
        return _allHitsMirrors.Length - 1 == _levelIndex;
    }

    public void Receive() 
    {

        print(name);
    }
}
