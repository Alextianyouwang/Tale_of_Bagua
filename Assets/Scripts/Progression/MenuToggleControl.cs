using UnityEngine;

public class MenuToggleControl : MonoBehaviour
{
    public GameObject Menu;
    private void OnEnable()
    {
        StateManager.OnToggleMenu += ToggleMenu;
    }
    private void OnDisable()
    {  
        StateManager.OnToggleMenu -= ToggleMenu;
    }

    void ToggleMenu(bool value) 
    {
        Menu.SetActive(value);
        Debug.Log(value);
    }
}
