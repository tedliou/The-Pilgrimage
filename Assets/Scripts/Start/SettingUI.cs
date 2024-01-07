using System;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

public class SettingUI : Singleton<SettingUI>
{
    public GameObject firstSelect;
    public GameSettingOption[] options;

    private void Start()
    {
        PlayerManager.Instance.Init();
        SettingManager.Instance.Init();
        
        Hide();
    }

    private void OnEnable()
    {
        if (options.Length == 0)
        {
            options = GetComponentsInChildren<GameSettingOption>();
        }
        
        foreach (var e in options)
        {
            e.Load();
        }

        var inputEvents = FindObjectsOfType<MultiplayerEventSystem>();
        foreach (var ie in inputEvents)
        {
            ie.SetSelectedGameObject(firstSelect);
        }
        firstSelect.GetComponent<TabButton>().Click();
    }

    // private void OnDisable()
    // {
    //     
    // }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}