using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public int garbageAmount;
    public int gasAmount;

    private CarExtendController m_carExtend;
    
    private void Start()
    {
        m_carExtend = FindObjectOfType<CarExtendController>();
        StartCoroutine(Work());
    }

    private IEnumerator Work()
    {
        while (true)
        {
            if (gasAmount > 0 && garbageAmount > 0)
            {
                gasAmount -= 1;
                garbageAmount -= 1;
                m_carExtend.Output();
            }
            yield return new WaitForSeconds(3);
        }
    }

    public void PutGarbage(int amount)
    {
        garbageAmount += amount;
        Debug.Log($"[{nameof(CarController)}] 放入 {amount} 個垃圾");
    }

    public void PutGas(int amount)
    {
        gasAmount += amount;
        Debug.Log($"[{nameof(CarController)}] 放入 {amount} 個晦氣");
    }
}
