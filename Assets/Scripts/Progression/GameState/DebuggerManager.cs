using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebuggerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sceneName;
    [SerializeField] private TextMeshProUGUI _levelCount;
    [SerializeField] private TextMeshProUGUI _currentStateName;
    [SerializeField] private TextMeshProUGUI _achievementProgress;
    [SerializeField] private GameObject _gameSessionDebugPanel;
    [SerializeField] private Image _achievementFillBar;

    private void OnEnable()
    {
        SceneDataManager.OnUpdateSceneInfoText += UpdateSceneInfoText;
        StateManager.OnSetCurrentStateName += SetCurrentStateName;
        StateManager.OnToggleGameDebugPanel += ToggleGameSessionDebugPanel;
        AchievementManager.OnUpdateAchievementStats += SetAchievementProgress;

    }
    private void OnDisable()
    {
        SceneDataManager.OnUpdateSceneInfoText -= UpdateSceneInfoText;
        StateManager.OnSetCurrentStateName -= SetCurrentStateName;
        StateManager.OnToggleGameDebugPanel -= ToggleGameSessionDebugPanel;
        AchievementManager.OnUpdateAchievementStats += SetAchievementProgress;
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

    public void SetAchievementProgress(int current, int total) 
    {
        if (_achievementFillBar == null) return;
        _achievementFillBar.fillAmount =(float)current / (float)total;
        UpdateAchievementProgressText(current,total);
    }

    public void UpdateAchievementProgressText(int current, int total) 
    {
        if (_achievementProgress == null) return;
        _achievementProgress.text = current.ToString() + "/" + total.ToString();
    }
}
