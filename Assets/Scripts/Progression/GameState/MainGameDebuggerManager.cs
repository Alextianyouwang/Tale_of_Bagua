using TMPro;
using UnityEngine;

public class MainGameDebuggerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sceneName;
    [SerializeField] private TextMeshProUGUI _levelCount;
    [SerializeField] private TextMeshProUGUI _currentStateName;
    [SerializeField] private GameObject _gameSessionDebugPanel;

    private void OnEnable()
    {
        SceneDataManager.OnUpdateSceneInfoText += UpdateSceneInfoText;
        StateManager.OnSetCurrentStateName += SetCurrentStateName;
        StateManager.OnToggleGameDebugPanel += ToggleGameSessionDebugPanel;

    }
    private void OnDisable()
    {
        SceneDataManager.OnUpdateSceneInfoText -= UpdateSceneInfoText;
        StateManager.OnSetCurrentStateName -= SetCurrentStateName;
        StateManager.OnToggleGameDebugPanel -= ToggleGameSessionDebugPanel;


    }
    private void ToggleGameSessionDebugPanel(bool value) 
    {
        if (_gameSessionDebugPanel == null)
            return;
        _gameSessionDebugPanel.SetActive(value);
    }
    private void UpdateSceneInfoText(SceneDataCommunicator data)
    {
        if (!_sceneName) return;
        if (!_levelCount) return;

        _sceneName.text = data.GetSceneName();
        _levelCount.text = data.GetLayerNumber().ToString();
    }

    public void SetCurrentStateName(string currentStateName)
    {
        if (_currentStateName == null)
            return;
        _currentStateName.text = currentStateName;
    }
}
