using System;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "TTOBG/AchievementObject")]
public class AchievementObject : ScriptableObject
{
    public AchievementObject[] RequiredAchievementsToUnlock;
    public bool isAccomplished = false;
    public bool isUnlocked = false;
    public static Action<AchievementObject> OnAchievementAccomplished;

    public void TryUnlock() 
    {
        if (RequiredAchievementsToUnlock.Length == 0) 
        {
            isUnlocked = true;
            return;
        }
          
        if (RequiredAchievementsToUnlock.Select(x => x.isAccomplished).Contains(false))
            return;

        isUnlocked = true;
    }
    public void Accomplish() 
    {
        if (!isUnlocked)
            return;
        isAccomplished = true;
        OnAchievementAccomplished?.Invoke(this);
    }
    public void ResetFromHere() 
    {
    
    }
}
