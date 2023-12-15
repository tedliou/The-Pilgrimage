using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid2DSystem: MonoBehaviour
{
    public static Dictionary<Vector2, InteractableObject> Cells;

    private void Awake()
    {
        Cells = new Dictionary<Vector2, InteractableObject>();
    }

    public static void Add(Vector3 position, InteractableObject gameObject)
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

    public static InteractableObject Find(Vector3 position)
    {
        var key = new Vector2(position.x, position.z);
        
        if (Cells.TryGetValue(key, out InteractableObject gameObject))
        {
            return gameObject;
        }

        return null;
    }

    public static Vector3 WorldToCell(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x);
        var z = Mathf.FloorToInt(position.z);

        //var cellCenterPos = new Vector3(x >= 0 ? x + .5f : x - .5f, 0, z >= 0 ? z + .5f : z - .5f);
        var cellCenterPos = new Vector3(x, 0, z);
        
        return cellCenterPos;
    }

    public static Vector2Int WorldTo2DCell(Vector3 position)
    {
        return new Vector2Int((int)position.x, (int)position.z);
    }
}
