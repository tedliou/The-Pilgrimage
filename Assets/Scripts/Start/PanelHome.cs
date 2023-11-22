using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHome : GamePanel
{
    protected override void OnPanelAction(PanelOption option)
    {
        base.OnPanelAction(option);
        SetDisplay(option);
    }

    private void SetDisplay(PanelOption option)
    {
        gameObject.SetActive(option == PanelOption.Home);
    }
}
