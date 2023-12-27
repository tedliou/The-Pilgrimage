using UnityEngine;

[CreateAssetMenu(menuName = "Custom/GasSetting", fileName = "Gas")]
public class GasSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}