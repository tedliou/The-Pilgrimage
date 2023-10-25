using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class InRangeTool
{
    public ToolController tool;
    public Vector2 vector;
    public float angle;

    public InRangeTool(ToolController tool, Vector2 vector)
    {
        this.tool = tool;
        this.vector = vector;
    }

    public void SetVector(Vector2 vector)
    {
        this.vector.x = vector.x;
        this.vector.y = vector.y;
    }
}

public class PlayerController : MonoBehaviour
{
    public List<InRangeTool> inRangeTools = new List<InRangeTool>();
    public float angle;
    public string holdName;
    private InRangeTool _hold;
    private InRangeTool _holding;
    private float _angleRange = 22.5f;
    public Grid grid;

    private static readonly string tagTool = "Tool";
    
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    private float _moveSpeed;
    private Vector3 _rotation;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _direction = Vector3.zero;
        _moveSpeed = .1f;
        _rotation = Vector3.zero;
        _hold = null;
        _holding = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_direction.x != 0 || _direction.z != 0)
        {
            _rigidbody.MovePosition(_rigidbody.position + _direction * _moveSpeed);
            UpdateVector();
            FindItem();
        }

        if (_rotation.x != 0 || _rotation.y != 0)
        {
            _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, angle, 0)));
            FindItem();
        }
    }

    private void FindItem()
    {
        for (int i = 0; i < inRangeTools.Count; i++)
        {
            var e = inRangeTools[i];
            var delta = Mathf.Abs(e.angle - angle);
            if (delta <= _angleRange)
            {
                if (_hold == e)
                {
                    continue;
                }
                else
                {
                    if (_hold is null)
                    {
                        _hold = e;
                        holdName = _hold.tool.gameObject.name;
                        Debug.LogFormat("Find Item: {0}", holdName);
                    }
                    else
                    {
                        if (Mathf.Abs(_hold.angle - angle) > Mathf.Abs(e.angle - angle))
                        {
                            _hold = e;
                            holdName = _hold.tool.gameObject.name;
                            Debug.LogFormat("Find Item: {0}", holdName);
                        }
                    }
                }
            }
            else
            {
                if (_hold == e)
                {
                    _hold = null;
                    holdName = string.Empty;
                    Debug.LogFormat("Release Item: {0}", holdName);
                }
            }
        }
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        _direction.x = dir.x;
        _direction.y = 0;
        _direction.z = dir.y;
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
        
        if (_holding is null)
        {
            if (_hold is not null)
            {
                _hold.tool.transform.SetParent(transform);
                _hold.tool.transform.localPosition = new Vector3(0, 1, 0);
                _holding = _hold;
            }
        }
        else
        {
            _holding.tool.transform.parent = null;
            var pos = transform.TransformPoint(transform.forward);
            grid ??= FindObjectOfType<Grid>();
            var gridPos = grid.WorldToCell(pos);
            _holding.tool.transform.position = gridPos + new Vector3(0, 1.13f);
            
            _holding = null;
        }
        
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagTool))
        {
            var ctrl = other.GetComponent<ToolController>();
            
            
            inRangeTools.Add(new InRangeTool(ctrl, CaculateVector(ctrl.transform.position)));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagTool))
        {
            var ctrl = other.GetComponent<ToolController>();
            
            
            inRangeTools.RemoveAll(x => x.tool == ctrl);
        }
    }

    private Vector2 CaculateVector(Vector3 targetPos)
    {
        var vector = (targetPos - transform.position).normalized;
        return new Vector2(vector.x, vector.z);
    }

    private void UpdateVector()
    {
        for (var i = 0; i < inRangeTools.Count; i++)
        {
            var e = inRangeTools[i];
            e.vector = CaculateVector(e.tool.transform.position);
            e.angle = Mathf.Atan2(e.vector.x, e.vector.y) * Mathf.Rad2Deg;
        }
    }
}
