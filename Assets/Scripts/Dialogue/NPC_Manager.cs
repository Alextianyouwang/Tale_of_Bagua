using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    public static NPC_Controller currentNPC;
    public GameObject[] NPC_Levels;
    private GameObject currentNPCLevel;


    private void OnEnable()
    {
        LevelManager.OnFixUpdate += CheckNPCLevel;
    }

    private void OnDisable()
    {
        LevelManager.OnFixUpdate -= CheckNPCLevel;
    }
    void CheckNPCLevel() 
    {
        currentNPCLevel = NPC_Levels[LevelManager.allMirrorOnTop];
        Utility.ToggleChildGameobjectColliderActivation(true, currentNPCLevel);
        foreach (GameObject level in NPC_Levels)
            if (level != currentNPCLevel)
                Utility.ToggleChildGameobjectColliderActivation(false, level);
 
    }
}
