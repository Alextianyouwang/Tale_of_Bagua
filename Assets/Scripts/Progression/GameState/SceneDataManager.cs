using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataManager : MonoBehaviour
{
    [SerializeField]
    private SceneDataObject[] _sceneDatas;

    private GameData _gameData;
    private int _currentScene = 0;
    private SceneDataCommunicator _currentCommunicator;

    public static Action<SceneDataCommunicator> OnUpdateSceneInfoText;


    public void Awake()
    {
        NewGame();
    }
    public void NewGame()
    {
        _gameData = new GameData();
        SceneInfo[] _sceneInfo = new SceneInfo[_sceneDatas.Length];
        _gameData.SetSceneInfos(_sceneInfo);
        LoadCurrentSceneInfo();
        OnUpdateSceneInfoText?.Invoke(_currentCommunicator);
    }

    public void GetCommunicator() 
    {
        _currentCommunicator = FindObjectOfType<SceneDataCommunicator>();
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
        OnUpdateSceneInfoText?.Invoke(_currentCommunicator);
    }
    private void SaveCurrentSceneInfo()
    {
        GetCommunicator();
        if (_currentCommunicator == null)
            return;
        if (_gameData.SceneInfos[_currentScene] == null)
            _gameData.SceneInfos[_currentScene] = new SceneInfo();
        else
        {
            _gameData.SceneInfos[_currentScene].SetPlayerPos(_currentCommunicator.GetPlayerPosition());
            _gameData.SceneInfos[_currentScene].SetMirrorPos(_currentCommunicator.GetMirrorPositions());
        }


    }
    private void LoadCurrentSceneInfo()
    {
        GetCommunicator();
        if (_currentCommunicator == null)
            return;
        if (_gameData.SceneInfos[_currentScene] == null)
            _gameData.SceneInfos[_currentScene] = new SceneInfo();
        else
        {
            _currentCommunicator.SetMirrorPositions(_gameData.SceneInfos[_currentScene].MirrorPos);
            _currentCommunicator.SetPlayerPosition(_gameData.SceneInfos[_currentScene].PlayerPos);
        }

    }

}
