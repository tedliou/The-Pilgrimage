using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    private void Start()
    {
        var current = transform.position;
        var cellPos = Grid2DSystem.WorldToCell(current);
        cellPos.y = GameManager.Instance.propsYPos;
        Grid2DSystem.Register(cellPos, gameObject);
        
        transform.position = cellPos;
    }
}
