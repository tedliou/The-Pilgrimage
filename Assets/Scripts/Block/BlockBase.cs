using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockBase : MonoBehaviour
{
    public BlockSetting Setting
    {
        get
        {
            return settings;
        }
    }
    [SerializeField] private BlockSetting settings;
    [SerializeField] private BlockSetting floatBlock;
    [SerializeField] private Transform progress;
    [SerializeField] private Transform floatObj;
    public bool isBroking = false;
    public float duration = 2;
    public float floatSpeed = 2;
    public int amount = 1;
    private float _time = 0;

    private void Start()
    {
        transform.position = Grid2DSystem.WorldToCell(transform.position);
        isBroking = false;
        if (progress)
            progress.localScale = new Vector3(1, 0, 1);
    }

    private void Update()
    {
        if (Setting.Type == BlockType.Float)
        {
            floatObj.Rotate(Vector3.up, floatSpeed);
        }
    }

    private void LateUpdate()
    {
        if (isBroking)
        {
            _time += Time.deltaTime;
            isBroking = false;
            if (_time >= duration)
            {
                _time = 0;
                // Spawn Float Obj
                var prefab = GameManager.Instance.GetPrefab(floatBlock.Id);
                var obj = Instantiate(prefab, transform.position, Quaternion.identity);
                Grid2DSystem.Remove(transform.position, Setting.Type);
                Grid2DSystem.Add(transform.position, obj.GetComponent<BlockBase>());
            }
        }
        else
        {
            _time = 0;
        }

        if (progress)
        {
            progress.localScale = new Vector3(1, Mathf.Min(_time / duration, 1), 1);
        }
    }
}
