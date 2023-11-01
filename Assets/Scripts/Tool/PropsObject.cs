using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsObject : MonoBehaviour
{
    private void Start()
    {
        var current = transform.position;
        var cellPos = Grid2DSystem.WorldToCell(current);
        cellPos.y = GameManager.Instance.propsYPos;
        Grid2DSystem.Add(cellPos, gameObject);
        
        transform.position = cellPos;
    }

    private Vector3 SelfCellPos => Grid2DSystem.WorldToCell(transform.position);

    public void AddToGrid()
    {
        Grid2DSystem.Add(SelfCellPos, gameObject);
    }

    public void RemoveFromGrid()
    {
        Grid2DSystem.Remove(SelfCellPos);
    }
}
