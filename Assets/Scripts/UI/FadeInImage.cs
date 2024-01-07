using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImage : MonoBehaviour
{
    private Image m_image;

    private void Start()
    {
        m_image = GetComponent<Image>();
        StartCoroutine(RunFadeIn());
    }

    private IEnumerator RunFadeIn()
    {
        m_image.color = m_image.color.SetAlpha(1);
        yield return new WaitForEndOfFrame();
        yield return m_image.DOFade(0, 2).WaitForCompletion();
        m_image.DOKill();
        m_image.gameObject.SetActive(false);
    }
}
