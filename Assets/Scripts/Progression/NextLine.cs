using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class NextLine : MonoBehaviour
{
    public int dialogueIndex = 0;
    public GameObject[] Dialogues;
    public bool Ending;

    public static Action<bool> OnToggleBagua;

    public BaguaShow bagua;

    public EventObject NextState;
    public EventObject StartState;
    public void NextDialogue()
    {
        for (int i = 0; i < Dialogues.Length; i++)
        {
            if(i == dialogueIndex)
            {
                Dialogues[i].SetActive(true);
            } else
            {
                Dialogues[i].SetActive(false);
            }

            if (!bagua)
                continue;
            if (dialogueIndex == 7) 
            {
                bagua.SetActive(true);
            }
            if (dialogueIndex == 12)
                bagua.SetActive(false);
        }


        if (dialogueIndex >= Dialogues.Length)
        {
            if (Ending)
            {
                //SceneManager.LoadScene("GDC_Menu");
                StartState.Raise();
            } else 
            {
                NextState?.Raise();
                //SceneManager.LoadScene("GDC_1"); 
            }
        }


        dialogueIndex++;
    }
}
