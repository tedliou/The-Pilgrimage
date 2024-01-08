using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : Singleton<FadeImage>
{
    private Image m_image;
    private Coroutine m_coroutine;

    public void Show()
    {
        m_image = GetComponentInChildren<Image>();
        
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }

        m_coroutine = StartCoroutine(RunFadeIn());
    }

    public void Hide()
    {
        m_image = GetComponentInChildren<Image>();
        
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }

        m_coroutine = StartCoroutine(RunFadeOut());
    }

    private IEnumerator RunFadeIn()
    {
        m_image.color = m_image.color.SetAlpha(1);
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return m_image.DOFade(0, 2).WaitForCompletion();
        m_image.DOKill();
        m_image.enabled = false;
    }
    
    private IEnumerator RunFadeOut()
    {
        m_image.enabled = true;
        m_image.color = m_image.color.SetAlpha(0);
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return m_image.DOFade(1, 2).WaitForCompletion();
        m_image.DOKill();
    }
}
