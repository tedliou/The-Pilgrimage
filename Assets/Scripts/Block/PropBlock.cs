using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class PropBlock : BlockBase
{
    public Vector3 originPosition = new Vector3(.5f, 0, .5f);
    public float stackingHeight;
    public int stackingLimit = 10;
    
    private int m_amount = 0;
    private GameObject m_propPrefab;
    private List<GameObject> m_propList = new List<GameObject>();

    protected override void Start()
    {
        Debug.Assert(transform.childCount > 0);
        m_propPrefab = transform.GetChild(0).gameObject;
        m_propPrefab.SetActive(false);
    }

    [Button]
    public bool SetAmount(int amount)
    {
        if (amount > stackingLimit)
            return false;
        
        m_amount = amount;
        UpdateCurrentObject();
        return true;
    }

    private void UpdateCurrentObject()
    {
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
                var prop = Instantiate(m_propPrefab, transform);
                prop.transform.localPosition = originPosition + new Vector3(0, stackingHeight * (i + 0), 0);
                prop.SetActive(true);
                m_propList.Add(prop);
            }
        }
    }
}