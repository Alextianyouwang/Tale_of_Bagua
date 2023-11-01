
using UnityEngine;
using UnityEngine.SceneManagement;

public class Esc : MonoBehaviour
{

    private bool hasPaused = false;
    private bool canUseEsc = true;
    public GameObject pauseCanvas;

    public GameObject[] panels;

    bool IsDisplayingInfo() 
    {
        foreach (GameObject g in panels) 
        {
            if (g.activeInHierarchy) 
            {
                return true;
            }
        }
        return false;
    }
   
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !IsDisplayingInfo()) 
        {
            if (!hasPaused)
            {
                hasPaused = true;
                pauseCanvas.SetActive(true);
                PlayerMove.canUseWASD = false;
                MirrorManager.canUseLeftClick = false;
                MirrorManager.canUseRightClick = false;
                //isPlaying = false;
            }
            else 
            {
                hasPaused = false;
                ReturnToGame();

            }

        }


           

    }

    public void LoadMainMenu() 
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ReturnToGame() 
    {
        hasPaused = false;
        pauseCanvas.SetActive(false);
        PlayerMove.canUseWASD = true;
        MirrorManager.canUseLeftClick = true;
        MirrorManager.canUseRightClick = true;
    }

    
}
