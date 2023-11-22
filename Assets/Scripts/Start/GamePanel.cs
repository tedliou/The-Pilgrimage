using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class GamePanel : MonoBehaviour
{
    private static readonly UnityEvent<PanelOption> panelEvent = new UnityEvent<PanelOption>();

    public bool showOnAwake;
    
    private void Awake()
    {
        panelEvent.AddListener(OnPanelAction);
        transform.position = Vector3.zero;
        
        gameObject.SetActive(showOnAwake);
    }

    protected virtual void OnPanelAction(PanelOption option)
    {
    }

    public enum PanelOption
    {
        Home = 0,
        Setting = 1,
        Lobby = 2,
        Leave = -1
    }
    
    public static void Show(PanelOption option)
    {
        panelEvent.Invoke(option);
    }
}
