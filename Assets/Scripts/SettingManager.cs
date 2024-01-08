using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class SettingManager : Singleton<SettingManager>
{
    public AudioMixer audioMixer;
    
    [SerializeField] private Setting setting;
    [FormerlySerializedAs("temprate")] [SerializeField] private Setting temperate;
    
    private Setting _default;
    private const string SaveKey = "GameSetting";

    protected override void OnInit()
    {
        Load();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            try
            {
                setting = Decode(PlayerPrefs.GetString(SaveKey));
                temperate = setting.Clone();
                GetQuality();
                GetResolution();
                GetWindowMode();
                GetMusicVolumn();
                GetEffectVolumn();
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

    private string Encode(Setting source)
    {
        var json = JsonUtility.ToJson(source);
        Debug.Log($"Encode JSON: {json}");
        return json;
    }
    
    private Setting Decode(string source)
    {
        Debug.Log($"Decode JSON: {source}");
        return JsonUtility.FromJson<Setting>(source);
    }

    public Setting.Quality GetQuality()
    {
        var quality = temperate.quality;
        SetQuality(quality);
        return quality;
    }

    public void SetQuality(Setting.Quality quality)
    {
        temperate.quality = quality;
        QualitySettings.SetQualityLevel((int)quality);
        
        Log($"{nameof(SetQuality)}: {quality}");
    }

    public Vector2Int GetResolution()
    {
        var res = temperate.resolution;
        SetResolution(res);
        return res;
    }
    
    public void SetResolution(Vector2Int resolution)
    {
        temperate.resolution = resolution;
        var mode = FullScreenMode.FullScreenWindow;
        if (temperate.windowMode == Setting.WindowMode.Fullscreen)
        {
            mode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (temperate.windowMode == Setting.WindowMode.Windowed)
        {
            mode = FullScreenMode.Windowed;
        }
        Screen.SetResolution(temperate.resolution.x, temperate.resolution.y, mode);
        
        Log($"{nameof(SetResolution)}: {resolution}");
    }

    public Setting.WindowMode GetWindowMode()
    {
        var mode = temperate.windowMode;
        SetWindowMode(mode);
        return mode;
    }
    
    public void SetWindowMode(Setting.WindowMode windowMode)
    {
        temperate.windowMode = windowMode;
        if (temperate.windowMode == Setting.WindowMode.Borderless)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (temperate.windowMode == Setting.WindowMode.Fullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (temperate.windowMode == Setting.WindowMode.Windowed)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        
        Log($"{nameof(SetWindowMode)}: {windowMode}");
    }

    public float GetMusicVolumn()
    {
        var volumn = temperate.volumn.music;
        SetMusicVolumn(volumn);
        return volumn;
    }
    
    public void SetMusicVolumn(float volumn)
    {
        temperate.volumn.music = volumn;
        audioMixer.SetFloat("Music", volumn);
        
        Log($"{nameof(SetMusicVolumn)}: {volumn}");
    }

    public float GetEffectVolumn()
    {
        var volumn = temperate.volumn.effect;
        SetEffectVolumn(volumn);
        return volumn;
    }

    public void SetEffectVolumn(float volumn)
    {
        temperate.volumn.effect = volumn;
        audioMixer.SetFloat("Effect", volumn);
        
        Log($"{nameof(SetEffectVolumn)}: {volumn}");
    }
}