
using UnityEngine;
using System;

public class NPC_Controller : RationalObject,IInteractable
{ 
    public Action<Vector3, TextAsset, Sprite> OnDetactPlayer;
    public Action OnLostPlayer;
    public TextAsset InkDialogueAsset;
    public Sprite IconImage;
    public static Action<TextAsset, Sprite> OnPlayDialogue;

    public int interactionCounter { get; private set; } = 0;
    public DialogueProgressionSetting[] ProgressionSettings;

    [Serializable]
    public class DialogueProgressionSetting 
    {
        [Header (">>>>>>>>>>> Always Move to the Next Interaction When Clicked")]
        public bool progressRegardless;

        [Header(">>>>>>>>>>> Only Move to the Next Interaction When Condition Satisfied")]
        public NPC_Controller npcToCompare;
        public int atSpecificDialogueToProgress;

        [Header(">>>>>>>>>>> Only Move to the Next Interaction When Event is Called")]
        public string progressEventName;
    }
    private void OnEnable()
    { 
        DialogueManager.OnGeneralEventCalledGlobal+= ReceiveGeneralEvent;
    }
    private void OnDisable()
    {
        DialogueManager.OnGeneralEventCalledGlobal-= ReceiveGeneralEvent;
    }
    public void Interact() 
    {
        NPC_Manager.currentNPC = this;
        OnPlayDialogue?.Invoke(InkDialogueAsset, IconImage);
    }
    public void Hold() { }

    public void Disengage() { }
    public bool IsVisible() 
    {
        return IsObjectVisibleAndSameLevelWithPlayer();
    }

    public IconType GetIconType() 
    {
        return IconType.exclamation;
    }
    void ReceiveGeneralEvent(string value) 
    {
        var stage = ProgressionSettings[interactionCounter <= ProgressionSettings.Length - 1 ? interactionCounter : ProgressionSettings.Length - 1];
        if (stage.progressEventName != null)
            if (stage.progressEventName == value)
                interactionCounter ++;
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
