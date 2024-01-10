using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBGM : AudioBase<GameBGM>
{
    public AudioClip[] clips;

    private int m_index = 0;
    private Coroutine m_coroutine;

    protected override void OnStart()
    {
        base.OnStart();
    }

    public override void Play(float pitch = 1)
    {
        m_audioSource.clip = clips[m_index];
        m_index++;
        if (m_index == clips.Length)
        {
            m_index = 0;
        }
        base.Play(pitch);

        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }
        m_coroutine = StartCoroutine(EventOnStop());
    }

    protected override void OnStop()
    {
        base.OnStop();
        Play();
    }
}