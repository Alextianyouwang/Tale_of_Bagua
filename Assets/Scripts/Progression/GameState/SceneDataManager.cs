using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataManager : MonoBehaviour
{
    [SerializeField]
    private SceneDataObject[] _sceneDatas;
    private SceneInfo[] _tempSceneInfo;
    private Dictionary<string, int> _tempTestObjectStates = new Dictionary<string, int>();

    private SceneDataCommunicator _currentCommunicator;
    private int _currentScene = 0;
    private int _currentScenePreviousDataSaver = 0;

    public static Action<SceneDataCommunicator> OnUpdateSceneInfoText;
    public static Action OnSceneFinishedLoading;

    public GameObject AreYouSurePanel;
    public TextMeshProUGUI AreYouSurePanaelText;
    public static bool _IsAreYouSureActive;
    private int _levelYouWillBeVisiting;
    public void Awake()
    {
        InitializeTempSceneInfo();
        //GetCurrentSceneData();
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
       // GetCurrentSceneData();
       //
       // data.SceneInfos = _tempSceneInfo;
       // data.CurrentScene = _currentScene;
    }

    public void LoadData(GameData data) 
    {
 
       // _tempSceneInfo = data.SceneInfos;
       // _currentScene = data.CurrentScene;
       // _tempTestObjectStates = data.TestObjectStates;
       // if (Main.Instance.LoadingGame_co == null)
       //     Main.Instance.LoadingGame_co = Main.Instance.StartCoroutine(ImplementAllScenes(data));
    }
    public void GetCommunicator() 
    {
        _currentCommunicator = FindObjectOfType<SceneDataCommunicator>();
    }
    public IEnumerator ImplementAllScenes(GameData data)
    {
        int tempCurrentScene = _currentScene;
        for (int i = 0; i < _sceneDatas.Length; i++) 
        {
            _currentScene = i;
            yield return Main.Instance.StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
        }
        _currentScene = tempCurrentScene;

        yield return Main.Instance.StartCoroutine(LoadLevel(_sceneDatas[data.CurrentScene].Name));
        Main.Instance.LoadingGame_co = null;
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
        Main.Instance.StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
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
        Main.Instance.StartCoroutine(LoadLevel(_sceneDatas[_currentScene].Name));
    }

    public void SelectSceneToGo(int value) 
    {
        _currentScene = value;
        Main.Instance.StartCoroutine(LoadLevel(_sceneDatas[value].Name));
    }
    //GDC 2025
    public void Update()
    {
        if (Esc._IsPauseOn)
            return;
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _currentScenePreviousDataSaver = _currentScene;
                AreYouSurePanel.SetActive(true);
                AreYouSurePanaelText.text = $"Are you sure about skipping to level {i + 1}? Your Progress won't be saved";
                _IsAreYouSureActive = true;
                _levelYouWillBeVisiting = i;

                PlayerMove.canUseWASD = false;
                MirrorManager.CanUseLeftClick = false;
                MirrorManager.CanUseRightClick = false;

                break; // Exit loop after finding a match
            }
        }

    }

    public void SetAreYouSureToFalse() 
    {
       _currentScene = _currentScenePreviousDataSaver;
        _IsAreYouSureActive = false;
        PlayerMove.canUseWASD = true;
        MirrorManager.CanUseLeftClick = true;
        MirrorManager.CanUseRightClick = true;
    }
    public void GoToScene() 
    {
        _currentScene = _levelYouWillBeVisiting;
         Main.Instance.StartCoroutine(LoadLevel(_sceneDatas[_levelYouWillBeVisiting].Name));
    }
    private IEnumerator LoadLevel(string sceneName)
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
            yield return null;
        OnSceneFinishedLoading?.Invoke();
     //   ImplementCurrentSceneWithData();
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
        _currentCommunicator.TrySetTestObjectState(_tempTestObjectStates);
       
    }


}
