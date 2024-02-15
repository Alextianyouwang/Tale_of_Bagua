using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementTest : RationalObject, IInteractable
{
    public AchievementObject Achievement;
    public void Interact() 
    {
        Achievement.Accomplish();
    }

    public void Hold() { }
    public void Disengage() { }

    public IconType GetIconType() { return IconType.exclamation; }
    public bool IsVisible() { return IsObjectVisibleAndSameLevelWithPlayer(); }
    public bool IsActive() { return true; } 
}
