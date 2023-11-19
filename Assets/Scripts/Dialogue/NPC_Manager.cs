using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC_Manager : MonoBehaviour
{
    private NPC_Controller[] npcs;
    private GameObject exclamationIcon_prefab;
    private GameObject exclamationIcon_instance;
    public static Action<TextAsset,Sprite> OnReadyToPlayDialogue;
    public GameObject[] NPC_Levels;
    GameObject currentNPCLevel, previousNPCLevel;

    private void Awake()
    {
        
        npcs = FindObjectsOfType<NPC_Controller>();
        foreach (NPC_Controller n in npcs) 
        {
            n.OnDetactPlayer += DisplayIcon;
            n.OnLostPlayer += HideIcon;
        }
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
        ToggleChildGameobjectColliderActivation(true, currentNPCLevel);
        foreach (GameObject level in NPC_Levels)
            if (level != currentNPCLevel) 
                ToggleChildGameobjectColliderActivation(false, level);
        if (previousNPCLevel != currentNPCLevel)
            HideIcon();
        previousNPCLevel = currentNPCLevel;
    }

    void ToggleChildGameobjectColliderActivation(bool value, GameObject target) 
    {
        if (target.transform.childCount == 0)
            return;
        foreach (Transform t in target.transform) 
            if (t.GetComponent<Collider>())
                t.GetComponent<Collider>().enabled = value;     
    }

    void DisplayIcon(Vector3 pos, TextAsset inkStory_NPC, Sprite icon) 
    {
        exclamationIcon_instance.transform.position = pos;
        exclamationIcon_instance.SetActive(true);
        OnReadyToPlayDialogue?.Invoke(inkStory_NPC,icon);
    }

    void HideIcon()
    {
        exclamationIcon_instance.SetActive(false);
        OnReadyToPlayDialogue?.Invoke(null,null);

    }

}
