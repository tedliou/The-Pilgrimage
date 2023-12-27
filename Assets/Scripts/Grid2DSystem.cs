using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Grid2DSystem: MonoBehaviour
{
    private static Dictionary<Vector2, BlockBase> CellTop = new Dictionary<Vector2, BlockBase>();
    private static Dictionary<Vector2, BlockBase> CellBottom = new Dictionary<Vector2, BlockBase>();

    public static void Add(Vector3 position, BlockBase block)
    {
        var key = new Vector2(position.x, position.z);

        if (IsTopGrid(block.Setting.Type))
        {
            if (CellTop.TryAdd(key, block))
            {
                Debug.Log($"[{nameof(Grid2DSystem)}] {block.name} 註冊於 {key}");
            }
            else
            {
                Debug.Log($"[{nameof(Grid2DSystem)}] {block.name} 註冊失敗");
            }
        }
        else
        {
            if (CellBottom.TryAdd(key, block))
            {
                Debug.Log($"[{nameof(Grid2DSystem)}] {block.name} 註冊於 {key}");
            }
            else
            {
                Debug.Log($"[{nameof(Grid2DSystem)}] {block.name} 註冊失敗 {key}");
            }
        }
    }

    public static void Remove(Vector3 position, BlockType blockType)
    {
        var key = new Vector2(position.x, position.z);
        
        if (IsTopGrid(blockType))
        {
            if (CellTop.TryGetValue(key, out BlockBase block))
            {
                if (CellTop.Remove(key))
                {
                    Debug.Log($"[{nameof(Grid2DSystem)}] 刪除位於 {key} 的 {block.name}");
                    Destroy(block.gameObject);
                    Debug.Log($"[{nameof(Grid2DSystem)}] 刪除狀態 {CellTop.ContainsKey(key)}");
                }
                else
                {
                    Debug.Log($"[{nameof(Grid2DSystem)}] {block.name} 刪除失敗");
                }
            }
        }
        else
        {
            if (CellBottom.TryGetValue(key, out BlockBase block))
            {
                if (CellBottom.Remove(key))
                {
                    Debug.Log($"[{nameof(Grid2DSystem)}] 刪除位於 {key} 的 {block.name}");
                    Destroy(block.gameObject);
                }
                else
                {
                    Debug.Log($"[{nameof(Grid2DSystem)}] {block.name} 刪除失敗");
                }
            }
        }
    }

    public static bool Find(Vector3 position, BlockType blockType, out BlockBase block)
    {
        var key = new Vector2(position.x, position.z);

        if (IsTopGrid(blockType))
        {
            if (CellTop.TryGetValue(key, out BlockBase entity))
            {
                block = entity;
                return true;
            }
        }
        else
        {
            if (CellBottom.TryGetValue(key, out BlockBase entity))
            {
                block = entity;
                return true;
            }
        }

        block = null;
        return false;
    }

    public static bool Find(string blockId, out BlockBase block)
    {
        var blockPrefab = GameManager.Instance.GetPrefab(blockId);
        if (IsTopGrid(blockPrefab.Setting.Type))
        {
            var query = CellTop.Values.FirstOrDefault(x => x.Setting.Id == blockId);
            if (query is not null)
            {
                block = query;
                return true;
            }

            block = null;
            return false;
        }
        else
        {
            var query = CellBottom.Values.FirstOrDefault(x => x.Setting.Id == blockId);
            if (query is not null)
            {
                block = query;
                return true;
            }

            block = null;
            return false;
        }
    }

    private static bool IsTopGrid(BlockType input)
    {
        return input == BlockType.Prop || input == BlockType.Tool ||
               input == BlockType.Chest || input == BlockType.Float;
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
