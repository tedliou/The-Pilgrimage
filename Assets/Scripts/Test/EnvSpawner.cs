using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnvSpawner : MonoBehaviour
{
    #region Singleton

    public static EnvSpawner current;

    #endregion
    
    #region Inspector

    [Header("Block")]
    public GameObject grassBlock;
    public GameObject concreteBlock;
    public GameObject roadBlock;
    
    public GameObject templeBlock;
    
    public GameObject garbageProp;
    public GameObject gasProp;

    public GameObject sedanChairPrefab;

    public GameObject chipProp;
    public Indicator clipIndicator;
    
    
    
    
    public BlockSetting grass;
    public BlockSetting temple;
    public BlockSetting garbage;
    public BlockSetting gas;
    public BlockSetting concrete;
    public BlockSetting road;
    public BlockSetting sedanChair;
    public BlockSetting chip;
    
    public BlockSetting car;
    public BlockSetting carExtend;
    
    
    [Header("Generate Parameters")]
    public Vector2Int mapSize;

    public int templeMinX = 2;
    public int templeMaxX = 6;
    public int templeMinY = 2;
    public int templeMaxY = 14;
    
    public Vector2Int templeSize;


    public float garbageScale = .2f;
    
    public Vector2Int sedanChairSpawnPos;
    public int prebuildRoadLength = 6;

    public bool spawnedSedanChair = false;
    
    
    public Vector2Int templeFirstOffset;

    private Vector2Int m_lastMapPos;
    private int m_lastMapIndex;
    #endregion

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        m_lastMapPos = Vector2Int.zero;
        m_lastMapIndex = 0;
        GenerateNewMap();
        GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        m_lastMapPos = new Vector2Int(m_lastMapIndex * mapSize.x, 0);
        
        GenerateTemple(m_lastMapPos);
        GenerateGarbage(m_lastMapPos);
        GenerateGas(m_lastMapPos);
        GenerateGrass(m_lastMapPos);

        m_lastMapIndex++;
    }
    
    private void GenerateGrass(Vector2Int offset)
    {
        for (var x = offset.x; x < offset.x + mapSize.x; x++)
        {
            for (var z = offset.y; z < offset.y + mapSize.y; z++)
            {
                if (!GridSystem.Find(new Vector3(x, 0, z), CellType.Down))
                {
                    SpawnBlock(x, z, grassBlock);
                }
            }
        }
    }
    
    private void GenerateTemple(Vector2Int offset)
    {
        var x = Random.Range(templeMinX, templeMaxX);
        var y = Random.Range(templeMinY, templeMaxY);
        
        x += offset.x;
        y += offset.y;

        SpawnBlock(x, y, templeBlock, false);
        //ReplaceBlock(x, y, templeBlock, templeSize, true);
        
        // Road and SedanChair
        x += sedanChairSpawnPos.x;
        y += sedanChairSpawnPos.y;
        var endPoint = SpawnBlock(x, y, roadBlock).GetComponent<RoadBlock>();
        endPoint.isPassed = m_lastMapIndex == 0;
        endPoint.isEndPoint = m_lastMapIndex != 0;
        //ReplaceBlock(x, y, roadBlock, new Vector2Int(prebuildRoadLength, 1));

        if (!spawnedSedanChair)
        {
            SpawnBlock(x, y, sedanChairPrefab, false);
            var obj = SpawnBlock(x, y - 2, chipProp, false);
            clipIndicator.SetFollowTransform(obj.transform);
            
            spawnedSedanChair = true;
        }
    }

    private void GenerateGarbage(Vector2Int offset)
    {
        var mapDimision = mapSize.x * mapSize.y;
        var scale = Convert.ToInt32(mapDimision * garbageScale);

        for (var i = 0; i < scale; i++)
        {
            var randX = Random.Range(0, mapSize.x);
            var randY = Random.Range(0, mapSize.y);
            //var size = new Vector2Int(Random.Range(1, 4), Random.Range(1, 4));
            var size = new Vector2Int(1, 1);
            var dimision = size.x * size.y;
            i += dimision;

            var pos = new Vector3(offset.x + randX, 0, offset.y + randY);
            if (!GridSystem.Find(pos, CellType.Top) && !GridSystem.Find(pos, CellType.Down))
            {
                SpawnBlock(offset.x + randX, offset.y + randY, garbageProp);
                SpawnBlock(offset.x + randX, offset.y + randY, concreteBlock);
            }
        }
    }
    
    private void GenerateGas(Vector2Int offset)
    {
        var mapDimision = mapSize.x * mapSize.y;
        var scale = Convert.ToInt32(mapDimision * garbageScale);

        for (var i = 0; i < scale; i++)
        {
            var randX = Random.Range(0, mapSize.x);
            var randY = Random.Range(0, mapSize.y);
            // var size = new Vector2Int(Random.Range(1, 4), Random.Range(1, 4));
            var size = new Vector2Int(1, 1);
            var dimision = size.x * size.y;
            i += dimision;

            SpawnBlock(offset.x + randX, offset.y + randY, gasProp);
            SpawnBlock(offset.x + randX, offset.y + randY, concreteBlock);
        }
    }

    private BlockBase SpawnBlock(float x, float z, GameObject prefab, bool isTerrain = true)
    {
        var position = new Vector3(x, 0, z);
        var blockObj = Instantiate(prefab, position, Quaternion.identity, isTerrain ? transform: null);
        var block = blockObj.GetComponent<BlockBase>();
        return block;
    }
    
    private void ReplaceBlock(int x, int z, GameObject prefab, Vector2Int size, bool once = false)
    {
        var position = new Vector3Int(x, 0, z);
        var blockBase = prefab;
        
        // 建立要挖空的座標
        var queue = new List<Vector3>();
        for (var i = 0; i < size.x; i++)
        {
            for (var j = 0; j < size.y; j++)
            {
                queue.Add(position + new Vector3(i, 0, j));
            }
        }
            
        // 挖空
        for (var i = 0; i < queue.Count; i++)
        {
            GridSystem.Remove(queue[i], BlockType.Floor, CellType.Down);
        }

        if (once)
        {
            // 在起點建立物件，並連結到所有被挖空的位置
            var entity = SpawnBlock(queue[0].x, queue[0].z, prefab);
            for (var i = 1; i < queue.Count; i++)
            {
                GridSystem.Add(entity, queue[i]);
            }
        }
        else
        {
            for (var i = 0; i < queue.Count; i++)
            {
                //GridSystem.Add(SpawnBlock(queue[i].x, queue[i].z, blockId), queue[i]);
            }
        }
    }
    
    

    // private void Map_Remove(Vector2 position)
    // {
    //     var key = position;
    //
    //     if (_map.TryGetValue(key, out GameObject obj))
    //     {
    //         _map.Remove(key);
    //         Destroy(obj);
    //         Debug.Log("Removed");
    //     }
    //     else
    //     {
    //         Debug.Log("Remove failed");
    //     }
    // }
    //
    // public GameObject Map_Find(Vector2 position)
    // {
    //     var key = position;
    //     if (_map.TryGetValue(key, out GameObject obj))
    //     {
    //         return obj;
    //     }
    //
    //     return null;
    // }

    // private void SpawnSedanChair()
    // {
    //     var pos = (Vector3)(Vector2)sedanChairSpawnPos;
    //     pos.z = sedanChairSpawnPos.y;
    //     pos.y = GameManager.Instance.propsYPos;
    //     var obj = Instantiate(sedanChair, pos, Quaternion.identity);
    // }

    // public GameObject SpawnBlock(BlockType blockType, Vector3 position)
    // {
    //     GameObject prefab;
    //     
    //     if (blockType is BlockType.Grass or BlockType.Road or BlockType.Temple)
    //     {
    //         position.y = GameManager.Instance.floorYPos;
    //     }
    //     
    //     switch (blockType)
    //     {
    //         case BlockType.Grass:
    //             prefab = grass;
    //             break;
    //         case BlockType.Road:
    //             prefab = floor;
    //             break;
    //         case BlockType.Temple:
    //             prefab = temple;
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null);
    //     }
    //     
    //     var obj = Instantiate(prefab, position, Quaternion.identity, transform);
    //     return obj;
    // }
    //
    // private GameObject SpawnTemple(Vector3 position)
    // {
    //     var obj = Instantiate(temple, position,
    //         Quaternion.identity);
    //     return obj;
    // }

}
