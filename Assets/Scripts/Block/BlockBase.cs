using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockBase : CustomBehaviour<BlockBase>
{
    public CellType cellType;
    public BlockType blockType;

    protected virtual void Start()
    {
        GameScene.OnShowTerrain.AddListener(Show);
        GridSystem.Add(this);
        Hide();
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