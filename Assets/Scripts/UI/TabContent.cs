using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabContent : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private string group;
    [SerializeField] private TabButton tabButton;

    [Header("Cache")] [SerializeField] private Selectable[] selectables;

    #region Private

    private RectTransform _rectTransform;

    #endregion
    
    #region Unity Messages
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (selectables.Length == 0)
            selectables = GetComponentsInChildren<Selectable>();
    }
#endif
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        TabButton.onClick.AddListener(OnTabButtonClick);
        _rectTransform.anchoredPosition = Vector2.zero;
        Hide();
    }
    #endregion

    #region Events
    private void OnTabButtonClick(string groupID, TabButton entry)
    {
        if (groupID == group && entry == tabButton)
        {
            Show();
        }
        else
        {
            if (groupID == group)
                Hide();
        }
    }
    #endregion

    private void Show()
    {
        _rectTransform.anchoredPosition = Vector2.zero;
        foreach (var e in selectables)
        {
            var navigation = e.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            e.navigation = navigation;
        }
    }

    private void Hide()
    {
        _rectTransform.anchoredPosition = new Vector2(6000, 4000);
        foreach (var e in selectables)
        {
            var navigation = e.navigation;
            navigation.mode = Navigation.Mode.None;
            e.navigation = navigation;
        }
    }
}
