using System.Collections;
using System.Collections.Generic;
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
                SceneManager.LoadScene("Main Menu");
            } else 
            { 
                SceneManager.LoadScene("GameScene"); 
            }
                
                
        }


        dialogueIndex++;
    }
}
