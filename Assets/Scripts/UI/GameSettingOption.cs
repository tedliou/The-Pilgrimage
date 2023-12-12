using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingOption : MonoBehaviour
{
    #region Inspector
    [SerializeField] private GameSetting.Option option;
    [SerializeField] private int intValue;
    [SerializeField] private int floatValue;
    [SerializeField] private Vector2Int vectorValue;
    #endregion
    

    #region Unity Messages

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

    #endregion

    public void Load()
    {
        switch (option)
        {
            case GameSetting.Option.Quality:
                if (GameSettingManager.current.GetIntSetting(GameSetting.Option.Quality) == intValue)
                {
                    GetComponent<TabButton>().Click();
                }
                break;
            case GameSetting.Option.Resolution:
                var vector = GameSettingManager.current.GetVector2IntSetting(GameSetting.Option.Resolution);
                if (vector.x == vectorValue.x && vector.y == vectorValue.y)
                {
                    GetComponent<TabButton>().Click();
                }
                break;
            case GameSetting.Option.WindowMode:
                if (GameSettingManager.current.GetIntSetting(GameSetting.Option.WindowMode) == intValue)
                {
                    GetComponent<TabButton>().Click();
                }
                break;
            case GameSetting.Option.Volumn:
                GetComponent<Slider>().value = GameSettingManager.current.GetFloatSetting(GameSetting.Option.Volumn, intValue);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Cache()
    {
        Debug.Log($"Cache: {option}, {intValue}, {floatValue}, {vectorValue}");
        switch (option)
        {
            case GameSetting.Option.Quality:
                GameSettingManager.current.SetIntSetting(GameSetting.Option.Quality, value: intValue);
                break;
            case GameSetting.Option.Resolution:
                GameSettingManager.current.SetVector2IntSetting(GameSetting.Option.Resolution, value: vectorValue);
                break;
            case GameSetting.Option.WindowMode:
                GameSettingManager.current.SetIntSetting(GameSetting.Option.WindowMode, value: intValue);
                break;
            case GameSetting.Option.Volumn:
                GameSettingManager.current.SetFloatSetting(GameSetting.Option.Volumn, intValue, GetComponent<Slider>().value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
