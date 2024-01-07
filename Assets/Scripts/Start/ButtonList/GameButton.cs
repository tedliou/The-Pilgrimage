using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class GameButton : MonoBehaviour, IGameButton
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnButtonClick);
        m_button.image.alphaHitTestMinimumThreshold = .1f;
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
