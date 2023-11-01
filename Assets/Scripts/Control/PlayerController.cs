using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public float angle;
    public string holdName;
    [SerializeField]private GameObject _fowardObject;
    [SerializeField]private GameObject _holdingObject;
    private float _angleRange = 22.5f;

    private static readonly string tagTool = "Tool";
    
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    private Vector3 _rotation;
    
    // Movement
    public float moveSpeed;
    
    // Select
    [FormerlySerializedAs("fowardPos")] public Vector3 fowardPlacePos;
    public GameObject targetBlock;
    public Transform selfCellTransform;

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
        
        fowardPlacePos = targetBlockCellPos;
        fowardPlacePos.y = GameManager.Instance.propsYPos;
        
        // Check Forward Props
        var forwardProps = Grid2DSystem.Find(targetBlockCellPos);
        if (forwardProps && forwardProps.CompareTag("Tool"))
        {
            // 這邊要改成能兼容破壞地形
            _fowardObject = forwardProps;
        }
    }

    private void FixedUpdate()
    {
        if (_direction.x != 0 || _direction.z != 0)
        {
            //_rigidbody.MovePosition(_rigidbody.position + _direction * _moveSpeed);
            _rigidbody.velocity = _direction * moveSpeed;
            UpdateVector();
            FindItem();
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }

        if (_rotation.x != 0 || _rotation.y != 0)
        {
            _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, angle, 0)));
            FindItem();
        }


        _rigidbody.angularVelocity = Vector3.zero;
        
        ConsoleProDebug.Watch("Velocity", $"{_rigidbody.velocity}");
        ConsoleProDebug.Watch("AngularVelocity", $"{_rigidbody.angularVelocity}");


        
        
        
        
        //targetBlock.transform.position = targetBlockPos;
        
        //ConsoleProDebug.Watch("TargetBlockPos", $"{targetBlockPos}");

        
        // Fixed Player Y Pos
        var currentPlayerPos = transform.position;
        currentPlayerPos.y = GameManager.Instance.playerYPos;
        transform.position = currentPlayerPos;
    }

    private void FindItem()
    {
        
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
            if (_fowardObject is not null)
            {
                _fowardObject.transform.SetParent(transform);
                _fowardObject.transform.localPosition = new Vector3(0, 1, 0);
                _holdingObject = _fowardObject;
            }
        }
        else
        {
            _holdingObject.transform.parent = null;
            _holdingObject.transform.position = fowardPlacePos;
            
            _holdingObject = null;
        }
        
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagTool))
        {
            var ctrl = other.GetComponent<ToolController>();
            
            
            //inRangeTools.Add(new InRangeTool(ctrl, CaculateVector(ctrl.transform.position)));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagTool))
        {
            var ctrl = other.GetComponent<ToolController>();
            
            
            //inRangeTools.RemoveAll(x => x.tool == ctrl);
        }
    }

    private Vector2 CaculateVector(Vector3 targetPos)
    {
        var vector = (targetPos - transform.position).normalized;
        return new Vector2(vector.x, vector.z);
    }

    private void UpdateVector()
    {
        // for (var i = 0; i < inRangeTools.Count; i++)
        // {
        //     var e = inRangeTools[i];
        //     e.vector = CaculateVector(e.tool.transform.position);
        //     e.angle = Mathf.Atan2(e.vector.x, e.vector.y) * Mathf.Rad2Deg;
        // }
    }
}
