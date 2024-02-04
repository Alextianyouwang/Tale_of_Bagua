using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataManager : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private SceneDataObject[] _sceneDatas;
    private SceneInfo[] _tempSceneInfo;
    private SceneDataCommunicator _currentCommunicator;
    private int _currentScene = 0;

    public static Action<SceneDataCommunicator> OnUpdateSceneInfoText;

    public void Awake()
    {
        InitializeTempSceneInfo();
        GetCurrentSceneData();
    }
  
    public void OnEnable()
    {
        PersistenceDataManager.OnRequestSceneInfo += GetInitialSceneInfo;
    }
    public void OnDisable()
    {
        PersistenceDataManager.OnRequestSceneInfo -= GetInitialSceneInfo;
    }
    public void InitializeTempSceneInfo()
    {
        if (_tempSceneInfo == null)
            _tempSceneInfo = new SceneInfo[_sceneDatas.Length];
    }
    private SceneInfo[] GetInitialSceneInfo()
    {
        return new SceneInfo[_sceneDatas.Length];
    }
    public void SaveData(ref GameData data) 
    {
        GetCurrentSceneData();
   
        data.SceneInfos = _tempSceneInfo;
        data.CurrentScene = _currentScene;
    }

    public void LoadData(GameData data) 
    {
 
        _tempSceneInfo = data.SceneInfos;
        _currentScene = data.CurrentScene;
        if (Main.instance.LoadingGame_co == null)
            Main.instance.LoadingGame_co = Main.instance.StartCoroutine(ImplementAllScenes(data));
    }
    public void GetCommunicator() 
    {
        _currentCommunicator = FindObjectOfType<SceneDataCommunicator>();
    }
    IEnumerator ImplementAllScenes(GameData data)
    {
        int tempCurrentScene = _currentScene;
        for (int i = 0; i < _sceneDatas.Length; i++) 
        {
            _currentScene = i;
            yield return Main.instance.StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
        }
        _currentScene = tempCurrentScene;

        yield return Main.instance.StartCoroutine(LoadLevel(_sceneDatas[data.CurrentScene].Name));
        Main.instance.LoadingGame_co = null;
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
        Main.instance.StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
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
        Main.instance.StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
    }

    private IEnumerator LoadLevel(string sceneName)
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
            yield return null;
        ImplementCurrentSceneWithData();
        OnUpdateSceneInfoText?.Invoke(_currentCommunicator);
    }
    private void GetCurrentSceneData()
    {
        GetCommunicator();
        if (_currentCommunicator == null)
            return;
        if (_tempSceneInfo[_currentScene] == null) 
            _tempSceneInfo[_currentScene] = new SceneInfo();
        _tempSceneInfo[_currentScene].PlayerPos = _currentCommunicator.GetPlayerPosition();
        _tempSceneInfo[_currentScene].MirrorPos = _currentCommunicator.GetMirrorPositions();

    }
    private void ImplementCurrentSceneWithData()
    {
        GetCommunicator();
        if (_currentCommunicator == null)
            return;
        if (_tempSceneInfo[_currentScene] == null)
        {
            _tempSceneInfo[_currentScene] = new SceneInfo();
            _tempSceneInfo[_currentScene].PlayerPos = _currentCommunicator.GetPlayerPosition();
            _tempSceneInfo[_currentScene].MirrorPos = _currentCommunicator.GetMirrorPositions();
        }
        else
        {
            _currentCommunicator.SetMirrorPositions(_tempSceneInfo[_currentScene].MirrorPos);
            _currentCommunicator.SetPlayerPosition(_tempSceneInfo[_currentScene].PlayerPos);
        }
       
    }


}
