
using UnityEngine;

public class SceneInfo
{
    public Vector3 PlayerPos { get; private set; }
    public void SetPlayerPos(Vector3 value) 
    {
        PlayerPos = value;
    }
    public Vector3[] MirrorPos { get; private set; }
    public void SetMirrorPos(Vector3[] value) 
    {
        MirrorPos = value;
    }

}
