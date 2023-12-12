using System;
using UnityEngine;

public class PanelSetting : GamePanel
{
    [SerializeField] private GameSettingOption[] gameSettingOptions;

    #region Unity Messages

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameSettingOptions.Length == 0)
            gameSettingOptions = GetComponentsInChildren<GameSettingOption>();
    }
    #endif

    #endregion
    
    protected override void OnPanelAction(PanelOption option)
    {
        base.OnPanelAction(option);
        SetDisplay(option);
    }

    private void SetDisplay(PanelOption option)
    {
        var display = option == PanelOption.Setting;
        gameObject.SetActive(display);
        if (display)
        {
            foreach (var e in gameSettingOptions)
            {
                e.Load();
            }
        }
    }
}