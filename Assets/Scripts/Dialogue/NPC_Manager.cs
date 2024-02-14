using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    public static NPC_Controller currentNPC;
    private GameObject exclamationIcon_prefab;
    private GameObject exclamationIcon_instance;
    public GameObject[] NPC_Levels;
    private GameObject currentNPCLevel;

    private void Awake()
    {
        
        exclamationIcon_prefab = Resources.Load<GameObject>("UI/P_ExclamationIcon");
        exclamationIcon_instance = Instantiate(exclamationIcon_prefab);
        exclamationIcon_instance.SetActive(false);
    }

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
