using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    private void Start()
    {
        FadeImage.Instance.Show();
        
        FrameRateSetter.Instance.Init();
        PlayerManager.Instance.Init();
        SettingManager.Instance.Init();
        
        StartBGM.Instance.Play();
        
        HomeUI.Instance.Show();
        SettingUI.Instance.Hide();
        LobbyUI.Instance.Hide();
        
    }
}
