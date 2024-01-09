using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : CustomBehaviour<Indicator>
{
    public Transform followTransform;

    private RectTransform m_rectTransform;

    private void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (followTransform == null)
        {
            return;
        }

        var screenPos = Camera.main.WorldToScreenPoint(followTransform.position);
        m_rectTransform.position = screenPos;
    }

    public void SetFollowTransform(Transform follow = null)
    {
        followTransform = follow;
        if (follow != null)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
