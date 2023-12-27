using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : UnityEngine.Object
{
    public static T Instance
    {
        get
        {
            _instance ??= FindObjectOfType<T>();
            return _instance;
        }
    }
    private static T _instance;

    private void Awake()
    {
        if (Instance is not null && Instance != this)
        {
            gameObject.SetActive(false);
        }
    }
}