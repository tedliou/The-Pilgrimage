using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    public BlockSetting Setting
    {
        get
        {
            return settings;
        }
    }
    [SerializeField] private BlockSetting settings;
    

    private void Start()
    {
        transform.position = Grid2DSystem.WorldToCell(transform.position);
        Grid2DSystem.Add(transform.position, this);
    }
}
