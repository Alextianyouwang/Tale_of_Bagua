using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebuggerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sceneName;
    [SerializeField] private TextMeshProUGUI _levelCount;
    [SerializeField] private TextMeshProUGUI _currentStateName;
    [SerializeField] private TextMeshProUGUI _achievementProgress;
    [SerializeField] private GameObject _gameSessionDebugPanel;
    [SerializeField] private Image _achievementFillBar;
    [SerializeField] private Button[] _slotButtons;


    private void OnEnable()
    {
        SceneDataManager.OnUpdateSceneInfoText += UpdateSceneInfoText;
        StateManager.OnSetCurrentStateName += SetCurrentStateName;
        StateManager.OnToggleGameDebugPanel += ToggleGameSessionDebugPanel;
        AchievementManager.OnUpdateAchievementStats += SetAchievementProgress;
        PersistenceDataManager.OnChangeSaveSlot += ChangeButtonColor;
        PersistenceDataManager.OnShareAchievementsProgresses += SetSlotButtonProgression;

        
    }
    private void OnDisable()
    {
        SceneDataManager.OnUpdateSceneInfoText -= UpdateSceneInfoText;
        StateManager.OnSetCurrentStateName -= SetCurrentStateName;
        StateManager.OnToggleGameDebugPanel -= ToggleGameSessionDebugPanel;
        AchievementManager.OnUpdateAchievementStats -= SetAchievementProgress;
        PersistenceDataManager.OnChangeSaveSlot -= ChangeButtonColor;
        PersistenceDataManager.OnShareAchievementsProgresses -= SetSlotButtonProgression;


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

       // _sceneName.text = data.GetSceneName();
       // _levelCount.text = data.GetLayerNumber().ToString();
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

    public void ChangeButtonColor(int index) 
    {
        if (_slotButtons == null)
            return;
        for (int i = 0; i < _slotButtons.Length; i++)
        {
            if (_slotButtons[index] == null || _slotButtons[i] == null)
                continue;

            ColorBlock c = _slotButtons[i].colors;
            c.normalColor = index == i ? Color.green : Color.white;
            _slotButtons[i].colors = c;
        }
    }

    public void SetSlotButtonProgression(AchievementObject.AchievementStates[][] states) 
    {
        if (_slotButtons == null)
            return;
        for (int i = 0; i < _slotButtons.Length; i++)
        {
            if ( _slotButtons[i] == null)
                continue;

            _slotButtons[i].transform.GetChild(0).GetComponent<Image>().fillAmount =
                 (float)states[i].Where(x => x == AchievementObject.AchievementStates.Accomplished).Count() / (float)states[i].Length;
        }
    }

    

}
