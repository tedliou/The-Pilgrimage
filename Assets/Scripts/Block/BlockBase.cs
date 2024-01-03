using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockBase : MonoBehaviour
{
    public CellType cellType;
    public BlockType blockType;

    protected virtual void Start()
    {
        GridSystem.Add(this);
    }
}