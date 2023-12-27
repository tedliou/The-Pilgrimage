using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ConcreteSetting", fileName = "Concrete")]
public class ConcreteSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}