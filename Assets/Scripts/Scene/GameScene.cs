using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GameScene : CustomBehaviour<GameScene>
{
    public TMP_Text overlayText;

    public static UnityEvent OnShowTerrain = new();
    
    private void Start()
    {
        FadeImage.Instance.Show();
        StartBGM.Instance.Stop();
        //GameBGM.Instance.Play();

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        overlayText.text = string.Empty;
        
        while (FadeImage.Instance.IsActive)
        {
            yield return null;
        }

        PlayerManager.Instance.MInputManager.EnableJoining();
        //OnShowTerrain.Invoke();
        
        //yield return new WaitForSeconds(1);
        
        var countdown = 3;
        while (countdown > 0)
        {
            Log(countdown);
            overlayText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }
        
        PlayerManager.Instance.MInputManager.DisableJoining();
        
        Log("Spawn Players");
        GameManager.Instance.SpawnAllPlayers();
        
        
        Log("Start");
        overlayText.text = "START";
        yield return new WaitForSeconds(1);
        overlayText.text = string.Empty;
    }
}
