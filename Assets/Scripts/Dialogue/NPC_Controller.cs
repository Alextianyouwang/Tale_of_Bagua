
using UnityEngine;
using System;
using UnityEditor.SceneManagement;
using Palmmedia.ReportGenerator.Core.Reporting.Builders.Rendering;

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

        public bool progressRegardless;

        public NPC_Controller npcToCompare;
        public int atSpecificDialogueToProgress;

        public string progressEventName;
        public int setToWhichInteractionWhenCalled;
    }
    private void OnEnable()
    { 
        DialogueManager.OnGeneralEventCalled += ReceiveGeneralEvent;
    }
    private void OnDisable()
    {
        DialogueManager.OnGeneralEventCalled -= ReceiveGeneralEvent;
    }
    object ReceiveGeneralEvent(string value) 
    {
        var stage = ProgressionSettings[interactionCounter <= ProgressionSettings.Length - 1 ? interactionCounter : ProgressionSettings.Length - 1];
        if (stage.progressEventName != null)
            if (stage.progressEventName == value)
                interactionCounter = stage.setToWhichInteractionWhenCalled;
        return null;
    }
    public void UpdateInteractionBeforePrint() 
    {
        var stage = ProgressionSettings[interactionCounter <= ProgressionSettings.Length -1?interactionCounter : ProgressionSettings.Length -1];
        if (stage.progressRegardless)
            return;
        if (stage.npcToCompare == null)
            return;
        if (stage.npcToCompare.interactionCounter == stage.atSpecificDialogueToProgress)
            interactionCounter++;

        if (interactionCounter > ProgressionSettings.Length)
            interactionCounter = ProgressionSettings.Length;

    }

    public void UpdateInteractionAfterPrint()
    {
        var stage = ProgressionSettings[interactionCounter <= ProgressionSettings.Length - 1 ? interactionCounter : ProgressionSettings.Length - 1];
        if (stage.progressRegardless)
            interactionCounter++;

        if (interactionCounter > ProgressionSettings.Length)
            interactionCounter = ProgressionSettings.Length;
    }

}
