using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SedanChair : MonoBehaviour
{
    public Direction moveDirection = Direction.Stop;
    public float moveSpeed = .2f;

    private Rigidbody _rigidbody;
    [SerializeField]private List<RoadBlock> _roadHistory = new List<RoadBlock>();
    private Vector3Int _targetPos;

    [System.Serializable]
    public enum Direction
    {
        Top,
        Down,
        Right,
        Left,
        Stop
    }
    
    #region Unity Messages

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(FindPathLoop());
    }

    private void FixedUpdate()
    {
        var velocity = Vector3.zero;
        var dir = Vector3.zero;
        switch (moveDirection)
        {
            case Direction.Top:
                dir = new Vector3(0, 0, 1);
                break;
            case Direction.Down:
                dir = new Vector3(0, 0, -1);
                break;
            case Direction.Right:
                dir = new Vector3(1, 0, 0);
                break;
            case Direction.Left:
                dir = new Vector3(-1, 0, 0);
                break;
            case Direction.Stop:
                dir = Vector3.zero;
                break;
        }
        velocity =  dir * moveSpeed;
        

        _rigidbody.velocity = velocity;
    }

    #endregion

    private IEnumerator FindPathLoop()
    {
        while (gameObject.activeSelf)
        {
            FindPath();
            
            yield return null;
        }
    }

    private void FindPath()
    {
        var cellPos = Grid2DSystem.WorldToCell(transform.position);
        var roadBlockObj = EnvSpawner.current.Map_Find(new Vector2(cellPos.x, cellPos.z));
        if (!roadBlockObj)
            return;
        
        var roadBlockStart = roadBlockObj.GetComponent<RoadBlock>();
        if (!roadBlockStart)
            return;
        
        var roadPos = new Vector2Int((int)cellPos.x, (int)cellPos.z);
        if (FindNear(Direction.Top, roadPos))
        {
            moveDirection = Direction.Top;
        }
        else if (FindNear(Direction.Down, roadPos))
        {
            moveDirection = Direction.Down;
        }
        else if (FindNear(Direction.Right, roadPos))
        {
            moveDirection = Direction.Right;
        }
        else if (FindNear(Direction.Left, roadPos))
        {
            moveDirection = Direction.Left;
        }
    }

    private bool FindNear(Direction direction, Vector2Int roadPos)
    {
        var roadLimit = EnvSpawner.current.mapSize;
        var pos = roadPos;
        switch (direction)
        {
            case Direction.Top:
                pos.y += 1;
                if (pos.y > roadLimit.y)
                {
                    return false;
                }
                break;
            case Direction.Down:
                pos.y -= 1;
                if (pos.y < 0)
                {
                    return false;
                }
                break;
            case Direction.Right:
                pos.x += 1;
                if (pos.x > roadLimit.x)
                {
                    return false;
                }
                break;
            case Direction.Left:
                pos.x -= 1;
                if (pos.x < 0)
                {
                    return false;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        var roadBlock = GetRoadBlock(pos);
        if (!roadBlock)
            return false;

        if (_roadHistory.Contains(roadBlock))
            return false;
        
        _roadHistory.Add(roadBlock);
        _targetPos = new Vector3Int(pos.x, 0, pos.y);
        return true;
    }

    private RoadBlock GetRoadBlock(Vector2 pos)
    {
        var block = EnvSpawner.current.Map_Find(pos);
        if (block)
        {
            var roadBlock = block.GetComponent<RoadBlock>();
            return roadBlock;
        }

        return null;
    }
}
