using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingOpenButton : GameButton
{
    private void Start()
    {
        SettingUI.Instance.Init();
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        SettingUI.Instance.Show();
    }
}
