using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnvSpawner : MonoBehaviour
{
    #region Singleton

    public static EnvSpawner current;

    #endregion
    
    #region Inspector
    [Header("Block Objects")]
    public GameObject grass;
    public GameObject floor;
    public GameObject temple;
    public GameObject sedanChair;
    
    [Header("Generate Parameters")]
    public Vector2 mapSize;
    public Vector2 mapOffset;
    public Vector3 mapOriginPos;
    public List<Vector2Int> templePos = new();
    public Vector2 templeSize;
    public Vector2Int templeFirstOffset;
    public Vector2Int sedanChairSpawnPos;
    public int prebuildRoadLength = 6;
    #endregion

    #region Private
    private Dictionary<Vector2, GameObject> _map = new Dictionary<Vector2, GameObject>();
    #endregion

    public enum BlockType
    {
        Grass,
        Road,
        Temple
    }

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        GenerateTemplePos(2);
        GenerateMap();
        SpawnSedanChair();
    }

    private void GenerateTemplePos(int amount)
    {
        templePos.Clear();
        
        var last = sedanChairSpawnPos + templeFirstOffset;
        templePos.Add(last);
        
        if (amount > 1)
        {
            var minDistance = 25;
            for (var i = 0; i < amount - 1; i++)
            {
                int x, y, xMin, xMax, yMin, yMax;

                xMin = last.x + minDistance;
                xMax = 40;
                x = Random.Range(xMin, xMax);

                var top = Random.Range(0, 2);
                yMin = top == 0 ? (int)mapSize.y / 2 : 2;
                yMax = top == 0 ? (int)mapSize.y - 5 : (int)mapSize.y / 2;
                y = Random.Range(yMin, yMax);

                last = new Vector2Int(x, y);
            
                templePos.Add(last);
            }
        }
        
        
    }

    private void GenerateMap()
    {
        for (var x = mapOriginPos.x; x < mapOriginPos.x + mapSize.x; x++)
        {
            for (var z = mapOriginPos.z; z < mapOriginPos.z + mapSize.y; z++)
            {
                var buildBlock = false;

                var cellPos = new Vector2(x, z);
                
                foreach (var e in templePos)
                {
                    var cellFix = new Vector2(mapOriginPos.x + e.x, mapOriginPos.z + e.y);
                    var spawnPos = new Vector3(cellPos.x + mapOffset.x, 0, cellPos.y + mapOffset.y);
                    if (cellPos.x >= cellFix.x && cellPos.y >= cellFix.y && cellPos.x <= cellFix.x + templeSize.x - 1 && cellPos.y <= cellFix.y + templeSize.y - 1)
                    {
                        buildBlock = true;
                        if (cellPos == cellFix)
                        {
                            Map_Add(cellPos, SpawnTemple(spawnPos));
                        }
                        else
                        {
                            Map_Add(cellPos, Map_Find(cellFix));
                        }
                        break;
                    }
                }
                
                if (!buildBlock)
                    Map_Add(cellPos, SpawnBlock(BlockType.Grass, new Vector3(cellPos.x + mapOffset.x, 0, cellPos.y + mapOffset.y)));
            }
        }
        
        // Sedan Road Prebuild
        var sedanRoadOriginPos = sedanChairSpawnPos;
        for (var i = sedanRoadOriginPos.x; i < sedanRoadOriginPos.x + prebuildRoadLength; i++)
        {
            var key = new Vector2(i, sedanRoadOriginPos.y);
            Map_Replace(key, SpawnBlock(BlockType.Road, new Vector3(key.x, 0, key.y)));
        }
    }

    #region Map
    private void Map_Add(Vector2 position, GameObject obj)
    {
        var key = position;
        
        if (_map.TryAdd(key, obj))
        {
            //Debug.Log($"Add cell in {key}:{obj.name}");   
        }
        else
        {
            Debug.Log($"Add failed in {key}:{obj.name}");
        }
    }
    
    
    public void Map_Replace(Vector2 position, GameObject replaceObj)
    {
        var key = position;
        
        if (_map.TryGetValue(key, out GameObject obj))
        {
            _map[key] = replaceObj;
            Destroy(obj);
            Debug.Log($"Add cell in {key}:{obj.name}");   
        }
        else
        {
            Debug.Log("Add failed");
        }
    }

    private void Map_Remove(Vector2 position)
    {
        var key = position;

        if (_map.TryGetValue(key, out GameObject obj))
        {
            _map.Remove(key);
            Destroy(obj);
            Debug.Log("Removed");
        }
        else
        {
            Debug.Log("Remove failed");
        }
    }

    public GameObject Map_Find(Vector2 position)
    {
        var key = position;
        if (_map.TryGetValue(key, out GameObject obj))
        {
            return obj;
        }

        return null;
    }
    #endregion

    private void SpawnSedanChair()
    {
        var pos = (Vector3)(Vector2)sedanChairSpawnPos;
        pos.z = sedanChairSpawnPos.y;
        pos.y = GameManager.current.propsYPos;
        var obj = Instantiate(sedanChair, pos, Quaternion.identity);
    }

    public GameObject SpawnBlock(BlockType blockType, Vector3 position)
    {
        GameObject prefab;
        
        if (blockType is BlockType.Grass or BlockType.Road or BlockType.Temple)
        {
            position.y = GameManager.current.floorYPos;
        }
        
        switch (blockType)
        {
            case BlockType.Grass:
                prefab = grass;
                break;
            case BlockType.Road:
                prefab = floor;
                break;
            case BlockType.Temple:
                prefab = temple;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null);
        }
        
        var obj = Instantiate(prefab, position, Quaternion.identity, transform);
        return obj;
    }

    private GameObject SpawnTemple(Vector3 position)
    {
        var obj = Instantiate(temple, position,
            Quaternion.identity);
        return obj;
    }

}
