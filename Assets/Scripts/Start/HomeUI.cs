using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class HomeUI : Singleton<HomeUI>
{
    public GameObject firstSelect;
    
    public void Show()
    {
        gameObject.SetActive(true);
        
        firstSelect.GetComponent<Button>().Select();
        var inputEvents = FindObjectsOfType<MultiplayerEventSystem>();
        foreach (var ie in inputEvents)
        {
            ie.SetSelectedGameObject(firstSelect);
        }
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
