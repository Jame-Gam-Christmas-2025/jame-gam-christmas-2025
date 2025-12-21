using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public InputSystemActions _inputActions;

    private bool _isEnabled = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _inputActions = new InputSystemActions();
            Debug.Log("created");
        } else
        {
            Debug.Log("destroy");
            Destroy(gameObject);
        }
        
        if(_isEnabled)
        {
            // Enable input actions
            _inputActions.Enable();
            _inputActions.Global.TogglePause.performed += ToggleSettingsView;
        }

        Debug.Log(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSubscription(bool isSceneAllowsControls)
    {
        if(isSceneAllowsControls != _isEnabled)
        {
            _isEnabled = isSceneAllowsControls;

            if(_isEnabled)
            {
                Debug.Log("enableinput");
                // Enable input actions
                _inputActions.Enable();
                _inputActions.Global.TogglePause.performed += ToggleSettingsView;
            } else
            {
                Debug.Log("disableinput");
                _inputActions.Disable();
                _inputActions.Global.TogglePause.performed -= ToggleSettingsView;
            }
        }
    }
    public void ToggleSettingsView(InputAction.CallbackContext context)
    {
        Debug.Log(this);
        if (context.performed)
        {
            Debug.Log("Key pressed: " + context.action.bindings[0].effectivePath);
            // Handle specific key actions here
            UIManager.Instance.ToggleSettingsView();
        }
    }
}
