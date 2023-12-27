using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSelectObject : MonoBehaviour
{
    public static GameObject Find()
    {
        return FindObjectOfType<FirstSelectObject>()?.gameObject;
    }
}
