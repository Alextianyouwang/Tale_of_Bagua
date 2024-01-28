using UnityEngine;

public class LevelInfoAnnouncer : MonoBehaviour
{
    public int GetLayerNumber() 
    {
        Level[] levels = FindObjectsOfType<Level>();
        return levels.Length;
    }
}
