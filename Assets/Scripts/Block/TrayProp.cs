using System;
using UnityEngine;

public class TrayProp : BlockBase
{
    public int foodAmount;

    public GameObject[] foods;

    private void LateUpdate()
    {
        foreach (var f in foods)
        {
            f.SetActive(foodAmount > 0);
        }
    }
}