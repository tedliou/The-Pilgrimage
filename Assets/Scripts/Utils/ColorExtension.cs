using UnityEngine;

public static class ColorExtension
{
    public static Color SetAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}