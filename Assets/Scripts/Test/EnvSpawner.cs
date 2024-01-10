using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnvSpawner : Singleton<EnvSpawner>
{
    [Header("Map")]
    public Vector2Int mapSize;
    public Vector2Int templeSize;
    public int templeMinX = 2;
    public int templeMaxX = 6;
    public int templeMinY = 2;
    public int templeMaxY = 14;


    public float garbageScale = .2f;
    
    public Vector2Int sedanChairSpawnPos;
    
    public int prebuildRoadLength = 6;

    public bool spawnedSedanChair = false;
    
    
    public Vector2Int templeFirstOffset;

    private Vector2Int m_lastMapPos;
    private int m_lastMapIndex;
    private bool m_isMailliTemple = false;
    
    [Header("Floor")]
    public GameObject grassPrefab;
    public GameObject concretePrefab;
    public GameObject roadPrefab;
    
    [Header("Temple")]
    public GameObject templeMailliPrefab;
    public GameObject templeNorthPrefab;
    
    [Header("Resource")]
    public GameObject gasPrefab;
    public GameObject garbagePrefab;

    [Header("Tool")]
    public GameObject clipPrefab;
    public GameObject bombPrefab;
    
    [Header("Tool Indicator")]
    public Indicator clipIndicator;
    public Indicator bombIndicator;
    
    [Header("Sedan Chair")]
    public GameObject sedanChairPrefab;
    
    


    void Start()
    {
        m_lastMapPos = Vector2Int.zero;
        m_lastMapIndex = 0;
        StartCoroutine(GenerateNewMap());
        StartCoroutine(GenerateNewMap());
    }

    public IEnumerator GenerateNewMap()
    {
        m_lastMapPos = new Vector2Int(m_lastMapIndex * mapSize.x, 0);

        var startPos = m_lastMapPos;
        
        GenerateTemple(startPos);

        m_lastMapIndex++;
        
        yield return null;
        
        //GenerateGarbage(startPos);
        
        yield return null;
        
        //GenerateGas(startPos);
        
        yield return null;
        
        GenerateGrass(startPos);
    }
    
    private void GenerateGrass(Vector2Int offset)
    {
        for (var x = offset.x; x < offset.x + mapSize.x; x++)
        {
            for (var z = offset.y; z < offset.y + mapSize.y; z++)
            {
                if (!GridSystem.Find(new Vector3(x, 0, z), CellType.Down))
                {
                    GridSystem.Add(SpawnBlock(x, z, grassPrefab));
                }
            }
        }
    }

    private Vector3 GetSpawnablePosition(Vector3 startPos)
    {
        var spawnPos = startPos;
        while (GridSystem.Find(spawnPos, CellType.Top))
        {
            spawnPos += new Vector3(1, 0, 0);
        }
        return spawnPos;
    }
    
    private void GenerateTemple(Vector2Int offset)
    {
        var prefab = m_isMailliTemple ? templeMailliPrefab : templeNorthPrefab;
        m_isMailliTemple = !m_isMailliTemple;
        
        var x = Random.Range(templeMinX, templeMaxX) + offset.x;
        var y = Random.Range(templeMinY, templeMaxY) + offset.y;

        GridSystem.Add(SpawnBlock(x, y, prefab, false));
        
        // Road and SedanChair
        x += sedanChairSpawnPos.x;
        y += sedanChairSpawnPos.y;
        var endPoint = SpawnBlock(x, y, roadPrefab).GetComponent<RoadBlock>();
        endPoint.isEndPoint = m_lastMapIndex != 0;
        GridSystem.Add(endPoint);
        
        if (!spawnedSedanChair)
        {
            endPoint.Enable();
            SpawnBlock(x, y, sedanChairPrefab, false);
            
            // 生成夾子
            var clipPos = GetSpawnablePosition(new Vector3(x, y - 2));
            var clipObj = SpawnBlock(clipPos.x, clipPos.y, clipPrefab, false);
            clipIndicator.SetFollowTransform(clipObj.transform);
            
            // 生成炸彈
            var bombPos = GetSpawnablePosition(new Vector3(x + 2, y - 2));
            var bombObj = SpawnBlock(bombPos.x, bombPos.y, bombPrefab, false);
            bombIndicator.SetFollowTransform(bombObj.transform);
            
            spawnedSedanChair = true;
        }

        if (m_lastMapIndex == 0)
        {
            for (var i = 0; i < 3; i++)
            {
                x += 1;
                var road = SpawnBlock(x, y, roadPrefab).GetComponent<RoadBlock>();
                road.Enable();
                GridSystem.Add(road);
            }
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
            var size = new Vector2Int(1, 1);
            var dimision = size.x * size.y;
            i += dimision;

            
            var pos = new Vector3(offset.x + randX, 0, offset.y + randY);
            if (!GridSystem.Find(pos, CellType.Top))
            {
                SpawnBlock(pos.x, pos.z, garbagePrefab);
                SpawnBlock(pos.x, pos.z, concretePrefab);
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

            var pos = new Vector3(offset.x + randX, 0, offset.y + randY);
            if (!GridSystem.Find(pos, CellType.Top) && !GridSystem.Find(pos, CellType.Down))
            {
                SpawnBlock(pos.x, pos.z, gasPrefab);
                SpawnBlock(pos.x, pos.z, concretePrefab);
            }
        }
    }

    private BlockBase SpawnBlock(float x, float z, GameObject prefab, bool isTerrain = true)
    {
        var position = new Vector3(x, 0, z);
        var blockObj = Instantiate(prefab, position, Quaternion.identity, isTerrain ? transform: null);
        var block = blockObj.GetComponent<BlockBase>();
        return block;
    }
}
