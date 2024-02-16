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
    private FileDataHandler _fileDataHandler;
    public string FileName = "";
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneLoaded;
        SceneDataManager.OnFinishLoadingAllScenes += LoadNonSceneSensitiveData;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneLoaded;
        SceneDataManager.OnFinishLoadingAllScenes -= LoadNonSceneSensitiveData;

    }

    public void Start()
    {
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, FileName);
        CreateNewGameDataIfNull();
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
        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref _gameData);
        }
        FindObjectOfType<SceneDataManager>().SaveData(ref _gameData);
        _fileDataHandler.Save(_gameData);
    }

    public void LoadGame()
    {
        if (!EnableSaveLoad)
            return;

        _gameData = _fileDataHandler.Load();

        CreateNewGameDataIfNull();
        FindObjectOfType<SceneDataManager>().LoadData(_gameData);
        
    }

    public void LoadNonSceneSensitiveData() 
    {
        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(_gameData);
        }
    }

}
