using System;
using UnityEngine;

public class FoodBlock : BlockBase
{
    public bool hasFood = true;
    public GameObject[] foods;


    public void Take()
    {
        if (hasFood)
        {
            hasFood = false;
            foreach (var f in foods)
            {
                f.SetActive(false);
            }
        }
    }
}