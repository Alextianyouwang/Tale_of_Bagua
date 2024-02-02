using TMPro;
using UnityEngine;

public class DebuggerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sceneName;
    [SerializeField] private TextMeshProUGUI _levelCount;

    private void OnEnable()
    {
        SceneDataManager.OnUpdateSceneInfoText += UpdateSceneInfoText;
    }
    private void OnDisable()
    {
        SceneDataManager.OnUpdateSceneInfoText -= UpdateSceneInfoText;

    }
    private void UpdateSceneInfoText(SceneDataCommunicator data)
    {
        if (!_sceneName) return;
        if (!_levelCount) return;

        _sceneName.text = data.GetSceneName();
        _levelCount.text = data.GetLayerNumber().ToString();
    }
}
