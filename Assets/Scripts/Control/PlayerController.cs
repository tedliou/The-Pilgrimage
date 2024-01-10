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
    [ShowInInspector] public bool hasFood => foodAmount > 0;
    [ShowInInspector] public bool hasRoad => roadAmount > 0;
    
    public int gasAmount = 0;
    public int garbageAmount = 0;
    public int foodAmount;
    
    public int gasAmountLimit = 10;
    public int garbageAmountLimit = 10;
    public int foodAmountLimit = 10;
    
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
        bool getClip = true, getBomb = true, getTray = true, getGas = false, getGasWreck = true, getGarbage = false, getGarbageWreck = true, getFood = false, getRoad = true;
        
        // 工具判斷
        if (hasClip)
        {
            getBomb = false;
            getTray = false;
            getGarbage = true;
            getFood = false;
            getRoad = false;
        }
        if (hasBomb)
        {
            getClip = false;
            getTray = false;
            getGas = true;
            getFood = false;
            getRoad = false;
        }
        if (hasTray)
        {
            getBomb = false;
            getClip = false;
            getGas = false;
            getGarbage = false;
            getFood = true;
            getRoad = false;
        }

        // 雜物判斷
        if (!hasClip && !hasBomb && !hasTray && hasGas)
        {
            getClip = false;
            getBomb = false;
            getTray = false;
            getGarbageWreck = false;
            getFood = false;
            getRoad = false;
        }
        if (!hasClip && !hasBomb && !hasTray && hasGarbage)
        {
            getClip = false;
            getBomb = false;
            getTray = false;
            getGasWreck = false;
            getFood = false;
            getRoad = false;
        }
        if (!hasClip && !hasBomb && hasTray)
        {
            getClip = false;
            getBomb = false;
            getTray = false;
            getGasWreck = false;
            getGarbageWreck = false;
            getFood = true;
            getRoad = false;
        }

        if (hasRoad)
        {
            getClip = false;
            getBomb = false;
            getTray = false;
            getGasWreck = false;
            getGarbageWreck = false;
            getFood = false;
            getRoad = true;
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
        if (!getTray)
        {
            colliders.RemoveAll(c => c.CompareTag("Tray"));
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
        if (!getFood)
        {
            colliders.RemoveAll(c => c.CompareTag("Food"));
        }
        if (!getRoad)
        {
            colliders.RemoveAll(c => c.CompareTag("Road"));
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
        var selfPos = GridSystem.WorldToCell(transform.position);
        if (GridSystem.Find(selfPos, CellType.Top, out target))
        {
            return true;
        }
        else if (GridSystem.Find(selfPos, CellType.Down, out target))
        {
            return true;
        }

        return false;
        
        //
        // var selectedPos = Vector3.zero;
        // var lookAngle = angle;
        //
        // if (lookAngle >= -45 && lookAngle < 45)
        // {
        //     selectedPos = selfPos + new Vector3(0, 0, 1);
        // }
        // else if (lookAngle >= 45 && lookAngle < 135)
        // {
        //     selectedPos = selfPos + new Vector3(1, 0, 0);
        // }
        // else if ((lookAngle >= 135 && lookAngle < 180) || (lookAngle >= -135 && lookAngle < -180)) 
        // {
        //     selectedPos = selfPos + new Vector3(0, 0, -1);
        // }
        // else
        // {
        //     selectedPos = selfPos + new Vector3(-1, 0, 0);
        // }
        //
        // if (GridSystem.Find(selectedPos, CellType.Top, out target))
        // {
        //     return true;
        // }
        // else if (GridSystem.Find(selectedPos, CellType.Down, out target))
        // {
        //     return true;
        // }
        // else
        // {
        //     return false;
        // }
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
            if (block)
            {
                switch (block.blockType)
                {
                    case BlockType.Clip:
                        hasClip = true;
                        GridSystem.Remove(block);
                        GameManager.Instance.clipIndicator.SetFollowTransform(null);
                        Animator.SetBool(m_aniPick, true);
                        return;
            
                    case BlockType.Bomb:
                        hasBomb = true;
                        GridSystem.Remove(block);
                        GameManager.Instance.bombIndicator.SetFollowTransform(null);
                        Animator.SetBool(m_aniPick, true);
                        return;
                }
            }
        }
        
        if (hasTargetObj)
        {
            // 找腳下
            var block = targetBlock;
            switch (block.blockType)
            {
                case BlockType.Floor:
                    if (hasRoad)
                    {
                        var blockPos = block.transform.position;
                        var aroundRoadCount = GridSystem.FindAround(block).Where(x => x.blockType == BlockType.Road && x.GetComponent<RoadBlock>() == RoadBlock.LastNode).Count();
                        
                        if (aroundRoadCount > 0)
                        {
                            GridSystem.Remove(block);
                            var roadObj = Instantiate(GameManager.Instance.roadPrefab);
                            roadObj.transform.position = blockPos;
                            var roadBlock = roadObj.GetComponent<RoadBlock>();
                            GridSystem.Add(roadBlock);
                            roadBlock.Enable();
                            roadAmount--;
                        }
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
                    return;
            }
        }
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
