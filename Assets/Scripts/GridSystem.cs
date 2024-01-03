using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem: Singleton<GridSystem>
{
    private Dictionary<Vector2, BlockBase> m_cellTop = new Dictionary<Vector2, BlockBase>();
    private Dictionary<Vector2, BlockBase> m_cellBottom = new Dictionary<Vector2, BlockBase>();

    public static void Add(BlockBase block, Vector3 key)
    {
        
    }
    public static void Add(BlockBase block)
    {
        var key = block.transform.GetGridKey();

        if (block.cellType == CellType.Top)
        {
            if (Instance.m_cellTop.TryAdd(key, block))
            {
                Debug.Log($"[{nameof(GridSystem)}] {block.name} 註冊於 {block.cellType} {key}");
            }
            else
            {
                Debug.Log($"[{nameof(GridSystem)}] {block.name} 註冊失敗");
            }
        }
        else
        {
            if (Instance.m_cellBottom.TryAdd(key, block))
            {
                Debug.Log($"[{nameof(GridSystem)}] {block.name} 註冊於 {block.cellType} {key}");
            }
            else
            {
                Debug.Log($"[{nameof(GridSystem)}] {block.name} 註冊失敗 {key}");
            }
        }
    }

    public static void Remove(Vector2 key, BlockType blockType, CellType cellType)
    {
        if (cellType == CellType.Top)
        {
            if (Instance.m_cellTop.TryGetValue(key, out BlockBase blockbase))
            {
                if (Instance.m_cellTop.Remove(key))
                {
                    Debug.Log($"[{nameof(GridSystem)}] 刪除位於 {key} 的 {blockbase.name}");
                    Destroy(blockbase.gameObject);
                }
                else
                {
                    Debug.Log($"[{nameof(GridSystem)}] 刪除失敗");
                }
            }
        }
        else
        {
            if (Instance.m_cellBottom.TryGetValue(key, out BlockBase blockbase))
            {
                if (Instance.m_cellBottom.Remove(key))
                {
                    Debug.Log($"[{nameof(GridSystem)}] 刪除位於 {key} 的 {blockbase.name}");
                    Destroy(blockbase.gameObject);
                }
                else
                {
                    Debug.Log($"[{nameof(GridSystem)}] {blockbase.name} 刪除失敗");
                }
            }
        }
    }
    public static void Remove(BlockBase block, BlockType blockType, CellType cellType)
    {
        var key = block.transform.GetGridKey();
        Remove(key, blockType, cellType);
    }
    
    public static void Move(BlockBase block, BlockType blockType, CellType cellType)
    {
        var cellDict = cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;
        var oldKey = cellDict.First(c => c.Value == block).Key;
        var newkey = block.transform.GetGridKey();
        
        if (cellDict.TryGetValue(oldKey, out BlockBase blockbase))
        {
            if (cellDict.TryAdd(newkey, blockbase))
            {
                Debug.Log($"[{nameof(GridSystem)}] {blockbase.name} 註冊於 {blockbase.cellType} {newkey}");
            }
            else
            {
                Debug.Log($"[{nameof(GridSystem)}] {blockbase.name} 註冊失敗");
            }
            
        }
        
        if (cellDict.Remove(oldKey))
        {
            Debug.Log($"[{nameof(GridSystem)}] 刪除位於 {oldKey} 的 {block.name}");
        }
        else
        {
            Debug.Log($"[{nameof(GridSystem)}] {block.name} 刪除失敗");
        }
    }

    public static bool Find(Vector3 position, CellType cellType)
    {
        var key = new Vector2(position.x, position.z);
        var dict = cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;
        return dict.ContainsKey(key);

    }
    
    public static bool Find(Vector3 position, BlockType blockType, out BlockBase block)
    {
        var key = new Vector2(position.x, position.z);

        if (IsTopGrid(blockType))
        {
            if (Instance.m_cellTop.TryGetValue(key, out BlockBase entity))
            {
                block = entity;
                return true;
            }
        }
        else
        {
            if (Instance.m_cellBottom.TryGetValue(key, out BlockBase entity))
            {
                block = entity;
                return true;
            }
        }

        block = null;
        return false;
    }
    
    public static BlockBase FindDownBlock(BlockBase current)
    {
        var key = current.transform.GetGridKey();
        if (Instance.m_cellBottom.TryGetValue(key, out BlockBase entity))
        {
            return entity;
        }

        return null;
    }
    
    public static List<BlockBase> FindAround(BlockBase current)
    {
        var key = current.transform.GetGridKey();
        var aroundKeys = new Vector2Int[]
        {
            key + new Vector2Int(0, 1),
            key + new Vector2Int(0, -1),
            key + new Vector2Int(-1, 0),
            key + new Vector2Int(1, 0)
        };
        var cellDict = current.cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;

        var block = new BlockBase[aroundKeys.Length];

        for (int i = 0; i < aroundKeys.Length; i++)
        {
            if (cellDict.TryGetValue(aroundKeys[i], out BlockBase entity))
            {
                block[i] = entity;
            }
        }

        return block.ToList();
    }

    public static bool Find(string blockId, out BlockBase block)
    {
        var blockPrefab = GameManager.Instance.GetPrefab(blockId);
        if (blockPrefab.cellType == CellType.Top)
        {
            var query = Instance.m_cellTop.Values.FirstOrDefault(x => x.name == blockId);
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
            var query = Instance.m_cellBottom.Values.FirstOrDefault(x => x.name == blockId);
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
               input == BlockType.Chest || input == BlockType.Drop;
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
