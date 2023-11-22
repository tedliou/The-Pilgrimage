using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStart : GameButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        ShowLobby();
    }

    private void ShowLobby()
    {
        GamePanel.Show(GamePanel.PanelOption.Lobby);
    }
}
