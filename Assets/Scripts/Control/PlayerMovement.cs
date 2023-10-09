using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    private float _speed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _direction = Vector3.zero;
        _speed = .1f;
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
            _rigidbody.MovePosition(_rigidbody.position + _direction * _speed);
        }
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.ReadValue<Vector2>());
        var dir = ctx.ReadValue<Vector2>();
        _direction.x = dir.x;
        _direction.y = 0;
        _direction.z = dir.y;
    }
}
