
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    [SerializeField] public SceneInfo[] SceneInfos { get; set; }
    [SerializeField] public int CurrentScene { get;  set; }
    [SerializeField] public AchievementObject.AchievementStates[] AchievementStates { get;  set; }
    [SerializeField] public Dictionary<string, int> TestObjectStates { get; set; }
}
[System.Serializable]
public class SceneInfo
{
    [SerializeField] public Vector3 PlayerPos { get; set; }
    [SerializeField] public Vector3[] MirrorPos { get; set; }

    public SceneInfo() {
        MirrorPos = new Vector3[4];
    }

}
[System.Serializable]
public class AchievementObjectStates 
{
    [SerializeField] public bool isUnlocked;
    [SerializeField] public bool isAccomplished;
}
public interface IDataPersistence
{
    public void SaveData(ref GameData data);
    public void LoadData(GameData data);
}
