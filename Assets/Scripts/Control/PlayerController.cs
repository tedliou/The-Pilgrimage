using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class Tool
{
    protected bool hasTool;
    protected string id;

    public Tool()
    {
        Reset();
    }
    
    public bool TryGet(out string id)
    {
        if (!hasTool)
        {
            id = string.Empty;
            return false;
        }

        id = this.id;
        return true;
    }

    public bool TrySet(string id)
    {
        if (hasTool)
        {
            return false;
        }

        if (id == string.Empty)
            return false;

        hasTool = true;
        this.id = id;
        return true;
    }

    public void Reset()
    {
        hasTool = false;
        this.id = string.Empty;
    }
}

public class Inventory
{
    protected bool hasItem;
    protected string id;
    protected int amount;

    public Inventory()
    {
        Reset();
    }

    public bool TryGet(out string id, out int amount)
    {
        if (!hasItem)
        {
            id = string.Empty;
            amount = 0;
            return false;
        }

        id = this.id;
        amount = this.amount;
        return true;
    }

    public bool TrySet(string id, int amount = 1)
    {
        if (hasItem && this.id != id)
        {
            return false;
        }

        if (id == string.Empty)
            return false;

        hasItem = true;
        if (this.id == id)
            this.amount += amount;
        else
        {
            this.id = id;
            this.amount = amount;
        }
        return true;
    }

    public void Reset()
    {
        hasItem = false;
        id = string.Empty;
        amount = 0;
    }
}

public class PlayerController : MonoBehaviour
{
    #region API

    public Player Player
    {
        get
        {
            return _player;
        }
        set
        {
            _player = value;
        }
    }
    private Player _player;

    #endregion
    
    
    #region Properties

    protected Rigidbody Rigidbody
    {
        get
        {
            _rigidbody ??= GetComponent<Rigidbody>();
            return _rigidbody;
        }
    }
    private Rigidbody _rigidbody;

    protected Animator Animator
    {
        get
        {
            _animator ??= GetComponentInChildren<Animator>();
            return _animator;
        }
    }
    private Animator _animator;

    protected PlayerInputHandler InputHandler
    {
        get
        {
            _InputHandler ??= Player.InputHandler;
            return _InputHandler;
        }
    }
    private PlayerInputHandler _InputHandler;


    /// <summary>
    /// PropID, Amount
    /// </summary>
    protected Tool Tool
    {
        get
        {
            _tool ??= new Tool();
            return _tool;
        }
    }
    private Tool _tool;
    protected Inventory Inventory
    {
        get
        {
            _inventory ??= new Inventory();
            return _inventory;
        }   
    }
    private Inventory _inventory;

    #endregion
    
    
    #region Inspector
    
    public float angle;
    public string holdName;
    [SerializeField]private InteractableObject _fowardObject;
    [SerializeField]private InteractableObject _holdingObject;

    private static readonly string INTERACTABLE_OBJECT = "InteractableObject";
    
    private Vector3 _direction;
    private Quaternion _rotation;
    private Vector3 _velocity;
    
    // Movement
    public float moveSpeed;
    
    // Select
    [FormerlySerializedAs("fowardPlacePos")] public Vector3 forwardPlacePos;
    
    
    
    public Transform targetBlock;
    public Transform targetBlockT;
    public Transform targetBlockD;
    public Transform targetBlockL;
    public Transform targetBlockR;
    
    
    
    
    public Transform selfCellTransform;
    
    // Event
    // public event UnityAction<InteractableObject> onFindObj;
    // public event UnityAction<InteractableObject> onLostObj;
    
    // Hand
    public Transform playerHand;
    public GameObject clipTool;






    [SerializeField] private Material[] playerColors;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private int replaceMaterialIndex = 2;
    
    
    
    
    #endregion

    #region Private
    private string _aniWalk = "Walk";
    private string _aniPick = "Pick";
    #endregion

    #region Unity Messages

    private void Start()
    {
        InputHandler.onPlayerMove.AddListener(Move);
        InputHandler.onPlayerLook.AddListener(Look);
        InputHandler.OnPlayerGet.AddListener(Get);
        InputHandler.OnPlayerGetCancel.AddListener(GetCancel);
        InputHandler.onPlayerFire.AddListener(Fire);
        InputHandler.onPlayerFireCancel.AddListener(FireCancel);
        
        _direction = Vector3.zero;
        _fowardObject = null;
        _holdingObject = null;
        clipTool.SetActive(false);
        
        SetPlayerColor();
    }

