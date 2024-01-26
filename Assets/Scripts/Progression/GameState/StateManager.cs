
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;
    [SerializeField]
    private SceneDataObject[] _sceneDatas;
    private int _currentScene = 0;
    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton() 
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject); 
    }

    public void NextScene() 
    {
        if (_sceneDatas.Length == 0) 
        {
            Debug.LogWarning("No Scene Data In Slot");
            return;
        }
        if (_currentScene >= _sceneDatas.Length - 1) 
        {
            Debug.LogWarning("All Scenes Have Been Played Through");
            return;
        }
           
        _currentScene++;
        LoadScene(_sceneDatas[_currentScene].Name);
    }
    public void PreviousScene() 
    {
        if (_sceneDatas.Length == 0)
        {
            Debug.LogWarning("No Scene Data In Slot");
            return;
        }
        if (_currentScene <= 0)
        {
            Debug.LogWarning("This Is The Very First Scene");
            return;
        }
        _currentScene--;
        LoadScene(_sceneDatas[_currentScene].Name);
    }

    private void LoadScene(string name) 
    {
        if (!Application.CanStreamedLevelBeLoaded(name) )
        {
            Debug.LogWarning($"Scene at Index {_currentScene} has invalid name");
            return;
        }
            SceneManager.LoadScene(name,LoadSceneMode.Single);
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
