using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PersistenceDataManager : MonoBehaviour
{
    private IDataPersistence[] _dataPersistenceObjects;
    private GameData _gameData;
    public bool EnableSaveLoad = true; 
    public static Func<SceneInfo[]> OnRequestSceneInfo;
    private FileDataHandler _fileDataHandler;
    public string FileName = "";
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneLoaded;
    }
 
    public void Start()
    {
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, FileName);
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

        _fileDataHandler.Save(_gameData);
    }

    public void LoadGame()
    {
        if (!EnableSaveLoad)
            return;

        _gameData = _fileDataHandler.Load();

        if (_gameData == null) 
        {
            _gameData = new GameData();
            _gameData.SceneInfos = OnRequestSceneInfo.Invoke();
        }
        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(_gameData);
        }
      
    }

}
