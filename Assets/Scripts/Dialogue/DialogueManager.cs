
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Canvas DialoguePanel;
    private bool isDialoguePlaying = false;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueIcon ;
    private Story currentDialogue_NPC;
    private void OnEnable()
    {
        PlayerInteract.OnPlayDialogue += EnterDialogueMode;
    }
    private void OnDisable()
    {
        PlayerInteract.OnPlayDialogue -= EnterDialogueMode;
    }
    public void Awake()
    {
        DialoguePanel.gameObject.SetActive(false);
    }

    void EnterDialogueMode(TextAsset inkJson_NPC,Sprite icon) 
    {
        if (!isDialoguePlaying)
        {
            isDialoguePlaying = true;
            DialoguePanel.gameObject.SetActive(true);
            PlayerMove.canUseWASD = false;
            MirrorManager.canUseLeftClick = false;
            MirrorManager.canUseRightClick = false;
            dialogueIcon.sprite = icon;
            currentDialogue_NPC = new Story(inkJson_NPC.text);
            PlayerInteract.currentNPC.UpdateInteractionBeforePrint();
            print(PlayerInteract.currentNPC.interactionCounter);
            try
            {
                currentDialogue_NPC.ChoosePathString($"{PlayerInteract.currentNPC.interactionCounter}Interaction");
            }
            catch (StoryException)
            {
                currentDialogue_NPC.ChoosePathString("Fallback");
            }
            if (currentDialogue_NPC.canContinue)
                dialogueText.text = currentDialogue_NPC.Continue();
            else
                ExitDialogueMode();
       
        }
        else
        {

            if (currentDialogue_NPC.canContinue)
                dialogueText.text = currentDialogue_NPC.Continue();
            else
                ExitDialogueMode();
        }
       

    }

    void ExitDialogueMode()
    {
        isDialoguePlaying = false;
        DialoguePanel.gameObject.SetActive(false);
        PlayerMove.canUseWASD = true;
        MirrorManager.canUseLeftClick = true;
        MirrorManager.canUseRightClick = true;
        dialogueText.text = null;
        dialogueIcon.sprite = null;
        PlayerInteract.currentNPC.UpdateInteractionAfterPrint();

    }
}
