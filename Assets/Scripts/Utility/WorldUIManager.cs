using UnityEngine;

public class WorldUIManager : MonoBehaviour
{
    private GameObject exclamationIcon_prefab;
    private GameObject exclamationIcon_instance;
    private void Awake()
    {
        exclamationIcon_prefab = Resources.Load<GameObject>("UI/P_ExclamationIcon");
        exclamationIcon_instance = Instantiate(exclamationIcon_prefab);
        exclamationIcon_instance.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerInteract.OnDetactPlayer += DisplayIcon;
        PlayerInteract.OnLostPlayer += HideIcon;
        LevelManager.OnPlayerSwitchLevel += HideIcon;
    }

    private void OnDisable()
    {
        PlayerInteract.OnDetactPlayer -= DisplayIcon;
        PlayerInteract.OnLostPlayer -= HideIcon;
        LevelManager.OnPlayerSwitchLevel -= HideIcon;

    }
    void DisplayIcon(Vector3 pos)
    {
        exclamationIcon_instance.transform.position = pos;
        exclamationIcon_instance.SetActive(true);
    }

    void HideIcon()
    {
        exclamationIcon_instance.SetActive(false);
    }

}
