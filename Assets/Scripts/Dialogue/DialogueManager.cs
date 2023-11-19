
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
        MirrorManager.canUseLeftClick = true;
        MirrorManager.canUseRightClick = true;
        dialogueText.text = null;
        dialogueIcon.sprite = null;
    }
}
