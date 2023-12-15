using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class InteractableObject : MonoBehaviour
{
    public int id;
    public ObjectType type = ObjectType.Props;
    public enum ObjectType
    {
        Props = 0,
        Block = 1,
        Chest = 2
    }

    public bool stack;
    public int stackAmount = 0;
    public float stackY;

    public GameObject originObj;
    public List<GameObject> stackObjs = new List<GameObject>();
    
    public void AddStack(int amount)
    {
        // Change Object State
        originObj.SetActive(false);
        if (amount >= 0)
        {
            for (var i = 0; i < amount; i++)
            {
                var obj = Instantiate(originObj, transform);
                obj.SetActive(true);
                obj.transform.localPosition = new Vector3(0, stackY * stackAmount);
                //Debug.Log(obj.transform.localPosition);
                stackObjs.Add(obj);
                stackAmount += 1;
            }
        }
        else
        {
            var abs = Mathf.Abs(amount);
            for (var i = 0; i < abs; i++)
            {
                var obj = stackObjs[i];
                Destroy(obj);
                stackObjs[i] = null;
            }

            stackObjs.RemoveAll(x => x == null);
        }
    }
    
    // Type == Block
    public int breakCount;
    public int maxBreakCount = 3;
    public float breakProgress;
    public float maxBreakProgress = .5f;
    public bool isFind;
    public bool interactable;
    public InteractableObject requireProps;
    public InteractableObject dropProps;
    public int maxStackCount = 1;
    
    // Grow
    public GameObject[] growing;

    public event UnityAction onObjectBreak;
    
    // Type == Chest
    // 容量 改事件制

    private void Awake()
    {
        breakCount = 0;
        breakProgress = 0;
    }

    private void Start()
    {
        var cellPos = SelfCellPos;
        cellPos.y = GameManager.current.propsYPos;
        transform.position = cellPos;
        AddToGrid();
        if (stack && stackObjs.Count == 0)
        {
            AddStack(1);
        }
    }

    private Vector3 SelfCellPos => Grid2DSystem.WorldToCell(transform.position);

    public void AddToGrid()
    {
        Grid2DSystem.Add(SelfCellPos, this);
    }

    public void RemoveFromGrid()
    {
        Grid2DSystem.Remove(SelfCellPos);
    }

    public void OnFindObj()
    {
        if (type == ObjectType.Block)
        {
            isFind = true;
        }
    }

    public void OnLostObj()
    {
        if (type == ObjectType.Block)
        {
            isFind = false;
        }
    }

    public void Check(InteractableObject tool)
    {
        if (tool && tool.id == id)
        {
            interactable = true;
        }
        else
        {
            interactable = false;
        }
    }

    public void DoDestroy()
    {
        if (type != ObjectType.Block)
            return;
        
        // Counting
        if (isFind && interactable)
        {
            if (breakCount < maxBreakCount)
            {
                breakProgress += Time.deltaTime;
                if (breakProgress >= maxBreakProgress)
                {
                    breakProgress = 0;
                    breakCount++;
                    if (growing.Length > 1)
                    {
                        for (int i = 0; i < growing.Length; i++)
                        {
                            growing[i].SetActive(breakCount == i);
                        }
                    }
                    
                    if (breakCount == maxBreakCount)
                    {
                        onObjectBreak?.Invoke();
                        RemoveFromGrid();
                        Destroy(gameObject);
                        if (dropProps)
                        {
                            DropProps();
                        }
                    }
                }
            }
        }
        else
        {
            breakProgress = 0;
        }
    }

    private void DropProps()
    {
        Instantiate(dropProps, transform.position, Quaternion.identity);
    }
}
