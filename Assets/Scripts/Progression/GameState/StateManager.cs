
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;
    [SerializeField]
    private SceneDataObject[] _sceneDatas;
    private int _currentScene = 0;
    [SerializeField] private TextMeshProUGUI _sceneName;
    [SerializeField] private TextMeshProUGUI _levelCount;
    private void Awake()
    {
        CreateSingleton();
        UpdateSceneInfo();
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
        StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
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
       StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name)) ;
    }

    private IEnumerator LoadLevel(string sceneName)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            Debug.Log("Loading the Scene");
            yield return null;
        }
        UpdateSceneInfo();
    }
    private void UpdateSceneInfo() 
    {
        if (!_sceneName) return;
        if (!_levelCount) return;

        LevelInfoAnnouncer announcer = FindObjectOfType<LevelInfoAnnouncer>();
        announcer?.GetLayerNumber();
        SceneDataObject data = _sceneDatas[_currentScene];
        _sceneName.text = data.Name;
        _levelCount.text = announcer?.GetLayerNumber().ToString();
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
