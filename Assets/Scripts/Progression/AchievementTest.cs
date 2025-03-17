using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementTest : RationalObject, IInteractable, IDataPersistence
{
    public AchievementObject Achievement;
    public string key;
    public void Interact(Vector3 pos) 
    {
        Achievement.Accomplish();

    }
    public void SaveData(ref GameData data)
    {
        if (data.TestObjectStates.ContainsKey(key))
            data.TestObjectStates.Remove(key);
        data.TestObjectStates.Add(key, (int)Achievement.State);
    }

    public void LoadData(GameData data)
    {
    }
    public void ChangeState(Dictionary<string,int> data) 
    {
        int value = 0;
        if (data.Keys.Contains(key))
            value = data[key];
        switch (value)
        {
            case 0:
                break;


            case 1:
                ChangeToYellow();
                break;


            case 2:
                ChangeToGreen();
                break;
        }
    }
    public void ChangeToYellow()
    {
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.yellow);
    }
    public void ChangeToGreen()
    {
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);

    }
    public void DetactPlayer() { }

    public void Hold() { }
    public void Disengage() { }

    public IconType GetIconType() { return IconType.exclamation; }
    public bool IsVisible() { return IsObjectVisibleAndSameLevelWithPlayer(); }
    public bool IsActive() { return true; } 
}
