using System;
using System.Collections;
using UnityEngine;

public class BombTinyProp : BlockBase
{
    public ParticleSystem particle;
    public PropBlock propBlock;
    public GameObject model;
    public AudioSource sfx;
    public AudioSource placeSFX;

    private void OnEnable()
    {
        placeSFX.Play();
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1);
        }
        
        particle.Play();
        yield return null;
        propBlock.Wreck();
        Destroy(model);
        sfx.Play();
        yield return new WaitUntil(() => !particle.IsAlive());
        sfx.Stop();
        Destroy(gameObject);
        
    }
}