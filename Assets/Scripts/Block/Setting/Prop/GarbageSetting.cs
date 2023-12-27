using UnityEngine;

[CreateAssetMenu(menuName = "Custom/GarbageSetting", fileName = "Garbage")]
public class GarbageSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}