using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;



public class SettingOption : CustomBehaviour<SettingOption>
{
    public Setting.Type type;
    
    [ShowIf(nameof(type), Setting.Type.Quality)]
    public Setting.Quality quality;
    
    [ShowIf(nameof(type), Setting.Type.WindowMode)]
    public Setting.WindowMode windowMode;

    [ShowIf(nameof(type), Setting.Type.Resolution)]
    public Vector2Int resolution;
    
    [ShowIf(nameof(type), Setting.Type.Volumn)]
    public int volumnId;

    private TabButton m_tabButton;
    private Slider m_slider;

    private bool m_inited = false;
    
    private void Awake()
    {
        if (m_inited)
        {
            return;
        }

        m_inited = true;
        
        if (type == Setting.Type.Volumn)
        {
            m_slider = GetComponent<Slider>();
            m_slider.onValueChanged.AddListener(_ => Cache());
        }
        else
        {
            m_tabButton = GetComponent<TabButton>();
            m_tabButton.onEntryClick.AddListener(Cache);
        }
    }


    public void Load()
    {
        Awake();
        switch (type)
        {
            case Setting.Type.Quality:
                if (SettingManager.Instance.GetQuality() == quality)
                {
                    m_tabButton.Click();
                }
                break;
            case Setting.Type.Resolution:
                var vector = SettingManager.Instance.GetResolution();
                if (vector.x == resolution.x && vector.y == resolution.y)
                {
                    m_tabButton.Click();
                }
                break;
            case Setting.Type.WindowMode:
                if (SettingManager.Instance.GetWindowMode() == windowMode)
                {
                    m_tabButton.Click();
                }
                break;
            case Setting.Type.Volumn:
                if (volumnId == 0)
                {
                    m_slider.value = SettingManager.Instance.GetMusicVolumn();
                }
                else
                {
                    m_slider.value = SettingManager.Instance.GetEffectVolumn();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Cache()
    {
        switch (type)
        {
            case Setting.Type.Quality:
                SettingManager.Instance.SetQuality(quality);
                break;
            case Setting.Type.Resolution:
                SettingManager.Instance.SetResolution(resolution);
                break;
            case Setting.Type.WindowMode:
                SettingManager.Instance.SetWindowMode(windowMode);
                break;
            case Setting.Type.Volumn:
                if (volumnId == 0)
                {
                    SettingManager.Instance.SetMusicVolumn(m_slider.value);
                }
                else
                {
                    SettingManager.Instance.SetEffectVolumn(m_slider.value);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
