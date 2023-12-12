using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class GameButton : MonoBehaviour, IGameButton
{
    [HideInInspector]
    public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    protected virtual void OnButtonClick()
    {
        Debug.Log($"Button Click: {gameObject.name}");
    }

    public void Click()
    {
        // Nothing
    }
}
