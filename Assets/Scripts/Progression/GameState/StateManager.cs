using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField]private StateDataObject[] _stateData;
    [SerializeField] private int _startState;
   public static int _currentState = 0;
    public static StateDataObject CurrentState;

    public static Action<string> OnSetCurrentStateName;
    public static Action<bool> OnToggleGameDebugPanel;
    public static Action<bool> OnToggleMenu;

    public GameObject Menu;


    private void OnEnable()
    {
        Main.Instance.OnStartTicked += OnSingletonStarted;
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

        if (_currentState == 0)
            Menu.SetActive(true);
        else
            Menu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) 
        {
            SelectState(3);
        }
    }

    public void SelectState(int value) 
    {
        _currentState = value;
        ExecuteCurrentState();
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
