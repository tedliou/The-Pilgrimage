using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

/// <summary>
/// 垃圾和晦氣的物件的控制腳本
/// </summary>
public class PropBlock : BlockBase
{
    #region Inspector

    /// <summary>
    /// 可堆疊
    /// </summary>
    public bool isStackable = false;

    /// <summary>
    /// 可破壞
    /// </summary>
    public bool isWreckable = false;

    /// <summary>
    /// 可拾取
    /// </summary>
    public bool isPickupable = false;

    [Header("Stacking")] [ShowIf(nameof(isStackable))]
    public Vector3 originPosition = new Vector3(.5f, 0, .5f);
    public GameObject propPrefab;

    [ShowIf(nameof(isStackable))] public float stackingHeight;
    [ShowIf(nameof(isStackable))] public int stackingLimit = 10;

    [Header("Wreck")] [ShowIf(nameof(isWreckable))]
    public GameObject dropPrefab;
    public int dropAmount = 1;
    [ShowIf(nameof(isWreckable))] public GameObject wreckEffect;

    #endregion

    #region Private
    
    private int m_amount = 0;
    private List<GameObject> m_propList = new List<GameObject>();
    
    #endregion

    protected override void Start()
    {
        base.Start();

        if (isStackable)
        {
            propPrefab.SetActive(false);
        }
    }

    [Button("破壞")]
    [ShowIf(nameof(isWreckable))]
    public void Wreck()
    {
        if (!isWreckable)
        {
            return;
        }

        var drop = Instantiate(dropPrefab);
        drop.transform.position = transform.position;
        drop.GetComponent<PropBlock>().SetAmount(dropAmount);
        GridSystem.Remove(this, blockType, cellType);

        var effect = Instantiate(wreckEffect);
        effect.transform.position = transform.position;
    }
    
    [Button("設置數量")]
    [ShowIf(nameof(isStackable))]
    public bool SetAmount(int amount)
    {
        if (!isStackable)
        {
            return false;
        }

        if (amount > stackingLimit)
        {
            return false;
        }

        m_amount = amount;
        UpdateCurrentObject();
        return true;
    }

    private void UpdateCurrentObject()
    {
        if (!isStackable)
        {
            return;
        }

        if (m_amount == m_propList.Count)
        {
            return;
        }

        if (m_amount < m_propList.Count)
        {
            var differ = m_propList.Count - m_amount;
            for (var i = 0; i < differ; i++)
            {
                m_propList.Last().DestroySelf();
                m_propList.RemoveLast();
            }
        }
        else
        {
            var differ = m_amount - m_propList.Count;
            for (var i = m_propList.Count; i < m_amount; i++)
            {
                var prop = Instantiate(propPrefab, transform);
                prop.transform.localPosition = originPosition + new Vector3(0, stackingHeight * (i + 0), 0);
                prop.SetActive(true);
                m_propList.Add(prop);
            }
        }
    }

    [Button("放置(指定數量)")]
    [ShowIf(nameof(isStackable))]
    public int Place(int amount)
    {
        var remainSpace = stackingLimit - m_amount;
        
        if (amount <= remainSpace)
        {
            Debug.Log($"放置成功 {amount}");
            SetAmount(amount + m_amount);
            return amount;
        }

        var overflowAmount = amount - remainSpace;
        if (SetAmount(overflowAmount + m_amount))
        {
            Debug.Log($"放置成功，但溢出 {overflowAmount}");
            return overflowAmount;
        }
        else
        {
            Debug.Log($"放置失敗");
            return 0;
        }
    }

    [Button("拾取(指定數量)")]
    [ShowIf(nameof(isPickupable))]
    public int Pickup(int amount)
    {
        if (!isPickupable)
        {
            return 0;
        }

        if (amount > m_amount)
        {
            return 0;
        }

        if (amount == m_amount)
        {
            GridSystem.Remove(this, blockType, cellType);
            return amount;
        }

        SetAmount(m_amount - amount);
        return amount;
    }

    [Button("拾取(全部)")]
    [ShowIf(nameof(isPickupable))]
    public int Pickup()
    {
        return Pickup(m_amount);
    }
}