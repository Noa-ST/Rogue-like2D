using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    Rigidbody2D _rb;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        InputManagerment();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
        }
    }

    private void InputManagerment()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
    }


}
