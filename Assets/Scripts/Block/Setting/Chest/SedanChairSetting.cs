using UnityEngine;

[CreateAssetMenu(menuName = "Custom/SedanChairSetting", fileName = "SedanChair")]
public class SedanChairSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}