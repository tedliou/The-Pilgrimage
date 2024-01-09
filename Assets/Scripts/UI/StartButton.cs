using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : GameButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        ShowLobby();
    }

    private void ShowLobby()
    {
        LobbyUI.Instance.Show();
        
    }
}
