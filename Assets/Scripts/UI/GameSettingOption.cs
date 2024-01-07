using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingOption : CustomBehaviour<GameSettingOption>
{
    [SerializeField] private GameSetting.Option option;
    [SerializeField] private int intValue;
    [SerializeField] private int floatValue;
    [SerializeField] private Vector2Int vectorValue;
    


    private void Awake()
    {
        if (option == GameSetting.Option.Volumn)
        {
            GetComponent<Slider>().onValueChanged.AddListener(_ => Cache());
        }
        else
        {
            GetComponent<TabButton>().onEntryClick.AddListener(Cache);
        }
    }


    public void Load()
    {
        switch (option)
        {
            case GameSetting.Option.Quality:
                if (SettingManager.Instance.GetIntSetting(GameSetting.Option.Quality) == intValue)
                {
                    GetComponent<TabButton>().Click();
                }
                break;
            case GameSetting.Option.Resolution:
                var vector = SettingManager.Instance.GetVector2IntSetting(GameSetting.Option.Resolution);
                if (vector.x == vectorValue.x && vector.y == vectorValue.y)
                {
                    GetComponent<TabButton>().Click();
                }
                break;
            case GameSetting.Option.WindowMode:
                if (SettingManager.Instance.GetIntSetting(GameSetting.Option.WindowMode) == intValue)
                {
                    GetComponent<TabButton>().Click();
                }
                break;
            case GameSetting.Option.Volumn:
                GetComponent<Slider>().value = SettingManager.Instance.GetFloatSetting(GameSetting.Option.Volumn, intValue);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Cache()
    {
        Log($"Cache: {option}, {intValue}, {floatValue}, {vectorValue}");
        switch (option)
        {
            case GameSetting.Option.Quality:
                SettingManager.Instance.SetIntSetting(GameSetting.Option.Quality, value: intValue);
                break;
            case GameSetting.Option.Resolution:
                SettingManager.Instance.SetVector2IntSetting(GameSetting.Option.Resolution, value: vectorValue);
                break;
            case GameSetting.Option.WindowMode:
                SettingManager.Instance.SetIntSetting(GameSetting.Option.WindowMode, value: intValue);
                break;
            case GameSetting.Option.Volumn:
                SettingManager.Instance.SetFloatSetting(GameSetting.Option.Volumn, intValue, GetComponent<Slider>().value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
