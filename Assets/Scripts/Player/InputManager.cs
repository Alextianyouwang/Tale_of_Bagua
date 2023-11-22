using System;
using UnityEngine;
using UnityEngine.InputSystem;

// This script acts as a single point for all other scripts to get
// the current input from. It uses Unity's new Input System and
// functions should be mapped to their corresponding controls
// using a PlayerInput component with Unity Events.

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public TheTaleofBagua inputActions;
    private InputAction rightClick;
    private InputAction leftClick;
    private InputAction interact;

    public Action<InputAction.CallbackContext> OnInteractionPressed;
    public Action<InputAction.CallbackContext> On_LMB_Down;
    public Action<InputAction.CallbackContext> On_LMB;
    public Action<InputAction.CallbackContext> On_LMB_Up;
    public Action<InputAction.CallbackContext> On_RMB_Down;
    public Action<InputAction.CallbackContext> On_RMB;
    public Action<InputAction.CallbackContext> On_RMB_Up;
    private void Awake()
    {
        inputActions = new TheTaleofBagua();
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    private void OnEnable()
    {


    }
    private void OnDisable()
    {

    }
    public static InputManager GetInstance()
    {
        return instance;
    }


}