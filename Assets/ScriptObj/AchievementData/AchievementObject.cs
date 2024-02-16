using System;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "TTOBG/AchievementObject")]
public class AchievementObject : ScriptableObject
{
    public AchievementObject[] RequiredAchievementsToUnlock;
    public EventObject[] EventToRaiseWhenUnlocked;
    public EventObject[] EventToRaiseWhenAccomplished;

    public enum AchievementStates { Locked, Unlocked, Accomplished}
    public AchievementStates State = AchievementStates.Locked;

    public static Action<AchievementObject> OnAchievementAccomplished;

    public void TryUnlock() 
    {
        if (RequiredAchievementsToUnlock.Length == 0) 
        {
            Unlock();
            return;
        }
          
        if (RequiredAchievementsToUnlock.Select(x => x.State).Contains(AchievementStates.Locked) ||
            RequiredAchievementsToUnlock.Select(x => x.State).Contains(AchievementStates.Unlocked))
            return;


        Unlock();
    }

    private void Unlock() 
    {
        if (State != AchievementStates.Locked)
            return;
        State = AchievementStates.Unlocked;
        foreach (EventObject e in EventToRaiseWhenUnlocked)
            e?.Raise();
    }
    public void Accomplish() 
    {
        if (State != AchievementStates.Unlocked)
            return;
        State = AchievementStates.Accomplished;
        OnAchievementAccomplished?.Invoke(this);
        foreach (EventObject e in EventToRaiseWhenAccomplished) 
            e?.Raise();
    }
    public void ResetFromHere() 
    {
    
    }
}
