using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    private void Start()
    {
        FadeImage.Instance.Show();
        StartBGM.Instance.Stop();
        GameBGM.Instance.Play();
    }
}
