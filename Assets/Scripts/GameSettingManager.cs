using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameSettingManager : MonoBehaviour
{
    #region Singleton
    public static GameSettingManager current;
    #endregion
    
    #region Inspector
    [SerializeField] private GameSetting setting;
    [FormerlySerializedAs("temprate")] [SerializeField] private GameSetting temperate;
    #endregion
    
    #region Private
    private GameSetting _default;
    private const string SaveKey = "GameSetting";

    #endregion

    #region Unity Messages
    private void Awake()
    {
        current = this;
        LoadSave();
    }
    #endregion

    private void LoadSave()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            try
            {
                setting = Decode(PlayerPrefs.GetString(SaveKey));
                temperate = setting.Clone();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Game Setting Load Failed");
                ResetSave();
            }
        }
        else
        {
            ResetSave();
        }
    }

    public void Save()
    {
        Debug.Log("Game Setting Saved");
        PlayerPrefs.SetString(SaveKey, Encode(temperate));
        PlayerPrefs.Save();
        setting = temperate.Clone();
    }

    public void Revert()
    {
        temperate = setting.Clone();
    }

    private void ResetSave()
    {
        setting = _default;
        PlayerPrefs.SetString(SaveKey, Encode(_default));
        Debug.Log("Reset Game Setting");
    }

    private string Encode(GameSetting source)
    {
        var json = JsonUtility.ToJson(source);
        Debug.Log($"Encode JSON: {json}");
        return json;
    }
    
    private GameSetting Decode(string source)
    {
        Debug.Log($"Decode JSON: {source}");
        return JsonUtility.FromJson<GameSetting>(source);
    }

    public int GetIntSetting(GameSetting.Option option, int id = 0)
    {
        switch (option)
        {
            case GameSetting.Option.Quality:
                return (int)setting.quality;
            case GameSetting.Option.Resolution:
                Debug.LogError("This setting is not integer");
                break;
            case GameSetting.Option.WindowMode:
                return (int)setting.windowMode;
            case GameSetting.Option.Volumn:
                Debug.LogError("This setting is not integer");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }

        return -1;
    }
    
    public float GetFloatSetting(GameSetting.Option option, int id = 0)
    {
        switch (option)
        {
            case GameSetting.Option.Quality:
                Debug.LogError("This setting is not float");
                break;
            case GameSetting.Option.Resolution:
                Debug.LogError("This setting is not float");
                break;
            case GameSetting.Option.WindowMode:
                Debug.LogError("This setting is not float");
                break;
            case GameSetting.Option.Volumn:
                if (id == 0)
                {
                    return setting.volumn.music;
                }
                if (id == 1)
                {
                    return setting.volumn.effect;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }

        return -1;
    }
    
    public Vector2Int GetVector2IntSetting(GameSetting.Option option, int id = 0)
    {
        switch (option)
        {
            case GameSetting.Option.Quality:
                Debug.LogError("This setting is not Vector2");
                break;
            case GameSetting.Option.Resolution:
                return setting.resolution;
            case GameSetting.Option.WindowMode:
                Debug.LogError("This setting is not Vector2");
                break;
            case GameSetting.Option.Volumn:
                Debug.LogError("This setting is not Vector2");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }

        return new Vector2Int(1920, 1080);
    }

    public void SetIntSetting(GameSetting.Option option, int id = 0, int value = 0)
    {
        switch (option)
        {
            case GameSetting.Option.Quality:
                temperate.quality = (GameSetting.Quality)value;
                Debug.Log($"TEMPERATE: {temperate.quality}, {value}");
                break;
            case GameSetting.Option.Resolution:
                Debug.LogError("This setting is not integer");
                break;
            case GameSetting.Option.WindowMode:
                temperate.windowMode = (GameSetting.WindowMode)value;
                break;
            case GameSetting.Option.Volumn:
                Debug.LogError("This setting is not integer");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
    
    public void SetFloatSetting(GameSetting.Option option, int id = 0, float value = 0)
    {
        switch (option)
        {
            case GameSetting.Option.Quality:
                Debug.LogError("This setting is not float");
                break;
            case GameSetting.Option.Resolution:
                Debug.LogError("This setting is not float");
                break;
            case GameSetting.Option.WindowMode:
                Debug.LogError("This setting is not float");
                break;
            case GameSetting.Option.Volumn:
                if (id == 0)
                {
                    temperate.volumn.music = Mathf.Clamp(value, 0, 1);
                }
                if (id == 1)
                {
                    temperate.volumn.effect = Mathf.Clamp(value, 0, 1);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
    
    public void SetVector2IntSetting(GameSetting.Option option, int id = 0, Vector2Int value = new Vector2Int())
    {
        switch (option)
        {
            case GameSetting.Option.Quality:
                Debug.LogError("This setting is not Vector2");
                break;
            case GameSetting.Option.Resolution:
                temperate.resolution = value;
                break;
            case GameSetting.Option.WindowMode:
                Debug.LogError("This setting is not Vector2");
                break;
            case GameSetting.Option.Volumn:
                Debug.LogError("This setting is not Vector2");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
}

[System.Serializable]
public class GameSetting
{
    public Quality quality = Quality.High;
    public Vector2Int resolution = new (1920, 1080);
    public WindowMode windowMode = WindowMode.Fullscreen;
    public Volumn volumn = new Volumn(1, 1);

    public GameSetting Clone()
    {
        return new GameSetting
        {
            quality = this.quality,
            resolution = this.resolution,
            windowMode = this.windowMode,
            volumn = this.volumn.Clone()
        };
    }
    
    [System.Serializable]
    public enum Option
    {
        Quality = 0,
        Resolution = 1,
        WindowMode = 2,
        Volumn = 3
    }
    
    [System.Serializable]
    public enum Quality
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
    
    [System.Serializable]
    public enum WindowMode
    {
        Fullscreen = 0,
        Windowed = 1,
        Borderless = 2
    }
    
    [System.Serializable]
    public class Volumn
    {
        public float music;
        public float effect;

        public Volumn Clone()
        {
            return new Volumn(music, effect);
        }
        
        public Volumn(float music, float effect)
        {
            this.music = music;
            this.effect = effect;
        }
    }
}
