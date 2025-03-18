using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WorldUIManager : MonoBehaviour
{
    private GameObject exclamationIcon_prefab;
    private GameObject exclamationIcon_instance;

    private GameObject doorIcon_prefab;
    private GameObject doorIcon_instance;

    private GameObject spaceUI_prefab;
    private GameObject spaceUI_instance;


    private void Awake()
    {

        doorIcon_prefab = Resources.Load<GameObject>("UI/P_UIDoor");
        doorIcon_instance = Instantiate(doorIcon_prefab);
        doorIcon_instance.SetActive(false);

        exclamationIcon_prefab = Resources.Load<GameObject>("UI/P_ExclamationIcon");
        exclamationIcon_instance = Instantiate(exclamationIcon_prefab);
        exclamationIcon_instance.SetActive(false);

        spaceUI_prefab = Resources.Load<GameObject>("UI/P_UISpace");
        spaceUI_instance = Instantiate(spaceUI_prefab);

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
    void DisplayIcon(Vector3 pos, IconType type, Orientation oriantation)
    {
        switch (type) 
        {
            case IconType.exclamation:
                exclamationIcon_instance.transform.position = pos;
                exclamationIcon_instance.transform.forward = DirectionHelper.GetDirection(oriantation);
                exclamationIcon_instance.SetActive(true);
                break;

            case IconType.space:
                spaceUI_instance.transform.position = pos;
                spaceUI_instance.transform.forward = DirectionHelper.GetDirection(oriantation);
                spaceUI_instance.SetActive(true);
                break;

            case IconType.door:
                doorIcon_instance.transform.position = pos;
                doorIcon_instance.transform.forward = DirectionHelper.GetDirection(oriantation);
                doorIcon_instance.SetActive(true);
                break;
            default:
                exclamationIcon_instance.transform.position = pos;
                exclamationIcon_instance.transform.forward = DirectionHelper.GetDirection(oriantation);
                exclamationIcon_instance.SetActive(true);
                break;
        }
  
    }
    void HideIcon()
    {
        exclamationIcon_instance.SetActive(false);
        spaceUI_instance.SetActive(false);
        doorIcon_instance.SetActive(false);
    }

}
