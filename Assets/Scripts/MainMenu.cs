using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Panel1;
    public GameObject Panel2;

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadCredit()
    {
        Panel1.SetActive(false);
        Panel2.SetActive(true);
    }

    public void HideCredit()
    {
        Panel1.SetActive(true);
        Panel2.SetActive(false);
    }
}
