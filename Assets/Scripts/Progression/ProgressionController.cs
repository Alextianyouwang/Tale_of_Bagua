using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ProgressionController : MonoBehaviour
{
    public GameObject NextItem;
    public GameObject NewMirror;
    public GameObject Panel;
    public static Action<GameObject> OnBaguaCollected;
    public int CanBeEnableWhenMirroEqual;

    public void OnCollisionEnter(Collision other)
    {
        if (CanBeEnableWhenMirroEqual == LevelManager.allMirrorOnTop)
        if (other.gameObject.tag =="Player")
        {
                OnBaguaCollected?.Invoke(gameObject);
                if (LevelManager.allMirrorOnTop == 4)
                {
                    SceneManager.LoadScene("Closing Cut");
                }
                else 
                {
                    NextItem.SetActive(true);
                    NewMirror.SetActive(true);
                    Panel.SetActive(true);

                    gameObject.SetActive(false);
                }
        }
        
    }
}
