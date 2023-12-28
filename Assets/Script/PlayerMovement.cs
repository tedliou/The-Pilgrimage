using JetBrains.Rider.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    private Vector3 movement;
    private Animator anim;
    private Rigidbody playerRig;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerRig = GetComponent<Rigidbody>();
    }

    void Move(float h,float v)
    {
        movement.Set(h, 0f, v);
        movement = movement.normalized*moveSpeed*Time.deltaTime;
        playerRig.MovePosition(transform.position + movement);
    }
    void Animating (float h,float v)
    {
        bool walking =h!=0 || v!=0;
        anim.SetBool("isWalking", walking);
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Move(h, v);
        Animating(h, v);
    }


    void Update()
    {
        
    }
}
