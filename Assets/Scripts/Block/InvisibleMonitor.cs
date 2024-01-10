using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleMonitor : MonoBehaviour
{
    public Transform master;
    
    private void OnBecameInvisible()
    {
        var sedanChair = SedanChair.Instance.transform.position;
        var newPos = EnvSpawner.Instance.GetSpawnablePosition(new Vector3(sedanChair.x, sedanChair.y - 2));
        master.transform.position = newPos;
    }
}
