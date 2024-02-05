using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GeneralDebuggerManager : MonoBehaviour
{
    public TextMeshProUGUI CurrentStateName;

    private void OnEnable()
    {
        StateManager.OnSetCurrentStateName += SetCurrentStateName;
    }
    private void OnDisable()
    {
        StateManager.OnSetCurrentStateName -= SetCurrentStateName;
    }

    public void SetCurrentStateName(string currentStateName) 
    {
        if (CurrentStateName == null)
            return;
        CurrentStateName.text = currentStateName;
    }
}
