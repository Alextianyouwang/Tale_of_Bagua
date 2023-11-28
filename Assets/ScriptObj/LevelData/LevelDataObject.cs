using System.Linq;
using UnityEngine;

[CreateAssetMenu (menuName = "TTOBG/LevelObject")]
public class LevelDataObject : ScriptableObject
{
    public int HorizontalChunk;
    public int VerticalChunk;
    //[Header("*Nest vertical inside horizontal when looping to read from this list properly")]
    [SerializeField]
    private bool[] LevelDataArray;

    public bool[] GetLevelDataArray() 
    {
        return LevelDataArray;
    }

    public void SetLevelDataArray(bool[] value) 
    {
        LevelDataArray = value.ToArray();
    }
}
