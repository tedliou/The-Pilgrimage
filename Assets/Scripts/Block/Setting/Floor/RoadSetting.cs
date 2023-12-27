using UnityEngine;

[CreateAssetMenu(menuName = "Custom/RoadSetting", fileName = "Road")]
public class RoadSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}