using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class TypeWritter : MonoBehaviour
{

    public bool isTyping { get; private set; } = false;

    private void OnEnable()
    {
        DialogueManager.OnRequestTyping += StartTyping;
        DialogueManager.OnCheckingTypingState += IsTyping;
    }

    private void OnDisable()
    {
        DialogueManager.OnRequestTyping -= StartTyping;
        DialogueManager.OnCheckingTypingState -= IsTyping;
    }

    void StartTyping(TextMeshProUGUI guiText, string content,Func<bool> exitCondition, Action todo) 
    {
        StartCoroutine(WriteText(guiText, content, 0.01f,exitCondition, todo));
    }
    IEnumerator WriteText(TextMeshProUGUI textHolder, string stringToWrite, float timePerChar, Func<bool> exitCondition, Action todo) 
    {
        isTyping = true;
        int charIndex = 0;
        bool canContinue = true;
        bool isAddingRichTextTag = false;
        float timer = timePerChar;
        textHolder.text = null;
        while (charIndex < stringToWrite.Length && canContinue) 
        {
            if (isAddingRichTextTag || stringToWrite[charIndex] == '<')
                isAddingRichTextTag = true;
            
            while (isAddingRichTextTag)
            {
                isAddingRichTextTag = stringToWrite[charIndex] != '>';
                textHolder.text += stringToWrite[charIndex];
                charIndex++;
            }
            if (timer > 0)
                timer -= Time.deltaTime;
            else 
            {
                timer = timePerChar;
                textHolder.text += stringToWrite[charIndex];
                charIndex++;
            }
            yield return null;
            canContinue = !exitCondition();
        }
        isTyping = false;
        textHolder.text = stringToWrite;
        todo?.Invoke();
    }
    public bool IsTyping() 
    {
        return isTyping;
    }

}
