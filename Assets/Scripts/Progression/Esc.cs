
using UnityEngine;
using UnityEngine.SceneManagement;

public class Esc : MonoBehaviour
{

    private bool hasPaused = false;
    public GameObject pauseCanvas;

    public GameObject[] panels;
    public static bool _IsPauseOn;

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
        if (SceneDataManager._IsAreYouSureActive)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && !IsDisplayingInfo()) 
        {
            if (!hasPaused)
            {
                
                hasPaused = true;
                PauseGame();
                _IsPauseOn = true;
                //isPlaying = false;
            }
            else 
            {
                hasPaused = false;
                ReturnToGame();

            }

        }


           

    }


    public void PauseGame() 
    {
        pauseCanvas.SetActive(true);
        PlayerMove.canUseWASD = false;
        MirrorManager.CanUseLeftClick = false;
        MirrorManager.CanUseRightClick = false;
    }
    public void ExitGame() 
    {
        Application.Quit();
    }
    public void LoadMainMenu() 
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ReturnToGame() 
    {
        hasPaused = false;
        pauseCanvas.SetActive(false);
        _IsPauseOn = false;
        PlayerMove.canUseWASD = true;
        MirrorManager.CanUseLeftClick = true;
        MirrorManager.CanUseRightClick = true;
    }

    
}
