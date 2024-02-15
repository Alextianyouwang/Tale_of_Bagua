using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public AchievementObject[] Achievements;

    private void OnEnable()
    {
        TryUnlockAllAchievements(null);
        AchievementObject.OnAchievementAccomplished += TryUnlockAllAchievements;
    }
    private void OnDisable()
    {
        ResetAllAchievements();
        AchievementObject.OnAchievementAccomplished -= TryUnlockAllAchievements;
    }

    private void TryUnlockAllAchievements(AchievementObject target) 
    {
        foreach (AchievementObject a in Achievements) 
            a.TryUnlock();
    }

    private void ResetAllAchievements() 
    {

        foreach (AchievementObject a in Achievements) 
        {
            a.isUnlocked = false;
            a.isAccomplished = false;
        }
            
    }

}
