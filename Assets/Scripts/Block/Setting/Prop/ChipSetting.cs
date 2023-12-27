using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ChipSetting", fileName = "Chip")]
public class ChipSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}