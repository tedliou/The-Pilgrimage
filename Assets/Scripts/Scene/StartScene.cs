using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    private void Start()
    {
        FrameRateSetter.Instance.Init();
        PlayerManager.Instance.Init();
        SettingManager.Instance.Init();
    }
}
