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
    [Header("Block")]
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
    #endregion

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        GenerateNewMap(Vector2Int.zero);
        GenerateNewMap(new Vector2Int(mapSize.x, 0));
    }

    private void GenerateNewMap(Vector2Int offset)
    {
        GenerateGrass(offset);
        GenerateTemple(offset);
        GenerateGarbage(offset);
        GenerateGas(offset);
    }
    
    private void GenerateGrass(Vector2Int offset)
    {
        for (var x = offset.x; x < offset.x + mapSize.x; x++)
        {
            for (var z = offset.y; z < offset.y + mapSize.y; z++)
            {
                SpawnBlock(x, z, grass.Id);
            }
        }
    }
    
    private void GenerateTemple(Vector2Int offset)
    {
        var x = Random.Range(templeMinX, templeMaxX);
        var y = Random.Range(templeMinY, templeMaxY);
        
        x += offset.x;
        y += offset.y;
        
        ReplaceBlock(x, y, temple.Id, templeSize, true);
        
        // Road and SedanChair
        x += sedanChairSpawnPos.x;
        y += sedanChairSpawnPos.y;
        ReplaceBlock(x, y, road.Id, new Vector2Int(prebuildRoadLength, 1));

        if (!spawnedSedanChair)
        {
            ReplaceBlock(x + 4, y, sedanChair.Id, new Vector2Int(1, 1));
            ReplaceBlock(x + 3, y, car.Id, new Vector2Int(1, 1));
            ReplaceBlock(x + 1, y, carExtend.Id, new Vector2Int(1, 1));
            
            ReplaceBlock(x, y - 2, chip.Id, new Vector2Int(1, 1));
            
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

            if (Grid2DSystem.Find(new Vector3(offset.x + randX, 0, offset.y + randY), concrete.Type, out BlockBase block))
            {
                // 只替換草地
                if (block.Setting.Id != grass.Id)
                    continue;
                
                ReplaceBlock(offset.x + randX, offset.y + randY, garbage.Id, size);
                ReplaceBlock(offset.x + randX, offset.y + randY, concrete.Id, size);
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

            if (Grid2DSystem.Find(new Vector3(offset.x + randX, 0, offset.y + randY), concrete.Type, out BlockBase block))
            {
                // 只替換草地
                if (block.Setting.Id != grass.Id)
                    continue;
                
                ReplaceBlock(offset.x + randX, offset.y + randY, gas.Id, size);
                ReplaceBlock(offset.x + randX, offset.y + randY, concrete.Id, size);
            }
        }
    }

    // private void GenerateMap()
    // {
    //     for (var x = 0; x < mapSize.x; x++)
    //     {
    //         for (var z = 0; z < mapSize.y; z++)
    //         {
    //             var buildBlock = false;
    //
    //             var cellPos = new Vector2(x, z);
    //             
    //             foreach (var e in templePos)
    //             {
    //                 var cellFix = new Vector2(e.x, e.y);
    //                 var spawnPos = new Vector3(cellPos.x + mapOffset.x, 0, cellPos.y + mapOffset.y);
    //                 if (cellPos.x >= cellFix.x && cellPos.y >= cellFix.y && cellPos.x <= cellFix.x + templeSize.x - 1 && cellPos.y <= cellFix.y + templeSize.y - 1)
    //                 {
    //                     buildBlock = true;
    //                     if (cellPos == cellFix)
    //                     {
    //                         Map_Add(cellPos, SpawnTemple(spawnPos));
    //                     }
    //                     else
    //                     {
    //                         Map_Add(cellPos, Map_Find(cellFix));
    //                     }
    //                     break;
    //                 }
    //             }
    //             
    //             if (!buildBlock)
    //                 Map_Add(cellPos, SpawnBlock(BlockType.Grass, new Vector3(cellPos.x + mapOffset.x, 0, cellPos.y + mapOffset.y)));
    //         }
    //     }
    //     
    //     // Sedan Road Prebuild
    //     var sedanRoadOriginPos = sedanChairSpawnPos;
    //     for (var i = sedanRoadOriginPos.x; i < sedanRoadOriginPos.x + prebuildRoadLength; i++)
    //     {
    //         var key = new Vector2(i, sedanRoadOriginPos.y);
    //         Map_Replace(key, SpawnBlock(BlockType.Road, new Vector3(key.x, 0, key.y)));
    //     }
    // }

    #region Map
    private BlockBase SpawnBlock(int x, int z, string blockId)
    {
        var position = new Vector3(x, 0, z);
        var prefab = GameManager.Instance.GetPrefab(blockId).gameObject;
        var blockObj = Instantiate(prefab, position, Quaternion.identity);
        var block = blockObj.GetComponent<BlockBase>();
        Grid2DSystem.Add(position, block);
        return block;
    }
    
    private void ReplaceBlock(int x, int z, string blockId, Vector2Int size, bool once = false)
    {
        var position = new Vector3Int(x, 0, z);
        var blockBase = GameManager.Instance.GetPrefab(blockId);
        
        // 建立要挖空的座標
        var queue = new List<Vector3Int>();
        for (var i = 0; i < size.x; i++)
        {
            for (var j = 0; j < size.y; j++)
            {
                queue.Add(position + new Vector3Int(i, 0, j));
            }
        }
            
        // 挖空
        for (var i = 0; i < queue.Count; i++)
        {
            Grid2DSystem.Remove(queue[i], blockBase.Setting.Type);
        }

        if (once)
        {
            // 在起點建立物件，並連結到所有被挖空的位置
            var entity = SpawnBlock(queue[0].x, queue[0].z, blockId);
            for (var i = 1; i < queue.Count; i++)
            {
                Grid2DSystem.Add(queue[i], entity);
            }
        }
        else
        {
            for (var i = 0; i < queue.Count; i++)
            {
                Grid2DSystem.Add(queue[i], SpawnBlock(queue[i].x, queue[i].z, blockId));
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
    #endregion

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
