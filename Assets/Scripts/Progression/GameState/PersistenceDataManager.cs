using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class PersistenceDataManager : MonoBehaviour
{
    private IDataPersistence[] _dataPersistenceObjects;
    private GameData[] _gameData;
    private int _gameDataIndex = 0;
    public bool EnableSaveLoad = true; 
    public static Func<SceneInfo[]> OnRequestSceneInfo;
    public static Func<AchievementObject.AchievementStates[]> OnRequestAchievementObjectStates;
    public static Action<int> OnChangeSaveSlot;
    public static Action<AchievementObject.AchievementStates[][]> OnShareAchievementsProgresses;
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
        StartGame();
        SetSlotIndex(0);
    }
    private void CreateNewGameDataIfNull() 
    {
        if (_gameData != null) return;
        _gameData = new GameData[5];
        for (int i = 0; i < _gameData.Length; i++)
            NewGameData(i);
    }

    private void NewGameData(int index) 
    {
        _gameData[index] = new GameData();
        _gameData[index].SceneInfos = OnRequestSceneInfo.Invoke();
        _gameData[index].AchievementStates = OnRequestAchievementObjectStates.Invoke();
        _gameData[index].TestObjectStates = new Dictionary<string, int>();
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
    public void StartGame() 
    {
        CreateNewGameDataIfNull();
        _dataPersistenceObjects.ToList().ForEach(x => x.LoadData(_gameData[_gameDataIndex]));
        GetComponent<SceneDataManager>().LoadData(_gameData[_gameDataIndex]);
        LogManager.Log("Game Started");
        LogManager.Log($"Set to default, please load");
    }
    public void NewGame()
    {
        if (!EnableSaveLoad)
            return;
        _gameData[_gameDataIndex] = null;
        _fileDataHandler.Save(_gameData);
        LogManager.Log($"Slot {_gameDataIndex} has been cleared");
        LoadGame();
    }

    public void SaveGame()
    {
        if (!EnableSaveLoad)
            return;

        _dataPersistenceObjects.ToList().ForEach(x => x.SaveData(ref _gameData[_gameDataIndex]));
        GetComponent<SceneDataManager>().SaveData(ref _gameData[_gameDataIndex]);
        _fileDataHandler.Save(_gameData);
        LogManager.Log($"Game Saved to slot: {_gameDataIndex}");

        OnShareAchievementsProgresses?.Invoke(_gameData.Select(x => x.AchievementStates).ToArray());
    }

    public void LoadGame()
    {
        if (!EnableSaveLoad)
            return;

        _gameData = _fileDataHandler.Load();
        if (_gameData[_gameDataIndex] == null)
            NewGameData(_gameDataIndex);
        _dataPersistenceObjects.ToList().ForEach(x => x.LoadData(_gameData[_gameDataIndex]));
        GetComponent<SceneDataManager>().LoadData(_gameData[_gameDataIndex]);
        _fileDataHandler.Save(_gameData);
        LogManager.Log($"Game Loaded from slot: {_gameDataIndex}");

        OnShareAchievementsProgresses?.Invoke(_gameData.Select(x => x.AchievementStates).ToArray());

    }
    // Make sure type like dictionary are correctly initialized with values.
    public void SaveDataPersistentObject() 
    {
        _dataPersistenceObjects.ToList().ForEach(x => x.SaveData(ref _gameData[_gameDataIndex]));
    }

    public void SetSlotIndex(int value)
    {
        _gameDataIndex = value;
        OnChangeSaveSlot?.Invoke(_gameDataIndex);
        LogManager.Log($"Active save slot: {_gameDataIndex}");
    }
}
