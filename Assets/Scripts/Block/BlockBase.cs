using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockBase : CustomBehaviour<BlockBase>
{
    public List<Vector2Int> cellKey = new List<Vector2Int>();
    public CellType cellType;
    public BlockType blockType;
    public MeshRenderer[] meshRenderers;

    [SerializeField]
    private bool m_selected;
    private Color[] m_colors;
    private float[] m_scales;

    private void OnValidate()
    {
        if (cellKey.Count == 0)
        {
            cellKey.Add(Vector2Int.zero);
        }
    }

    protected virtual void Start()
    {
        //GameScene.OnShowTerrain.AddListener(Show);
        //GridSystem.Add(this);
        
        // m_colors = new Color[meshRenderers.Length];
        // m_scales = new float[meshRenderers.Length];
        // for (var i = 0; i < meshRenderers.Length; i++)
        // {
        //     if (meshRenderers[i].material.HasColor("_OutlineColor"))
        //     {
        //         m_colors[i] = meshRenderers[i].material.GetColor("_OutlineColor");
        //     }
        //
        //     if (meshRenderers[i].material.HasFloat("_OutlineScale"))
        //     {
        //         m_scales[i] = meshRenderers[i].material.GetFloat("_OutlineScale");
        //     }
        // }
        
    }

    private void Update()
    {
        // Deselect();
    }

    public void Select()
    {
        if (m_selected)
            return;
        
        m_selected = true;
        for (var i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i].material.HasColor("_OutlineColor"))
            {
                m_colors[i] = meshRenderers[i].material.GetColor("_OutlineColor");
                meshRenderers[i].material.SetColor("_OutlineColor", Color.white);
            }

            if (meshRenderers[i].material.HasFloat("_OutlineScale"))
            {
                m_scales[i] = meshRenderers[i].material.GetFloat("_OutlineScale");
                meshRenderers[i].material.SetFloat("_OutlineScale", 1.1f);
            }
        }
    }

    public void Deselect()
    {
        if (!m_selected)
            return;
        
        m_selected = false;
        for (var i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i].material.HasColor("_OutlineColor"))
                meshRenderers[i].material.SetColor("_OutlineColor", m_colors[i]);
            
            if (meshRenderers[i].material.HasFloat("_OutlineScale"))
                meshRenderers[i].material.SetFloat("_OutlineScale", m_scales[i]);
        }
    }
    
    

    public void Show()
    {
        transform.DOScale(Vector3.one, .2f);
    }

    public void Hide()
    {
        transform.DOScale(Vector3.zero, 0);
    }
}