using UnityEngine;
using UnityEngine.SceneManagement;
public class AchievementManager : MonoBehaviour
{
    public AchievementObject[] Achievements;

    private void OnEnable()
    {
        TryUnlockAllAchievements(null);
        AchievementObject.OnAchievementAccomplished += TryUnlockAllAchievements;
        SceneManager.activeSceneChanged += OnSceneLoaded;
    }
    private void OnDisable()
    {
        ResetAllAchievements();
        AchievementObject.OnAchievementAccomplished -= TryUnlockAllAchievements;
        SceneManager.activeSceneChanged -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s1, Scene s2) 
    {
       
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
