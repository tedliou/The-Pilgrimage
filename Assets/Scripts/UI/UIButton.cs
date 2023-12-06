using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour
{
    public GameObject highlight;

    private void Update()
    {
        highlight.SetActive(EventSystem.current.currentSelectedGameObject == gameObject);
    }
}
