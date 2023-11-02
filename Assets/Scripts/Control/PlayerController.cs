using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public float angle;
    public string holdName;
    [SerializeField]private InteractableObject _fowardObject;
    [SerializeField]private InteractableObject _holdingObject;

    private static readonly string INTERACTABLE_OBJECT = "InteractableObject";
    
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    private Vector3 _rotation;
    
    // Movement
    public float moveSpeed;
    
    // Select
    [FormerlySerializedAs("fowardPlacePos")] public Vector3 forwardPlacePos;
    public GameObject targetBlock;
    public Transform selfCellTransform;
    
    // Event
    // public event UnityAction<InteractableObject> onFindObj;
    // public event UnityAction<InteractableObject> onLostObj;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _direction = Vector3.zero;
        _rotation = Vector3.zero;
        _fowardObject = null;
        _holdingObject = null;
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
        targetBlockCellPos.y = GameManager.Instance.propsYPos;
        targetBlock.transform.parent = null;
        targetBlock.transform.position = targetBlockCellPos;
        
        // Forward Pos
        forwardPlacePos = targetBlockCellPos;
        forwardPlacePos.y = GameManager.Instance.propsYPos;
        
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
        if (_direction.x != 0 || _direction.z != 0)
        {
            _rigidbody.velocity = _direction.normalized * moveSpeed;
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }

        if (_rotation.x != 0 || _rotation.y != 0)
        {
            _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, angle, 0)));
        }


        _rigidbody.angularVelocity = Vector3.zero;
        
        // ConsoleProDebug.Watch("Velocity", $"{_rigidbody.velocity}");
        // ConsoleProDebug.Watch("AngularVelocity", $"{_rigidbody.angularVelocity}");
        // ConsoleProDebug.Watch("TargetBlockPos", $"{targetBlockPos}");

        // Fixed Player Y Pos
        var currentPlayerPos = transform.position;
        currentPlayerPos.y = GameManager.Instance.playerYPos;
        transform.position = currentPlayerPos;
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        _direction.x = dir.x;
        _direction.y = 0;
        _direction.z = dir.y;
        ConsoleProDebug.Watch("InputMovement", $"{_direction}");
    }

    public void OnRotation(InputAction.CallbackContext ctx)
    {
        var rot = ctx.ReadValue<Vector2>();
        _rotation = rot;
        if (_rotation.magnitude >= 1)
        {
            angle = Mathf.Atan2(rot.x, rot.y) * Mathf.Rad2Deg;
        }
    }

    public void OnInteractive(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        
        if (_holdingObject is null)
        {
            if (_fowardObject is not null && _fowardObject.type == InteractableObject.ObjectType.Props)
            {
                _fowardObject.RemoveFromGrid();
                _fowardObject.transform.SetParent(transform);
                
                // Obj Holding Pos
                _fowardObject.transform.localPosition = new Vector3(0, .6f, 0);
                _fowardObject.transform.localEulerAngles = Vector3.zero;
                
                _holdingObject = _fowardObject;
            }
        }
        else
        {
            if (!Grid2DSystem.Find(forwardPlacePos))
            {
                _holdingObject.transform.parent = null;
                _holdingObject.transform.position = forwardPlacePos;
                _holdingObject.transform.eulerAngles = Vector3.zero;
                _holdingObject.AddToGrid();
                _holdingObject = null;
            }
        }
    }
}
