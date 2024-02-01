
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Main : MonoBehaviour
{
    public static Main instance;
    [SerializeField]
    private SceneDataObject[] _sceneDatas;
    private SceneInfo[] _sceneInfo;
    private int _currentScene = 0;
    [SerializeField] private TextMeshProUGUI _sceneName;
    [SerializeField] private TextMeshProUGUI _levelCount;
    private void Awake()
    {
        Initialize();
        CreateSingleton();
        UpdateSceneInfoText();
    }
    private void Initialize() 
    {
        _sceneInfo = new SceneInfo[_sceneDatas.Length];
        for (int i = 0; i< _sceneInfo.Length; i++)
            _sceneInfo[i] = new SceneInfo();
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
        SaveCurrentSceneInfo();
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
        SaveCurrentSceneInfo();
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
        LoadCurrentSceneInfo();
        UpdateSceneInfoText();
    }
    private void SaveCurrentSceneInfo() 
    {
        SceneDataCommunicator announcer = FindObjectOfType<SceneDataCommunicator>();
        if (announcer == null)
            return;
        _sceneInfo[_currentScene].SetPlayerPos(announcer.GetPlayerPosition());
        _sceneInfo[_currentScene].SetMirrorPos(announcer.GetMirrorPositions());
    }
    private void LoadCurrentSceneInfo() 
    {
        SceneDataCommunicator announcer = FindObjectOfType<SceneDataCommunicator>();
        if (announcer == null)
            return;
        announcer.SetMirrorPositions(_sceneInfo[_currentScene].MirrorPos);
        announcer.SetPlayerPosition(_sceneInfo[_currentScene].PlayerPos);
    }
    private void UpdateSceneInfoText() 
    {
        if (!_sceneName) return;
        if (!_levelCount) return;

        SceneDataCommunicator announcer = FindObjectOfType<SceneDataCommunicator>();
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
