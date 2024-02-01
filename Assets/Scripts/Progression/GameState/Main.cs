
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Main : MonoBehaviour , IDataPersistence
{
    public static Main instance;
    [SerializeField]
    private SceneDataObject[] _sceneDatas;
    private IDataPersistence[] _dataPersistenceObjects;
    private GameData _gameData;
    private int _currentScene = 0;
    private FileDataHandler _fileDataHandler;
    [SerializeField] private TextMeshProUGUI _sceneName;
    [SerializeField] private TextMeshProUGUI _levelCount;
    private void Awake()
    {
        CreateSingleton();
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, "Game.json", false);

    }
    private void Start() 
    {
        LoadGame();
        LoadCurrentSceneInfo();
        UpdateSceneInfoText();

    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
    }
    void CreateSingleton() 
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject); 
    }
    private IDataPersistence[] FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects).ToArray();
    }
    public void NewGame() 
    {
        _gameData = new GameData();

        SceneInfo[] _sceneInfo = new SceneInfo[_sceneDatas.Length];

        _gameData.SetSceneInfos(_sceneInfo);
        StartCoroutine(LoadLevel(_sceneDatas[0].Name));
    }

    public void SaveGame() 
    {
        /*foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(_gameData);
        }
        _fileDataHandler.Save(_gameData,null);*/
    }

    public void LoadGame() 
    {
        //_gameData = _fileDataHandler.Load(null,false);
        if (_gameData == null)
            NewGame();
        /*foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(_gameData);
        }*/

     
    }
    public void SaveData(GameData data)
    {
        //SaveCurrentSceneInfo();
        data.SetCurrentScene(_currentScene);
    }

    public void LoadData(GameData data) 
    {
        StartCoroutine(LoadLevel(_sceneDatas[data.CurrentScene].Name));
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
        StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
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
        if (_gameData.SceneInfos[_currentScene] == null)
            _gameData.SceneInfos[_currentScene] = new SceneInfo();
        else 
        {
            _gameData.SceneInfos[_currentScene].SetPlayerPos(announcer.GetPlayerPosition());
            _gameData.SceneInfos[_currentScene].SetMirrorPos(announcer.GetMirrorPositions());
        }
     

    }
    private void LoadCurrentSceneInfo() 
    {
        SceneDataCommunicator announcer = FindObjectOfType<SceneDataCommunicator>();
        if (announcer == null)
            return;
        if (_gameData.SceneInfos[_currentScene] == null)
            _gameData.SceneInfos[_currentScene] = new SceneInfo();
        else 
        {
            announcer.SetMirrorPositions(_gameData.SceneInfos[_currentScene].MirrorPos);
            announcer.SetPlayerPosition(_gameData.SceneInfos[_currentScene].PlayerPos);
        }
    
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
    void Update()
    {
        
    }
}
