using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBack : GameButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        BackToHome();
    }

    private void BackToHome()
    {
        GamePanel.Show(GamePanel.PanelOption.Home);
    }
}
