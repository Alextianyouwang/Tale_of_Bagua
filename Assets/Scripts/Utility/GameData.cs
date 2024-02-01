
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public SceneInfo[] SceneInfos { get; private set; }
    public void SetSceneInfos(SceneInfo[] value) 
    {
        SceneInfos = value;
    }
}
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
    public SceneInfo() {
        MirrorPos = new Vector3[4];
    }

}

public interface IDataPersistence
{
    public void SaveData(GameData data);
    public void LoadData(GameData data);
}
