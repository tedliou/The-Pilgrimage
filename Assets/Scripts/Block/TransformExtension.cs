using System;
using UnityEngine;

public static class TransformExtension
{
    public static void SnapToGrid(this Transform transform)
    {
        transform.position = GridSystem.WorldToCell(transform.position);
    }

    public static Vector2Int GetGridKey(this Transform transform)
    {
        transform.SnapToGrid();
        return new Vector2Int(Convert.ToInt32(transform.position.x), Convert.ToInt32(transform.position.z));
    }
}