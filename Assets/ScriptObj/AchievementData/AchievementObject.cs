using System;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "TTOBG/AchievementObject")]
public class AchievementObject : ScriptableObject
{
    public AchievementObject[] RequiredAchievementsToUnlock;
    public EventObject[] EventToRaiseWhenUnlocked;
    public bool isUnlocked = false;
    public EventObject[] EventToRaiseWhenAccomplished;
    public bool isAccomplished = false;

    public static Action<AchievementObject> OnAchievementAccomplished;

    public void TryUnlock() 
    {
        if (RequiredAchievementsToUnlock.Length == 0) 
        {
            Unlock();
            return;
        }
          
        if (RequiredAchievementsToUnlock.Select(x => x.isAccomplished).Contains(false))
            return;


        Unlock();
    }

    private void Unlock() 
    {
        isUnlocked = true;
        if (!isAccomplished)
        foreach (EventObject e in EventToRaiseWhenUnlocked)
            e?.Raise();
    }
    public void Accomplish() 
    {
        if (!isUnlocked)
            return;
        isAccomplished = true;
        OnAchievementAccomplished?.Invoke(this);
        foreach (EventObject e in EventToRaiseWhenAccomplished) 
            e?.Raise();
    }
    public void ResetFromHere() 
    {
    
    }
}
