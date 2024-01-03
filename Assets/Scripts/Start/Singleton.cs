using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : UnityEngine.MonoBehaviour
{
    public static T Instance
    {
        get
        {
            _instance ??= FindObjectOfType<T>();
            if (_instance is null)
            {
                var prefab = Resources.Load<T>(typeof(T).Name).gameObject;
                var instanceObj = Instantiate(prefab);
                _instance = instanceObj.GetComponent<T>();
            }
            return _instance;
        }
    }
    private static T _instance;

    private void Awake()
    {
        if (Instance is not null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }

        name = $"[{typeof(T).Name}]";
        DontDestroyOnLoad(gameObject);
    }
}