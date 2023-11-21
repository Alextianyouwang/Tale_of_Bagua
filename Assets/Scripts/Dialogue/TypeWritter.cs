using System.Collections;
using UnityEngine;
using TMPro;

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

    void StartTyping(TextMeshProUGUI guiText, string content) 
    {
        StartCoroutine(WriteText(guiText, content, 0.01f));
    }
    IEnumerator WriteText(TextMeshProUGUI textHolder, string stringToWrite, float timePerChar) 
    {
        int charIndex = 0;
        isTyping = true;
        while (charIndex < stringToWrite.Length) 
        {
            textHolder.text = stringToWrite.Substring(0, charIndex);
            charIndex ++;
            yield return new WaitForSeconds (timePerChar);
        }
        isTyping = false;
    }
    public bool IsTyping() 
    {
        return isTyping;
    }

}
