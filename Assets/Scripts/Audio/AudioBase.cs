using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioBase<T> : Singleton<T> where T : MonoBehaviour
{
    protected AudioSource m_audioSource;

    protected Coroutine m_coroutine;

    protected override void OnInit()
    {
        base.OnInit();
        m_audioSource = GetComponent<AudioSource>();
        Debug.Assert(m_audioSource);
    }

    public virtual void Play(float pitch = 1)
    {
        Init();
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        m_audioSource.pitch = pitch;
        m_audioSource.Play();

        // if (!m_audioSource.loop)
        // {
        //     if (m_coroutine != null)
        //     {
        //         StopCoroutine(m_coroutine);
        //     }
        //     m_coroutine = StartCoroutine(DisableOnStop());
        // }
    }

    private IEnumerator DisableOnStop()
    {
        while (m_audioSource.isPlaying)
        {
            yield return null;
        }
        
        if (gameObject)
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void Stop()
    {
        Init();
        m_audioSource.Stop();
        gameObject.SetActive(false);
    }

    protected IEnumerator EventOnStop()
    {
        while (true)
        {
            while (m_audioSource.isPlaying)
            {
                yield return null;
            }
        
            OnStop();
            yield return null;
        }
    }

    protected virtual void OnStop()
    {
        
    }
}
