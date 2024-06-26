using UnityEngine;
public enum IconType { exclamation, kavaii, door}

public class WorldUIManager : MonoBehaviour
{
    private GameObject exclamationIcon_prefab;
    private GameObject exclamationIcon_instance;

    private GameObject doorIcon_prefab;
    private GameObject doorIcon_instance;

    private GameObject kavaiiIcon_prefab;
    private GameObject kavaiiIcon_instance;


    private void Awake()
    {

        doorIcon_prefab = Resources.Load<GameObject>("UI/P_Door");
        doorIcon_instance = Instantiate(doorIcon_prefab);
        doorIcon_instance.SetActive(false);

        exclamationIcon_prefab = Resources.Load<GameObject>("UI/P_ExclamationIcon");
        exclamationIcon_instance = Instantiate(exclamationIcon_prefab);
        exclamationIcon_instance.SetActive(false);

        kavaiiIcon_prefab = Resources.Load<GameObject>("UI/P_KavaiiIcon");
        kavaiiIcon_instance = Instantiate(kavaiiIcon_prefab);
        kavaiiIcon_instance.SetActive(false);
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
    void DisplayIcon(Vector3 pos, IconType type)
    {
        switch (type) 
        {
            case IconType.exclamation:
                exclamationIcon_instance.transform.position = pos;
                exclamationIcon_instance.SetActive(true);
                break;

            case IconType.kavaii:
                kavaiiIcon_instance.transform.position = pos;
                kavaiiIcon_instance.SetActive(true);
                break;

            case IconType.door:
                doorIcon_instance.transform.position = pos;
                doorIcon_instance.SetActive(true);
                break;
            default:
                exclamationIcon_instance.transform.position = pos;
                exclamationIcon_instance.SetActive(true);
                break;
        }
  
    }

    void HideIcon()
    {
        exclamationIcon_instance.SetActive(false);
        kavaiiIcon_instance.SetActive(false);
        doorIcon_instance.SetActive(false);
    }

}
