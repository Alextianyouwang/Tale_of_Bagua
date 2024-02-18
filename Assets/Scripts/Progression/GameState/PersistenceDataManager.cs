using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class PersistenceDataManager : MonoBehaviour
{
    private IDataPersistence[] _dataPersistenceObjects;
    private GameData _gameData;
    public bool EnableSaveLoad = true; 
    public static Func<SceneInfo[]> OnRequestSceneInfo;
    public static Func<AchievementObject.AchievementStates[]> OnRequestAchievementObjectStates;
    public static Dictionary<string, int> OnRequestTestObjectState;
    private FileDataHandler _fileDataHandler;
    public string FileName = "";
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneLoaded;
        SceneDataManager.OnSceneFinishedLoading += SaveDataPersistentObject;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneLoaded;
        SceneDataManager.OnSceneFinishedLoading -= SaveDataPersistentObject;

    }

    public void Start()
    {
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, FileName);
        LoadGame();
    }
    private void CreateNewGameDataIfNull() 
    {
        if (_gameData == null)
        {
            _gameData = new GameData();
            _gameData.SceneInfos = OnRequestSceneInfo.Invoke();
            _gameData.AchievementStates = OnRequestAchievementObjectStates.Invoke();
            _gameData.TestObjectStates = new Dictionary<string, int>();
        }
    }

    void OnSceneLoaded(Scene scene, Scene current)
    {
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    private IDataPersistence[] FindAllDataPersistenceObjects()
    {
        IDataPersistence[] dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(false)
            .OfType<IDataPersistence>().ToArray();
        return dataPersistenceObjects;
    }

    public void NewGame()
    {
        if (!EnableSaveLoad)
            return;
        _gameData = null;
        _fileDataHandler.Delete();
        LoadGame();
    }

    public void SaveGame()
    {
        if (!EnableSaveLoad)
            return;

        _dataPersistenceObjects.ToList().ForEach(x => x.SaveData(ref _gameData));
        GetComponent<SceneDataManager>().SaveData(ref _gameData);
        _fileDataHandler.Save(_gameData);
    }

    public void LoadGame()
    {
        if (!EnableSaveLoad)
            return;

        _gameData = _fileDataHandler.Load();
        CreateNewGameDataIfNull();
        _dataPersistenceObjects.ToList().ForEach(x => x.LoadData(_gameData));
        GetComponent<SceneDataManager>().LoadData(_gameData);

    }
    // Make sure type like dictionary are correctly initialized with values.
    public void SaveDataPersistentObject() 
    {
        _dataPersistenceObjects.ToList().ForEach(x => x.SaveData(ref _gameData));
    }
}