    private void SetPlayerColor()
    {
        if (Player.Id < 0 || Player.Id >= playerColors.Length)
        {
            Debug.LogError("玩家顏色參數錯誤!");
            return;
        }
        
        Debug.Log(Player.Id);
        
        var playerMaterial = playerColors[Player.Id];
        var currentMaterials = skinnedMeshRenderer.materials.ToList();
        currentMaterials[replaceMaterialIndex] = playerMaterial;
        skinnedMeshRenderer.SetMaterials(currentMaterials);
    }

    private bool TryGetSelectedBlock(out BlockBase target)
    {
        var selectedPos = Vector3.zero;
        var selfPos = GridSystem.WorldToCell(transform.position);
        var lookAngle = angle;
        
        if (lookAngle >= -45 && lookAngle < 45)
        {
            selectedPos = selfPos + new Vector3(0, 0, 1);
        }
        else if (lookAngle >= 45 && lookAngle < 135)
        {
            selectedPos = selfPos + new Vector3(1, 0, 0);
        }
        else if ((lookAngle >= 135 && lookAngle < 180) || (lookAngle >= -135 && lookAngle < -180)) 
        {
            selectedPos = selfPos + new Vector3(0, 0, -1);
        }
        else
        {
            selectedPos = selfPos + new Vector3(-1, 0, 0);
        }

        if (GridSystem.Find(selectedPos, BlockType.Tool, out target))
        {
            return true;
        }
        else if (GridSystem.Find(selectedPos, BlockType.Prop, out target))
        {
            return true;
        }
        else if (GridSystem.Find(selectedPos, BlockType.Drop, out target))
        {
            return true;
        }
        else if (GridSystem.Find(selectedPos, BlockType.Floor, out target))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // private void Test_SetIndicator()
    // {
    //     targetBlock.parent = null;
    //     targetBlockT.parent = null;
    //     targetBlockD.parent = null;
    //     targetBlockL.parent = null;
    //     targetBlockR.parent = null;
    //     
    //     targetBlock.position = Grid2DSystem.WorldToCell(transform.position);
    //     targetBlockT.position = Grid2DSystem.WorldToCell(transform.position) + new Vector3(0, 0, 1);
    //     targetBlockD.position = Grid2DSystem.WorldToCell(transform.position) + new Vector3(0, 0, -1);
    //     targetBlockL.position = Grid2DSystem.WorldToCell(transform.position) + new Vector3(-1, 0, 0);
    //     targetBlockR.position = Grid2DSystem.WorldToCell(transform.position) + new Vector3(1, 0, 0);
    //     
    //    
    //     targetBlock.gameObject.SetActive(false);
    //     targetBlockT.gameObject.SetActive(false);
    //     targetBlockD.gameObject.SetActive(false);
    //     targetBlockL.gameObject.SetActive(false);
    //     targetBlockR.gameObject.SetActive(false);
    //     
    //     var testAngle = angle;
    //     // if (testAngle > 360)
    //     //     testAngle -= 360;
    //     // if (testAngle < 0)
    //     //     testAngle += 360;
    //     
    //     ConsoleProDebug.Watch("Debug Angle", $"{testAngle}");
    //     
    //     if (testAngle >= -45 && testAngle < 45)
    //     {
    //         targetBlockT.gameObject.SetActive(true);
    //     }
    //     else if (testAngle >= 45 && testAngle < 135)
    //     {
    //         targetBlockR.gameObject.SetActive(true);
    //     }
    //     else if ((testAngle >= 135 && testAngle < 180) || (testAngle >= -135 && testAngle < -180)) 
    //     {
    //         targetBlockD.gameObject.SetActive(true);
    //     }
    //     else
    //     {
    //         targetBlockL.gameObject.SetActive(true);
    //     }
    // }

    private void Update()
    {
        if (TryGetSelectedBlock(out BlockBase target))
        {
            if (target.name == "Prop_Garbage" || target.name == "Prop_Gas")
            {
                if (Tool.TryGet(out string holding))
                {
                    if (holding == "Tool_Chip")
                    {
                        Animator.SetBool(_aniPick, true);
                    }
                    else
                    {
                        Animator.SetBool(_aniPick, false);
                    }
                }
            }
            else
            {
                Animator.SetBool(_aniPick, false);
            }
        }
    }

    private void FixedUpdate()
    {
        // Move
        var isMoving = _direction.x != 0 || _direction.z != 0;
        _velocity = isMoving ? _direction.normalized * moveSpeed : Vector3.zero;
        Animator.SetBool(_aniWalk, isMoving);

        // Look
        if (angle != 0)
        {
            _rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        }
        
        // Update
        Rigidbody.velocity = _velocity;
        Rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Lerp(transform.rotation,_rotation, .7f);

        // Fixed Y Pos
        var currentPos = transform.position;
        currentPos.y = GameManager.Instance.playerYPos;
        transform.position = currentPos;
    }
    #endregion
    
    #region Events
    public void Move(Vector2 direction)
    {
        _direction.x = direction.x;
        _direction.y = 0;
        _direction.z = direction.y;
        ConsoleProDebug.Watch("InputMovement", $"{_direction}");
    }

    public void Look(float value)
    {
        angle = value;
    }

    public void Get()
    {
        Debug.Log($"玩家{Player.Id}: 點擊 A");
        if (TryGetSelectedBlock(out BlockBase target))
        {
            Debug.Log($"玩家{Player.Id}: 選擇 {target.name}");
            if (target.blockType == BlockType.Tool)
            {
                // Take
                if (Tool.TryGet(out string holding))
                {
                    Debug.Log($"玩家{Player.Id}: 持有工具 {holding}, 無法再拿取");
                }
                else if (Tool.TrySet(target.name))
                {
                    Debug.Log($"玩家{Player.Id}: 取得工具 {target.name}");
                    
                    //GridSystem.Remove(target.transform.position, BlockType.Tool);
                    
                    if (Tool.TryGet(out string itemId))
                    {
                        Debug.Log($"玩家{Player.Id}: 持有工具 {itemId}");
                        Animator.SetBool(_aniPick, true);
                    }
                }
            }
            else if (target.blockType == BlockType.Drop)
            {
                // Take or Place
                if (Inventory.TryGet(out string id, out int amount))
                {
                    if (id == target.name)
                    {
                        // if (Inventory.TrySet(id, target.amount))
                        // {
                        //     Debug.Log($"玩家{Player.Id}: 取得道具 {target.setting.Id}");
                        //     Animator.SetBool(_aniPick, true);
                        //     //GridSystem.Remove(target.transform.position, target.setting.Type);
                        // }
                    }
                    else
                    {
                        Debug.Log($"玩家{Player.Id}: 持有道具 {id}, 無法再拿取");
                    }
                }
                else
                {
                    // if (Inventory.TrySet(target.name, target.amount))
                    // {
                    //     Debug.Log($"玩家{Player.Id}: 取得道具 {target.setting.Id}");
                    //     Animator.SetBool(_aniPick, true);
                    //     //GridSystem.Remove(target.transform.position, target.setting.Type);
                    // }
                    // else
                    // {
                    //     Debug.Log($"玩家{Player.Id}: 道具取得失敗 {target.setting.Id}");
                    // }
                }
            }
            else if (target.blockType == BlockType.Floor)
            {
                // Place
                if (Inventory.TryGet(out string id, out int amount))
                {
                    var blockPrefab = GameManager.Instance.GetPrefab(id).gameObject;
                    var obj = Instantiate(blockPrefab, target.transform.position, Quaternion.identity);
                    var blockBase = obj.GetComponent<BlockBase>();
                    //blockBase.amount = amount;
                    GridSystem.Add(blockBase);
                    Inventory.Reset();
                }
                else if (Tool.TryGet(out string itemId))
                {
                    var blockPrefab = GameManager.Instance.GetPrefab(itemId).gameObject;
                    var obj = Instantiate(blockPrefab, target.transform.position, Quaternion.identity);
                    GridSystem.Add(obj.GetComponent<BlockBase>());
                    
                    Tool.Reset();
                }
                else
                {
                    // Nothing
                }
            }
            else if (target.blockType == BlockType.Chest)
            {
                // 放進去車廂
            }
        }
        else
        {
            // Nothing
        }
    }

    private void GetCancel()
    {
        Animator.SetBool(_aniPick, false);
    }

    public void Fire()
    {
        return;
        
        Debug.Log($"玩家{Player.Id}: 點擊 Fire");
        if (TryGetSelectedBlock(out BlockBase target))
        {
            Debug.Log($"玩家{Player.Id}: 選擇 {target.name}");

            if (target.blockType == BlockType.Prop)
            {
                if (target.name == "Garbage" || target.name == "Gas")
                {
                    if (Tool.TryGet(out string holding))
                    {
                        Animator.SetBool(_aniPick, true);
                    }
                }
            }
        }
        else
        {
            // Nothing
        }
    }

    private void FireCancel()
    {
        Animator.SetBool(_aniPick, false);
    }
    #endregion
}
