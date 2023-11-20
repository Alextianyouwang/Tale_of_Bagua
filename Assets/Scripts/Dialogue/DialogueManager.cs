
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Canvas DialoguePanel;
    private bool isDialoguePlaying = false;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueIcon ;
    private Story currentDialogue_NPC;
    public Func<string,bool,object> OnGeneralEventCalled;
    public static Action<string> OnGeneralEventCalledGlobal;
    private Dictionary<string, int> GeneralEventRecorder = new Dictionary<string, int>();

    private List<string> tags = new List<string>();
    [SerializeField] private GameObject[] choiceButtons;
    private TextMeshProUGUI[] choicesText;
    Sprite playerIcon;
    private void OnEnable()
    {
        PlayerInteract.OnPlayDialogue += EnterDialogueMode;
        OnGeneralEventCalled += ReceiveGeneralEvent;
    }
    private void OnDisable()
    {
        PlayerInteract.OnPlayDialogue -= EnterDialogueMode;
    }
    public void Awake()
    {
        DialoguePanel.gameObject.SetActive(false);

        choicesText = new TextMeshProUGUI[choiceButtons.Length];
        for (int i = 0; i < choicesText.Length; i++) 
        {
            choicesText[i] = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        playerIcon = Resources.Load<Sprite>("Icon/PlayerIcon");
    }

    private void DisplayChoices() 
    {
        List<Choice> currentChoices = currentDialogue_NPC.currentChoices;
        if (currentChoices.Count > choiceButtons.Length)
            Debug.LogError("More choices were given than the UI Could Support");
        int index;
        for (index = 0; index < currentChoices.Count; index++)
        {
            choiceButtons[index].gameObject.SetActive(true);
            choicesText[index].text = currentChoices[index].text;
        }
        for (int i = index; i < choiceButtons.Length; i++) 
        {
            choiceButtons[i].gameObject.SetActive(false);
        }

    }
    public void MakeChoices(int choiceIndex) 
    {
        currentDialogue_NPC.ChooseChoiceIndex(choiceIndex);
        ContinueDialogue();
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
            currentDialogue_NPC.BindExternalFunction("GeneralEvent",  OnGeneralEventCalled);
           
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
        }

        ContinueDialogue();
    }

    void ContinueDialogue() 
    {

        if (currentDialogue_NPC.canContinue)
        {
            dialogueText.text = currentDialogue_NPC.Continue();
            DisplayChoices();
            ParseTags();
        }
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
        PlayerInteract.currentNPC.UpdateInteractionAfterPrint();

    }

    void ParseTags()
    {
        tags = currentDialogue_NPC.currentTags;
        foreach (string t in tags)
        {
            string prefix = t.Split(' ')[0];
            string param = t.Split(' ')[1];

            switch (prefix.ToLower())
            {
                case "type":
                    if (param == "player") 
                    {
                        SetPlayerIcon();
                    }
                    break;
                default: break;
            }
        }
    }
    void SetPlayerIcon() 
    {
        dialogueIcon.sprite = playerIcon;
    }
    object ReceiveGeneralEvent(string eventName, bool onlyAllowOnce)
    {
        if (!GeneralEventRecorder.ContainsKey(eventName)) 
        {
            GeneralEventRecorder.Add(eventName, 1);
            OnGeneralEventCalledGlobal?.Invoke(eventName);
        }
        else
        {
            if (!onlyAllowOnce) 
                OnGeneralEventCalledGlobal?.Invoke(eventName);
           KeyValuePair<string,int> selected = GeneralEventRecorder.Where(x => x.Key == eventName).FirstOrDefault();
            GeneralEventRecorder[selected.Key]++;
        }
        
        return null;
    }
}
