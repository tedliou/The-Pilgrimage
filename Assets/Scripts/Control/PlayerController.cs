using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Rigidbody Rigidbody
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
    
    private Vector3 m_direction;
    private Quaternion m_rotation;
    private Vector3 m_velocity;
    
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






    [SerializeField] private Material[] playerColors;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private int replaceMaterialIndex = 2;


    public bool hasToolProp = false;
    public bool hasBombProp = false;
    public int gasAmount = 0;
    public int garbageAmount = 0;
    public int gasAmountLimit = 10;
    public int garbageAmountLimit = 10;
    public int roadAmount = 0;
    public GameObject clipTool;
    
    #endregion

    #region Private
    private string m_aniWalk = "Walk";
    private string _aniPick = "Pick";
    #endregion




    
    private Vector2 m_screenBounds;
    

    #region Unity Messages

    private void Start()
    {
        InputHandler.onPlayerMove.AddListener(Move);
        InputHandler.onPlayerLook.AddListener(Look);
        InputHandler.OnPlayerGet.AddListener(Get);
        InputHandler.OnPlayerGetCancel.AddListener(GetCancel);
        InputHandler.onPlayerFire.AddListener(Fire);
        InputHandler.onPlayerFireCancel.AddListener(FireCancel);
        InputHandler.onPlayerReset.AddListener(ResetPosition);
        
        m_direction = Vector3.zero;
        
        SetPlayerColor();
    }

    private void Update()
    {
        clipTool.SetActive(hasToolProp);
    }

    private void SetPlayerColor()
    {
        if (Player.Id < 0 || Player.Id >= playerColors.Length)
        {
            Debug.LogError("玩家顏色參數錯誤!");
            return;
        }
        
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

    private void FixedUpdate()
    {
        //var look = false;
        var move = false;
        
        // // Look
        // var updateRotation = Quaternion.Euler(new Vector3(0, angle, 0));
        // if (m_rotation != updateRotation)
        // {
        //     look = true;
        //     m_rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        // }
        
        // Move
        if (m_direction.x != 0 || m_direction.z != 0)
        {
            move = true;
            
            // if (!InputHandler.isLooking)
            // {
            //     // Auto Rotate
            //     transform.forward = m_direction.normalized;
            // }
            transform.forward = m_direction.normalized;
            
            m_velocity = m_direction.normalized * moveSpeed;
            SetMoveEffet(true);
        }
        else
        {
            m_velocity = Vector3.zero;
            SetMoveEffet(false);
        }
        

        
        // Update
        Rigidbody.velocity = m_velocity;
        Rigidbody.angularVelocity = Vector3.zero;

        // if (InputHandler.isLooking)
        // {
        //     transform.rotation = Quaternion.Lerp(transform.rotation,m_rotation, .7f);
        // }

        
        
        // Fixed Y Pos
        var updatedPos = transform.position;
        updatedPos.y = GameManager.Instance.playerYPos;
        //transform.position = updatedPos;
    }
    #endregion

    public void SetMoveEffet(bool active)
    {
        var state = Animator.GetBool(m_aniWalk);
        if (state == active)
        {
            return;
        }
        
        Animator.SetBool(m_aniWalk, active);
        if (active)
        {
            FootstepsSFX.Instance.Play();
        }
        else
        {
            FootstepsSFX.Instance.Stop();
        }
    }
    
    #region Events
    public void Move(Vector2 direction)
    {
        m_direction.x = direction.x;
        m_direction.y = 0;
        m_direction.z = direction.y;
    }

    public void Look(float value)
    {
        angle = value;
    }

    public void Get()
    {

        if (canPlaceWreckProp)
        {
            var car = triggerBlock.GetComponent<CarController>();

            if (gasAmount > 0)
            {
                car.PutGas(gasAmount);
                gasAmount = 0;
            }
            else if (garbageAmount > 0)
            {
                car.PutGarbage(garbageAmount);
                garbageAmount = 0;
            }
            
            
            return;
        }
        else if (canTakeProp)
        {
            if (garbageAmount > 0 || gasAmount > 0 || hasToolProp || hasBombProp)
            {
                return;
            }
            
            var carExtend = triggerBlock.GetComponent<CarExtendController>();
            var amount = carExtend.Take();
            if (amount > 0)
            {
                roadAmount = amount;
            }
            
            return;   
        }
        
        Debug.Log(nameof(Get));

        
        if (TryGetSelectedBlock(out BlockBase target))
        {
            Debug.Log(target.name);
            
            if (target.blockType == BlockType.Tool)
            {
                // Take
                if (hasToolProp)
                {
                    // Nothing
                }
                else
                {
                    hasToolProp = true;
                    GridSystem.Remove(target, BlockType.Tool, CellType.Top);
                    Animator.SetBool(_aniPick, true);
                }
            }
            else if (target.blockType == BlockType.GasWreck)
            {
                if (garbageAmount > 0)
                {
                    return;
                }

                if (hasToolProp || hasBombProp)
                {
                    return;
                }
                
                var prop = target.GetComponent<PropBlock>();

                if (gasAmount > 0)
                {
                    var placeAmount = prop.Place(gasAmount);
                    gasAmount -= placeAmount;
                }
                else
                {
                    var takeAmount = prop.Pickup();
                    gasAmount += takeAmount;
                }
            }
            else if (target.blockType == BlockType.GarbageWreck)
            {
                // Take or Place
                if (gasAmount > 0)
                {
                    return;
                }

                if (hasToolProp || hasBombProp)
                {
                    return;
                }
                
                var prop = target.GetComponent<PropBlock>();

                if (garbageAmount > 0)
                {
                    var placeAmount = prop.Place(garbageAmount);
                    garbageAmount -= placeAmount;
                }
                else
                {
                    var takeAmount = prop.Pickup();
                    garbageAmount += takeAmount;
                }
            }
            else if (target.blockType == BlockType.Floor)
            {
                // Place
                if (hasToolProp)
                {
                    var toolObj = Instantiate(GameManager.Instance.toolPrefab);
                    toolObj.transform.position = target.transform.position;
                    hasToolProp = false;
                }
                else if (hasBombProp)
                {
                    var bombObj = Instantiate(GameManager.Instance.bombPrefab);
                    bombObj.transform.position = target.transform.position;
                    hasBombProp = false;
                }
                else if (garbageAmount > 0)
                {
                    var garbageObj = Instantiate(GameManager.Instance.garbagePrefab);
                    garbageObj.transform.position = target.transform.position;
                    garbageObj.GetComponent<PropBlock>().SetAmount(garbageAmount);
                    garbageAmount = 0;
                }
                else if (gasAmount > 0)
                {
                    var gasObj = Instantiate(GameManager.Instance.gasPrefab);
                    gasObj.transform.position = target.transform.position;
                    gasObj.GetComponent<PropBlock>().SetAmount(gasAmount);
                    gasAmount = 0;
                }else if (roadAmount > 0)
                {
                    GridSystem.Remove(target, BlockType.Floor, CellType.Down);
                    var roadObj = Instantiate(GameManager.Instance.roadPrefab);
                    roadObj.transform.position = target.transform.position;
                    roadAmount --;
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
        if (TryGetSelectedBlock(out BlockBase target))
        {
            if (target.blockType == BlockType.Garbage)
            {
                var prop = target.GetComponent<PropBlock>();
                if (hasToolProp)
                {
                    prop.Wreck();
                    Animator.SetBool(_aniPick, true);
                }
            }
            else if (target.blockType == BlockType.Gas)
            {
                var prop = target.GetComponent<PropBlock>();
                if (hasToolProp)
                {
                    prop.Wreck();
                    Animator.SetBool(_aniPick, true);
                }
            }
        }
    }

    private void FireCancel()
    {
        Animator.SetBool(_aniPick, false);
    }

    private void ResetPosition()
    {
        GameManager.Instance.ResetPlayerPos(this);
    }
    #endregion


    public bool canPlaceWreckProp = false;
    public bool canTakeProp = false;
    public GameObject triggerBlock;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            canPlaceWreckProp = true;
            triggerBlock = other.gameObject;
        }
        else if (other.CompareTag("CarExtend"))
        {
            canTakeProp = true;
            triggerBlock = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            canPlaceWreckProp = false;
            triggerBlock = null;
        }
        else if (other.CompareTag("CarExtend"))
        {
            canTakeProp = false;
            triggerBlock = null;
        }
    }

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
}
