using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem: Singleton<GridSystem>
{
    private Dictionary<(int, int), BlockBase> m_cellTop = new Dictionary<(int, int), BlockBase>();
    private Dictionary<(int, int), BlockBase> m_cellBottom = new Dictionary<(int, int), BlockBase>();


    public static (int, int) GetKey(Vector3 position)
    {
        return (Convert.ToInt32(position.x), Convert.ToInt32(position.z));
    }
    
    public static void Add(BlockBase block)
    {
        (int x, int y) key = GetKey(block.transform.position);
        var dict = block.cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;

        foreach (var s in block.cellKey)
        {
            var scopeKey = (key.x + s.x, key.y + s.y);
            if (dict.TryAdd(scopeKey, block))
            {
                Instance.Log($"{block.name} 註冊於 {block.cellType} {scopeKey}");
            }
            else
            {
                Instance.Log($"{block.name} 註冊失敗 {block.cellType} {scopeKey}");
            }
        }

        if (block.blockType == BlockType.Building)
        {
            var _dict = block.cellType == CellType.Top ? Instance.m_cellBottom : Instance.m_cellTop;
            var _type = block.cellType == CellType.Top ? CellType.Down : CellType.Top;
            foreach (var s in block.cellKey)
            {
                var scopeKey = (key.x + s.x, key.y + s.y);
                if (_dict.TryAdd(scopeKey, block))
                {
                    Instance.Log($"{block.name} 註冊於 {_type} {scopeKey}");
                }
                else
                {
                    Instance.Log($"{block.name} 註冊失敗 {_type} {scopeKey}");
                }
            }
        }
    }

    public static void Remove(Vector3 position, CellType cellType)
    {
        var key = GetKey(position);
        var dict = cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;

        if (dict.TryGetValue(key, out BlockBase blockbase))
        {
            if (dict.Remove(key))
            {
                Instance.Log($"刪除位於 {key} 的 {blockbase.name}");
                Destroy(blockbase.gameObject);
            }
            else
            {
                Instance.Log($"刪除失敗 {key} 的 {blockbase.name}");
            }
        }
    }
    public static void Remove(BlockBase block)
    {
        Remove(block.transform.position, block.cellType);
    }
    
    
    public static bool Find(Vector3 position, CellType cellType)
    {
        var key = GetKey(position);
        var dict = cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;
        //Instance.Log($"{dict.ContainsKey(key)}, {key}");
        return dict.ContainsKey(key);
    }
    
    public static bool Find(Vector3 position, CellType cellType, out BlockBase block)
    {
        var key = GetKey(position);
        var dict = cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;

        if (dict.TryGetValue(key, out BlockBase entity))
        {
            block = entity;
            return true;
        }

        block = null;
        return false;
    }
    
    
    public static void Move(BlockBase block, BlockType blockType, CellType cellType)
    {
        var cellDict = cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;
        var oldKey = cellDict.First(c => c.Value == block).Key;
        var newkey = GetKey(block.transform.position);
        
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

    
    
    public static BlockBase FindDownBlock(Vector3 position)
    {
        var key = GetKey(position);
        if (Instance.m_cellBottom.TryGetValue(key, out BlockBase entity))
        {
            return entity;
        }
        return null;
    }
    
    public static BlockBase[] FindAround(BlockBase current)
    {
        (int x, int y) key = GetKey(current.transform.position);
        var aroundKeys = new (int, int)[]
        {
            (key.x, key.y + 1),
            (key.x, key.y - 1),
            (key.x - 1, key.y),
            (key.x + 1, key.y)
        };
        var dict = current.cellType == CellType.Top ? Instance.m_cellTop : Instance.m_cellBottom;

        var block = new BlockBase[4];

        for (int i = 0; i < 4; i++)
        {
            if (dict.TryGetValue(aroundKeys[i], out BlockBase entity))
            {
                block[i] = entity;
            }
        }

        return block;
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
