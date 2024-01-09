using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveButton : GameButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        CloseGame();
    }

    private void CloseGame()
    {
        Application.Quit();
    }
}
