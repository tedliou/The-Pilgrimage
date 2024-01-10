using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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

public class PlayerController : CustomBehaviour<PlayerController>
{

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


    // 工具和物件條件
    [Header("State")]
    public bool hasClip = false;
    public bool hasBomb = false;
    public bool hasTray = false;
    
    [ShowInInspector] public bool hasGas => gasAmount > 0;
    [ShowInInspector] public bool hasGarbage => garbageAmount > 0;
    [ShowInInspector] public bool hasRoad => roadAmount > 0;
    
    public int gasAmount = 0;
    public int garbageAmount = 0;
    
    public int gasAmountLimit = 10;
    public int garbageAmountLimit = 10;
    
    public int roadAmount = 0;
    
    public GameObject clipInHand;
    public GameObject bombInHand;
    public GameObject roadInHand;
    public GameObject trayInHand;
    
    #endregion

    private string m_aniWalk = "Walk";
    private string m_aniPick = "Pick";

    // 偵測範圍
    public float radius = 1;
    // 範圍內的物件列表
    public List<Collider> closetList = new List<Collider>();
    [ShowInInspector]
    public Transform closetObj => closetList.Count > 0 ? closetList[0].transform : null;
    


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

        roadAmount = 100;
        
        SetPlayerColor();
    }

    private void Update()
    {
        clipInHand.SetActive(hasClip);
        bombInHand.SetActive(hasBomb);

        #region 偵測周圍物件

        var colliders = Physics.OverlapSphere(transform.position, radius).ToList();
        
        // 刪除玩家
        colliders.RemoveAll(c => c.CompareTag("Player"));
        
        // 刪除當前條件無法互動的物件
        // 有夾子：不能拿炸彈、瘴氣、破壞物
        // 沒夾子：不能拿垃圾、瘴氣
        bool getClip = true, getBomb = true, getGas = false, getGasWreck = true, getGarbage = false, getGarbageWreck = true;
        
        // 工具判斷
        if (hasClip)
        {
            getBomb = false;
            getGarbage = true;
        }
        if (hasBomb)
        {
            getClip = false;
            getGas = true;
        }

        // 垃圾判斷
        if (!hasClip && !hasBomb && hasGas)
        {
            getClip = false;
            getBomb = false;
            getGarbageWreck = false;
        }
        if (!hasClip && !hasBomb && hasGarbage)
        {
            getClip = false;
            getBomb = false;
            getGasWreck = false;
        }

        #region 刪除不相干物件

        if (!getClip)
        {
            colliders.RemoveAll(c => c.CompareTag("Clip"));
        }
        if (!getBomb)
        {
            colliders.RemoveAll(c => c.CompareTag("Bomb"));
        }
        if (!getGas)
        {
            colliders.RemoveAll(c => c.CompareTag("Gas"));
        }
        if (!getGarbage)
        {
            colliders.RemoveAll(c => c.CompareTag("Garbage"));
        }
        if (!getGasWreck)
        {
            colliders.RemoveAll(c => c.CompareTag("GasWreck"));
        }
        if (!getGarbageWreck)
        {
            colliders.RemoveAll(c => c.CompareTag("GarbageWreck"));
        }

        #endregion
        
        
        
        
        // 由近到遠排序物件
        if (colliders.Count > 0)
        {
            closetList = colliders.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToList();
        }
        else
        {
            closetList.Clear();
        }

        #endregion
        
        
        
        
        
        
    }

    private void LateUpdate()
    {
        // 選擇高亮
        var hasClosetObj = closetObj != null;
        var hasTargetObj = TryGetSelectedBlock(out BlockBase targetBlock);
        if (hasClosetObj)
        {
            closetObj.GetComponent<BlockBase>()?.Select();
        }
        else if (hasTargetObj)
        {
            targetBlock?.Select();
        }
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

        if (GridSystem.Find(selectedPos, CellType.Top, out target))
        {
            return true;
        }
        else if (GridSystem.Find(selectedPos, CellType.Down, out target))
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
        transform.position = updatedPos;
    }

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
        var hasClosetObj = closetObj != null;
        var hasTargetObj = TryGetSelectedBlock(out BlockBase targetBlock);
        
        Log($"{Player.Id}: {nameof(Get)}, {closetObj}, {targetBlock}");
        
        if (hasClosetObj)
        {
            // 找最近的
            var block = closetObj.GetComponent<BlockBase>();
            switch (block.blockType)
            {
                case BlockType.Clip:
                    hasClip = true;
                    GridSystem.Remove(block);
                    GameManager.Instance.clipIndicator.SetFollowTransform(null);
                    Animator.SetBool(m_aniPick, true);
                    break;
            
                case BlockType.Bomb:
                    hasBomb = true;
                    GridSystem.Remove(block);
                    GameManager.Instance.bombIndicator.SetFollowTransform(null);
                    Animator.SetBool(m_aniPick, true);
                    break;
            }
        }
        else if (hasTargetObj)
        {
            // 找十字的
            var block = targetBlock;
            switch (block.blockType)
            {
                case BlockType.Floor:
                    if (hasRoad)
                    {
                        var blockPos = block.transform.position;
                        GridSystem.Remove(block);
                        var roadObj = Instantiate(GameManager.Instance.roadPrefab);
                        roadObj.transform.position = blockPos;
                        var roadBlock = roadObj.GetComponent<RoadBlock>();
                        GridSystem.Add(roadBlock);
                        roadBlock.FindAroundRoads();
                        roadAmount--;
                    }
                    else
                    {
                        var prefab = GameManager.Instance.clipPrefab;
                        if (hasBomb)
                        {
                            prefab = GameManager.Instance.bombPrefab;
                        }
                        else if (hasGas)
                        {
                            prefab = GameManager.Instance.gasPrefab;
                        }
                        else if (hasGarbage)
                        {
                            prefab = GameManager.Instance.garbagePrefab;
                        }

                        var obj = Instantiate(prefab);
                        obj.transform.position = GridSystem.WorldToCell(transform.position);
                        Animator.SetBool(m_aniPick, true);

                        if (hasClip)
                        {
                            hasClip = false;
                            GameManager.Instance.clipIndicator.SetFollowTransform(obj.transform);
                        }
                        else if (hasBomb)
                        {
                            hasBomb = false;
                            GameManager.Instance.bombIndicator.SetFollowTransform(obj.transform);
                        }
                        else if (hasGas)
                        {
                            gasAmount = 0;
                            obj.GetComponent<PropBlock>().SetAmount(gasAmount);
                        }
                        else if (hasGarbage)
                        {
                            garbageAmount = 0;
                            obj.GetComponent<PropBlock>().SetAmount(garbageAmount);
                        }
                    }
                    
                    break;
            }
        }

        /*
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
            if (garbageAmount > 0 || gasAmount > 0 || hasClip || hasBomb)
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
        */
        /*
        if (closetObj == null)
        {
            if (TryGetSelectedBlock(out BlockBase target))
            {
                if (target.blockType == BlockType.Floor)
                {
                    // Place
                    if (hasClip)
                    {
                        var toolObj = Instantiate(GameManager.Instance.toolPrefab);
                        toolObj.transform.position = GridSystem.WorldToCell(transform.position);
                        GameManager.Instance.clipIndicator.SetFollowTransform(toolObj.transform);

                        hasClip = false;
                    }
                    else if (hasBomb)
                    {
                        var bombObj = Instantiate(GameManager.Instance.bombPrefab);
                        bombObj.transform.position = target.transform.position;
                        hasBomb = false;
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
                    }
                    else if (roadAmount > 0)
                    {
                        GridSystem.Remove(target, BlockType.Floor, CellType.Down);
                        var roadObj = Instantiate(GameManager.Instance.roadPrefab);
                        roadObj.transform.position = target.transform.position;
                        roadAmount--;
                    }
                }
            }
        }
        else
        {
            var target = closetObj.GetComponent<BlockBase>();
            if (target != null)
            {
                Debug.Log(target.name);

                if (target.blockType == BlockType.Tool)
                {
                    // Take
                    if (hasClip)
                    {
                        // Nothing
                    }
                    else
                    {
                        hasClip = true;
                        GridSystem.Remove(target, BlockType.Tool, CellType.Top);
                        GameManager.Instance.clipIndicator.SetFollowTransform(null);
                        Animator.SetBool(m_aniPick, true);
                    }
                }
                else if (target.blockType == BlockType.GasWreck)
                {
                    if (garbageAmount > 0)
                    {
                        return;
                    }

                    if (hasClip || hasBomb)
                    {
                        return;
                    }

                    var prop = target.GetComponent<PropBlock>();

                    if (gasAmount > 0)
                    {
                        if (gasAmount < gasAmountLimit)
                        {
                            var takeAmount = prop.Pickup();
                            gasAmount += takeAmount;
                        }
                        else
                        {
                            var placeAmount = prop.Place(gasAmount);
                            gasAmount -= placeAmount;
                        }
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

                    if (hasClip || hasBomb)
                    {
                        return;
                    }

                    var prop = target.GetComponent<PropBlock>();

                    if (garbageAmount > 0)
                    {
                        if (garbageAmount < garbageAmountLimit)
                        {
                            var takeAmount = prop.Pickup();
                            garbageAmount += takeAmount;
                        }
                        else
                        {
                            var placeAmount = prop.Place(garbageAmount);
                            garbageAmount -= placeAmount;
                        }
                    }
                    else
                    {
                        var takeAmount = prop.Pickup();
                        garbageAmount += takeAmount;
                    }
                }
                else if (target.blockType == BlockType.Chest)
                {
                    // 放進去車廂
                }
            }
        }*/
    }

    private void GetCancel()
    {
        Animator.SetBool(m_aniPick, false);
    }

    public void Fire()
    {
        var hasClosetObj = closetObj != null;
        
        Log($"{Player.Id}: {nameof(Fire)}, {closetObj}, {targetBlock}");
        
        if (hasClosetObj)
        {
            var block = closetObj.GetComponent<PropBlock>();
            block.Wreck();
            Animator.SetBool(m_aniPick, true);
        }
    }

    private void FireCancel()
    {
        Animator.SetBool(m_aniPick, false);
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
