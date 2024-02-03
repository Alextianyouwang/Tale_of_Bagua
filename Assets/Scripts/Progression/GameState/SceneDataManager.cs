using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataManager : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private SceneDataObject[] _sceneDatas;
    private SceneInfo[] _tempSceneInfo;
    private int _currentScene = 0;
    private SceneDataCommunicator _currentCommunicator;

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
        PersistenceDataManager.OnRequestSceneInfo -= GetInitialSceneInfo    ;
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
   
        data.SetSceneInfos(_tempSceneInfo);
        data.SetCurrentScene(_currentScene);
     
        /*  foreach (SceneInfo s in data.SceneInfos)
          {
              if (s == null)
                  continue;

              foreach (Vector3 p in s.MirrorPos)
              {
                  if (p == null)
                      continue;
                  print(p);
              }
          }*/
    }

    public void LoadData(GameData data) 
    {
 
        _tempSceneInfo = data.SceneInfos;
        _currentScene = data.CurrentScene;
        Main.instance.StartCoroutine(ImplementAllScenes(data));
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
        //GetCurrentSceneData();
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
        //GetCurrentSceneData();
        _currentScene--;
        StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
    }

    private IEnumerator LoadLevel(string sceneName)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
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
        _tempSceneInfo[_currentScene].SetPlayerPos(_currentCommunicator.GetPlayerPosition());
        _tempSceneInfo[_currentScene].SetMirrorPos(_currentCommunicator.GetMirrorPositions());

    }
    private void ImplementCurrentSceneWithData()
    {
        GetCommunicator();
        if (_currentCommunicator == null)
            return;
        if (_tempSceneInfo[_currentScene] == null)
        {
            _tempSceneInfo[_currentScene] = new SceneInfo();
            _tempSceneInfo[_currentScene].SetPlayerPos(_currentCommunicator.GetPlayerPosition());
            _tempSceneInfo[_currentScene].SetMirrorPos(_currentCommunicator.GetMirrorPositions());
        }
        else
        {
            _currentCommunicator.SetMirrorPositions(_tempSceneInfo[_currentScene].MirrorPos);
            _currentCommunicator.SetPlayerPosition(_tempSceneInfo[_currentScene].PlayerPos);
        }
       
    }


}
