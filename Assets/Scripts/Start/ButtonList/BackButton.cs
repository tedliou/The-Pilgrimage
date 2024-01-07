using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : GameButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        BackToHome();
    }

    private void BackToHome()
    {
        SettingManager.Instance.Revert();
        SettingUI.Instance.Hide();
    }
}