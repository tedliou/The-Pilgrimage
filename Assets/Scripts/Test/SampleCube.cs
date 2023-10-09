using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCube : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    
    private void Start()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material.color = new Color(Random.Range(.8f, 1f), Random.Range(.9f, 1f), Random.Range(.9f, 1f));
    }
}
