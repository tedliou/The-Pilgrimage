using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

/// <summary>
/// https://forum.gamer.com.tw/Co.php?bsn=60602&amp;sn=8006&amp;subbsn=1&amp;bPage=0
/// </summary>
public class DOTweenKiller : Singleton<DOTweenKiller>
{
    private void OnEnable() => SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
    private void SceneManagerOnSceneUnloaded(Scene scene) => DOTween.KillAll();
    private void OnApplicationQuit() => DOTween.KillAll();
    private void OnDisable() => SceneManager.sceneUnloaded -= SceneManagerOnSceneUnloaded;
}
