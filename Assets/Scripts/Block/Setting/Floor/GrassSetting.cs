using UnityEngine;

[CreateAssetMenu(menuName = "Custom/GrassSetting", fileName = "Grass")]
public class GrassSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}