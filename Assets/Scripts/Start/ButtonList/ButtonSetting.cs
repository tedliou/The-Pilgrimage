using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSetting : GameButton
{
    public GameObject setting;

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        ShowSetting();
    }

    private void ShowSetting()
    {
        GamePanel.Show(GamePanel.PanelOption.Setting);
    }
}
