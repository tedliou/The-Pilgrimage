using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    private float _moveSpeed;
    private Vector3 _rotation;
    private float _rotateSpeed;
    private float _angle;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _direction = Vector3.zero;
        _moveSpeed = .1f;
        _rotation = Vector3.zero;
        _rotateSpeed = .1f;
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
        }

        if (_rotation.x != 0 || _rotation.y != 0)
        {
            _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, _angle, 0)));
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
        Debug.Log(ctx.ReadValue<Vector2>());
        var rot = ctx.ReadValue<Vector2>();
        _rotation = rot;
        _angle = Mathf.Atan2(rot.x, rot.y) * Mathf.Rad2Deg;
        Debug.Log(_angle);
        
    }
}
