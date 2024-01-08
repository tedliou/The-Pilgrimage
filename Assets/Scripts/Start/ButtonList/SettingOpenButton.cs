using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingOpenButton : GameButton
{
    private void Start()
    {
        
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        SettingUI.Instance.Show();
    }
}
