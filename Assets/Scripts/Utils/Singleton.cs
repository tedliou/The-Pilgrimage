using System;
using UnityEngine;

public class Singleton<T> : CustomBehaviour<T> where T : UnityEngine.MonoBehaviour
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

    protected bool m_inited = false;

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

    protected virtual void OnAwake()
    {
        Init();
    }

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        Init();
    }

    public void Init()
    {
        if (m_inited)
            return;
        m_inited = true;
        OnInit();
    }

    protected virtual void OnInit()
    {
        
    }
}