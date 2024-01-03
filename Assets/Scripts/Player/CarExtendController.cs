using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarExtendController : MonoBehaviour
{
    public int amount = 0;
    
    public void Output()
    {
        Debug.Log($"[{nameof(CarExtendController)}] 生成 1 個祈路");
        amount++;
    }

    public int Take()
    {
        if (amount > 0)
        {
            Debug.Log($"[{nameof(CarExtendController)}] 拿取 1 個祈路");
            amount -= 1;
            return 1;
        }

        return 0;
    }
}
