using UnityEngine;

[System.Serializable]
public class Setting
{
    public Quality quality = Quality.High;
    public Vector2Int resolution = new (1920, 1080);
    public WindowMode windowMode = WindowMode.Fullscreen;
    public Volumn volumn = new Volumn(1, 1);

    public Setting Clone()
    {
        return new Setting
        {
            quality = this.quality,
            resolution = this.resolution,
            windowMode = this.windowMode,
            volumn = this.volumn.Clone()
        };
    }
    
    [System.Serializable]
    public enum Type
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