using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    #region Inspector
    public float angle;
    public string holdName;
    [SerializeField]private InteractableObject _fowardObject;
    [SerializeField]private InteractableObject _holdingObject;

    private static readonly string INTERACTABLE_OBJECT = "InteractableObject";
    
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    private Quaternion _rotation;
    private Vector3 _velocity;
    
    // Movement
    public float moveSpeed;
    
    // Select
    [FormerlySerializedAs("fowardPlacePos")] public Vector3 forwardPlacePos;
    public GameObject targetBlock;
    public Transform selfCellTransform;
    
    // Event
    // public event UnityAction<InteractableObject> onFindObj;
    // public event UnityAction<InteractableObject> onLostObj;
    
    // Hand
    public Transform playerHand;
    public GameObject clipTool;
    #endregion

    #region Private
    private PlayerInputHandler _inputHandler;
    private Animator _animator;
    private string _aniWalk = "Walk";
    private string _aniPick = "Pick";
    #endregion

    #region Unity Messages
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _direction = Vector3.zero;
        _fowardObject = null;
        _holdingObject = null;
    }

    private void Start()
    {
        clipTool.SetActive(false);
    }

    private void Update()
    {
        // Self Cell Pos
        selfCellTransform.parent = null;
        selfCellTransform.position = Grid2DSystem.WorldToCell(transform.position);
        selfCellTransform.rotation = transform.rotation;
        
        // Get Forward Cell
        var targetBlockPos = selfCellTransform.TransformPoint(new Vector3(0, 0, 1.4f));
        var targetBlockCellPos = Grid2DSystem.WorldToCell(targetBlockPos);
        targetBlockCellPos.y = GameManager.current.propsYPos;
        targetBlock.transform.parent = null;
        targetBlock.transform.position = targetBlockCellPos;
        
        // Forward Pos
        forwardPlacePos = targetBlockCellPos;
        forwardPlacePos.y = GameManager.current.propsYPos;
        
        // Check Forward Props
        var forwardProps = Grid2DSystem.Find(targetBlockCellPos);
        if (forwardProps && forwardProps.CompareTag(INTERACTABLE_OBJECT))
        {
            _fowardObject = forwardProps;
            _fowardObject?.OnFindObj();
            
            // Check Destroy Target
            _fowardObject.Check(_holdingObject);
            _fowardObject.DoDestroy();
        }
        else
        {
            _fowardObject?.OnLostObj();
            _fowardObject = null;
        }
    }

    private void FixedUpdate()
    {
        // Move
        var isMoving = _direction.x != 0 || _direction.z != 0;
        _velocity = isMoving ? _direction.normalized * moveSpeed : Vector3.zero;
        _animator.SetBool(_aniWalk, isMoving);

        // Look
        if (angle != 0)
        {
            _rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        }
        
        // Update
        _rigidbody.velocity = _velocity;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Lerp(transform.rotation,_rotation, .7f);

        // Fixed Y Pos
        var currentPos = transform.position;
        currentPos.y = GameManager.current.playerYPos;
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
        _animator.SetBool(_aniPick, true);
        
        if (_holdingObject is null)
        {
            if (_fowardObject is not null && _fowardObject.type == InteractableObject.ObjectType.Props)
            {
                _fowardObject.RemoveFromGrid();
                _fowardObject.gameObject.SetActive(false);
                
                if (_fowardObject.id == 0)
                {
                    clipTool.SetActive(true);
                }
                else
                {
                    clipTool.SetActive(false);
                }
                
                // _fowardObject.transform.SetParent(playerHand);
                //
                // // Obj Holding Pos
                // _fowardObject.transform.localPosition = new Vector3(0, 0, 0);
                // _fowardObject.transform.localEulerAngles = Vector3.zero;
                
                _holdingObject = _fowardObject;
            }
        }
        else
        {
            var obj = Grid2DSystem.Find(forwardPlacePos);
            Debug.Log($"Try Place {_holdingObject}");
            if (obj)
            {
                if (obj.stack)
                {
                    obj.AddStack(_holdingObject.stackAmount);
                    Debug.Log($"Add Exist {_holdingObject.stackAmount} {_holdingObject}");
                    Destroy(_holdingObject.gameObject);
                    _holdingObject = null;
                }
            }
            else
            {
                clipTool.SetActive(false);
                _holdingObject.gameObject.SetActive(true);
                _holdingObject.transform.parent = null;
                _holdingObject.transform.position = forwardPlacePos;
                _holdingObject.transform.eulerAngles = Vector3.zero;
                _holdingObject.AddToGrid();
                if (_holdingObject.stack && _holdingObject.stackAmount == 0)
                {
                    _holdingObject.AddStack(1);
                    Debug.Log($"Add 1 {_holdingObject}");
                }
                Debug.Log($"Placed {_holdingObject}");
                _holdingObject = null;
            }
        }
    }

    private void GetCancel()
    {
        _animator.SetBool(_aniPick, false);
    }

    public void Fire()
    {
        _animator.SetBool(_aniPick, true);
        
        // Test
        if (_holdingObject && _holdingObject.id == 0)
        {
            var targetPos = targetBlock.transform.position;
            var cellPos = new Vector2(targetPos.x, targetPos.z);
            Debug.Log(cellPos);
            var roadBlockObj = EnvSpawner.current.Map_Find(cellPos);
            if (roadBlockObj)
            {
                Debug.Log(roadBlockObj);
                var roadBlock = roadBlockObj.GetComponent<RoadBlock>();
                if (!roadBlock)
                {
                    EnvSpawner.current.Map_Replace(cellPos, EnvSpawner.current.SpawnBlock(EnvSpawner.BlockType.Road, new Vector3(cellPos.x, 0, cellPos.y)));
                    Debug.Log("Replace");
                }
            }
        }
    }

    private void FireCancel()
    {
        _animator.SetBool(_aniPick, false);
    }
    #endregion
    
    public void SetInputHandler(PlayerInputHandler playerInputHandler)
    {
        _inputHandler = playerInputHandler;
        _inputHandler.onPlayerMove.AddListener(Move);
        _inputHandler.onPlayerLook.AddListener(Look);
        _inputHandler.onPlayerGet.AddListener(Get);
        _inputHandler.onPlayerGetCancel.AddListener(GetCancel);
        _inputHandler.onPlayerFire.AddListener(Fire);
        _inputHandler.onPlayerFireCancel.AddListener(FireCancel);
    }
}
