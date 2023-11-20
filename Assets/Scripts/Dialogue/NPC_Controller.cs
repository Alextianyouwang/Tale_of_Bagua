
using UnityEngine;
using System;
using UnityEditor.SceneManagement;

public class NPC_Controller : MonoBehaviour
{
    public Action<Vector3, TextAsset, Sprite> OnDetactPlayer;
    public Action OnLostPlayer;
    public TextAsset InkDialogueAsset;
    public Sprite IconImage;
    

    public int interactionCounter { get; private set; } = 0;
    public DialogueProgressionSetting[] ProgressionSettings;

    [Serializable]
    public class DialogueProgressionSetting 
    {
        public NPC_Controller npcToCompare;
        public int atSpecificDialogueToProgress;
        public bool progressRegardless;
    }

    public void UpdateInteractionBeforePrint() 
    {
        var stage = ProgressionSettings[interactionCounter <= ProgressionSettings.Length -1?interactionCounter : ProgressionSettings.Length -1];
        if (stage == null) 
        {
            print($"Next Progression Condition has not been set for {name}, next dialogue will not displayed.");
            return;
        }
        if (stage.progressRegardless)
            return;
        if (stage.npcToCompare.interactionCounter == stage.atSpecificDialogueToProgress)
            interactionCounter++;

        if (interactionCounter > ProgressionSettings.Length)
            interactionCounter = ProgressionSettings.Length;

    }

    public void UpdateInteractionAfterPrint()
    {
        var stage = ProgressionSettings[interactionCounter <= ProgressionSettings.Length - 1 ? interactionCounter : ProgressionSettings.Length - 1];
        if (stage == null)
        {
            print($"Next Progression Condition has not been set for {name}, next dialogue will not displayed.");
            return;
        }
        if (stage.progressRegardless)
            interactionCounter++;

        if (interactionCounter > ProgressionSettings.Length)
            interactionCounter = ProgressionSettings.Length;
    }

}
