using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

public class SettingUI : Singleton<SettingUI>
{
    public GameObject firstSelect;
    public SettingOption[] options;

    private void Start()
    {
        PlayerManager.Instance.Init();
        SettingManager.Instance.Init();
    }

    [Button]
    private void GetAllOptions()
    {
        options = GetComponentsInChildren<SettingOption>();
    }

    private void OnEnable()
    {
        foreach (var e in options)
        {
            e.Load();
        }

        firstSelect.GetComponent<TabButton>().Click();
        firstSelect.GetComponent<TabButton>().MouseEnter();
        var inputEvents = FindObjectsOfType<MultiplayerEventSystem>();
        foreach (var ie in inputEvents)
        {
            ie.SetSelectedGameObject(firstSelect);
        }
    }

    // private void OnDisable()
    // {
    //     
    // }

    public void Show()
    {
        gameObject.SetActive(true);
        HomeUI.Instance.Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}