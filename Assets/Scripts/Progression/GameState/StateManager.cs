using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField]private StateDataObject[] _stateData;
    [SerializeField] private int _startState;
    private int _currentState = 0;
    public static StateDataObject CurrentState;

    public static Action<string> OnSetCurrentStateName;
    public static Action<bool> OnToggleGameDebugPanel;

    private void OnEnable()
    {
        Main.instance.OnStartTicked += OnSingletonStarted;
    }
    private void OnDisable()
    {
        
    }

    private void OnSingletonStarted() 
    {
        _currentState = _startState;
        ExecuteCurrentState();
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
