
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.EventSystems;
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Canvas DialoguePanel;
    private bool isDialoguePlaying = false;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueIcon ;
    [SerializeField] private Image dialogueContinue;
    private Story currentDialogue_NPC;
    public Func<string,bool,object> OnGeneralEventCalled;
    public static Action<string> OnGeneralEventCalledGlobal;
    private Dictionary<string, int> GeneralEventRecorder = new Dictionary<string, int>();

    private List<string> tags = new List<string>();
    [SerializeField] private GameObject[] choiceButtons;
    private TextMeshProUGUI[] choicesText;

    public static Action<TextMeshProUGUI, string,Func<bool>,Action> OnRequestTyping;
    public static Func<bool> OnCheckingTypingState;
    private Sprite playerIcon;

    [SerializeField] private Animator iconAnimator;
    private void OnEnable()
    {
        NPC_Controller.OnPlayDialogue += EnterDialogueMode;
        OnGeneralEventCalled += ReceiveGeneralEvent;
    }
    private void OnDisable()
    {
        NPC_Controller.OnPlayDialogue -= EnterDialogueMode;
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
        if (currentChoices.Count > 0)
            StartCoroutine(SelectFirstChoice());
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

    public IEnumerator SelectFirstChoice() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        EventSystem.current.SetSelectedGameObject(choiceButtons[0]);
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
            MirrorManager.CanUseLeftClick = false;
            MirrorManager.CanUseRightClick = false;
            currentDialogue_NPC = new Story(inkJson_NPC.text);
            currentDialogue_NPC.BindExternalFunction("GeneralEvent",  OnGeneralEventCalled);
            NPC_Manager.currentNPC.UpdateInteractionBeforePrint();
            print("Current dialogue:" + NPC_Manager.currentNPC.name + " interaction " + NPC_Manager.currentNPC.interactionCounter);
            try
            {
                currentDialogue_NPC.ChoosePathString($"{NPC_Manager.currentNPC.interactionCounter}Interaction");
            }
            catch (StoryException)
            {
                currentDialogue_NPC.ChoosePathString("Fallback");
            }
        }

        iconAnimator.SetBool("ShowOnRight", false);
        dialogueIcon.sprite = icon;
        if (currentDialogue_NPC.currentChoices.Count == 0)
            ContinueDialogue();
    }

    void ContinueDialogue() 
    {
        if (currentDialogue_NPC.canContinue)
        {
            dialogueContinue.gameObject.SetActive(false);
            if (!OnCheckingTypingState())
                OnRequestTyping?.Invoke(dialogueText, currentDialogue_NPC.Continue(), () => Input.GetKeyDown(KeyCode.Space),() => dialogueContinue.gameObject.SetActive(true));
            DisplayChoices();
            ParseTags();
        }
        else
            if (!OnCheckingTypingState())
                ExitDialogueMode();
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
                        dialogueIcon.sprite = playerIcon;
                        iconAnimator.SetBool("ShowOnRight", true);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    void ExitDialogueMode()
    {
        isDialoguePlaying = false;
        DialoguePanel.gameObject.SetActive(false);
        PlayerMove.canUseWASD = true;
        MirrorManager.CanUseLeftClick = true;
        MirrorManager.CanUseRightClick = true;
        dialogueText.text = null;
        dialogueIcon.sprite = null;
        NPC_Manager.currentNPC.UpdateInteractionAfterPrint();

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
