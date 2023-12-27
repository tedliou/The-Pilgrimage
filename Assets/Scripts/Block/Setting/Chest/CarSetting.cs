using UnityEngine;

[CreateAssetMenu(menuName = "Custom/CarSetting", fileName = "Car")]
public class CarSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}