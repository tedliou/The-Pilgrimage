using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ChipToolSetting", fileName = "ChipTool")]
public class ChipToolSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}