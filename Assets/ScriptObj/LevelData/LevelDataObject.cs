using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu (menuName = "TTOBG/LevelObject")]
public class LevelDataObject : ScriptableObject
{
    public int HorizontalChunk;
    public int VerticalChunk;
    [SerializeField]
    private bool[] LevelDataArray;

    public bool[] GetLevelDataArray() 
    {
        return LevelDataArray;
    }

    public void SetLevelDataArray(bool[] value) 
    {
        LevelDataArray = value.ToArray();
        EditorUtility.SetDirty(this);
    }
}
