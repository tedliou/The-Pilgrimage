using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    public ParticleSystem[] particleSystems;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Start()
    {
        if (particleSystems.Length > 0)
        {
            StartCoroutine(ObserveParticleSystemState());
        }
    }

    private IEnumerator ObserveParticleSystemState()
    {
        var isAlive = true;
        while (isAlive)
        {
            isAlive = false;
            foreach (var ps in particleSystems)
            {
                if (ps.isPlaying)
                {
                    isAlive = true;
                    break;
                }
            }
            
            if (isAlive)
            {
                yield return null;
                continue;
            }
        }
        
        Destroy(gameObject);
    }
}
