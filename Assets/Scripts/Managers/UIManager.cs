using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("UIManager");
                _instance = gameObject.AddComponent<UIManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

    public GameObject settingsView;

    private bool _isMenuOpen = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSettingsView()
    {
        _isMenuOpen = !_isMenuOpen;
        Debug.Log(_isMenuOpen);

        if(_isMenuOpen)
        {
            GameObject gameObject = Instantiate(settingsView);
            gameObject.name = "SettingsView";
        } else
        {
            GameObject viewToDestroy = GameObject.Find("SettingsView");
            Destroy(viewToDestroy);
        }
    }

    public void CloseAllViews()
    {
        _isMenuOpen = false;
    }
}
