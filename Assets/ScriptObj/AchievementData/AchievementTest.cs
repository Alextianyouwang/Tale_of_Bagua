using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AchievementTest : RationalObject, IInteractable, IDataPersistence
{
    public AchievementObject Achievement;
    public string key;
    public void Interact() 
    {
        Achievement.Accomplish();
    }
    public void SaveData(ref GameData data)
    {
        data.TestObjectStates.Clear();
        data.TestObjectStates.Add(key, (int)Achievement.State);
    }

    public void LoadData(GameData data)
    {
        int value = 0;
        if (data.TestObjectStates.Keys.Contains(key))
            value = data.TestObjectStates[key];

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
        print(value);

    }

    public void ChangeToYellow()
    {
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.yellow);
    }
    public void ChangeToGreen()
    {
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);

    }
    public void Hold() { }
    public void Disengage() { }

    public IconType GetIconType() { return IconType.exclamation; }
    public bool IsVisible() { return IsObjectVisibleAndSameLevelWithPlayer(); }
    public bool IsActive() { return true; } 
}
