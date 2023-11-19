using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Canvas DialoguePanel;
    private bool isDialoguePlaying = false;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueIcon ;
    private Story currentDialogue_NPC;
    private TextAsset currentDialogueText_NPC;
    private Sprite currentIcon_NPC;

    private void OnEnable()
    {
        NPC_Manager.OnReadyToPlayDialogue += PlayerReadyToPlayDialogue;
    }
    private void OnDisable()
    {
        NPC_Manager.OnReadyToPlayDialogue -= PlayerReadyToPlayDialogue;

    }
    public void Awake()
    {
        DialoguePanel.gameObject.SetActive(false);
    }
    void PlayerReadyToPlayDialogue(TextAsset inkStory_NPC, Sprite icon)
    {
        currentDialogueText_NPC = inkStory_NPC;
        currentIcon_NPC = icon;
    }

    private void Update()
    {
        if (currentDialogueText_NPC) 
        {
            if (Input.GetKeyDown(KeyCode.Space))
                EnterDialogueMode(currentDialogueText_NPC, currentIcon_NPC);
        }
    }

    void EnterDialogueMode(TextAsset inkJson_NPC,Sprite icon) 
    {
        if (!isDialoguePlaying) 
        {
            isDialoguePlaying = true;
            DialoguePanel.gameObject.SetActive(true);
            PlayerMove.canUseWASD = false;
            currentDialogue_NPC = new Story(inkJson_NPC.text);
            dialogueIcon.sprite = icon;
        }
        
        if (currentDialogue_NPC.canContinue)
            dialogueText.text = currentDialogue_NPC.Continue();
        else
            ExitDialogueMode();

    }

    void ExitDialogueMode()
    {
        isDialoguePlaying = false;
        DialoguePanel.gameObject.SetActive(false);
        PlayerMove.canUseWASD = true;
        dialogueText.text = null;
        dialogueIcon.sprite = null;
    }
}
