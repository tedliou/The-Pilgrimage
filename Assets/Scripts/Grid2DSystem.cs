using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Grid2DSystem
{
    public static Dictionary<Vector2, GameObject> Cells = new Dictionary<Vector2, GameObject>();

    public static void Add(Vector3 position, GameObject gameObject)
    {
        var key = new Vector2(position.x, position.z);
        
        if (Cells.TryAdd(key, gameObject))
        {
            Debug.Log($"Add cell in {key}:{gameObject.name}");   
        }
        else
        {
            Debug.Log("Add failed");
        }
    }

    public static void Remove(Vector3 position)
    {
        var key = new Vector2(position.x, position.z);

        if (Cells.Remove(key))
        {
            Debug.Log("Removed");
        }
        else
        {
            Debug.Log("Remove failed");
        }
    }

    public static GameObject Find(Vector3 position)
    {
        var key = new Vector2(position.x, position.z);
        
        if (Cells.TryGetValue(key, out GameObject gameObject))
        {
            return gameObject;
        }

        return null;
    }

    public static Vector3 WorldToCell(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x);
        var y = Mathf.FloorToInt(position.y);
        var z = Mathf.FloorToInt(position.z);

        var cellCenterPos = new Vector3(x >= 0 ? x + .5f : x - .5f, y >= 0 ? y + .5f : y - .5f, z >= 0 ? z + .5f : z - .5f);
        
        return cellCenterPos;
    }
}
