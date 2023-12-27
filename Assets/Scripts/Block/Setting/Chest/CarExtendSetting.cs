using UnityEngine;

[CreateAssetMenu(menuName = "Custom/CarExtendSetting", fileName = "CarExtend")]
public class CarExtendSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}