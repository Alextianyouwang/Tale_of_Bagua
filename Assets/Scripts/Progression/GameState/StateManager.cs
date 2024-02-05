using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField]private StateDataObject[] _stateData;
    private int _currentState = 2;
    public static StateDataObject CurrentState;

    public static Action<string> OnSetCurrentStateName;
    public static Action<bool> OnToggleGameDebugPanel;

    private void ExecuteSelectedState(string name) 
    {
        StateDataObject state = _stateData.ToList().Find(x => x.Name == name);
        CurrentState = state;
        state.LoadState();
        OnSetCurrentStateName?.Invoke(CurrentState.Name);
    }

    private void ExecuteCurrentState() 
    {
        _stateData[_currentState].LoadState();
        CurrentState = _stateData[_currentState];
        OnSetCurrentStateName?.Invoke(CurrentState.Name);
        OnToggleGameDebugPanel?.Invoke(_stateData[_currentState].IsGameSession);
    }

    public void NextState()
    {
        if (_stateData.Length == 0)
        {
            Debug.LogWarning("No State Data In Slot");
            return;
        }
        if (_currentState >= _stateData.Length - 1)
        {
            Debug.LogWarning("All State Have Been Played Through");
            return;
        }
        _currentState++;
        ExecuteCurrentState();
    }
    public void PreviousState()
    {
        if (_stateData.Length == 0)
        {
            Debug.LogWarning("No State Data In Slot");
            return;
        }
        if (_currentState <= 0)
        {
            Debug.LogWarning("This Is The Very First State");
            return;
        }
        _currentState--;
        ExecuteCurrentState();
    }

}
