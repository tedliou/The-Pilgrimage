using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/TempleSetting", fileName = "Temple")]
public class TempleSetting : BlockSetting
{
    public override bool GetRequirement(out int id)
    {
        id = 0;
        return false;
    }
}
