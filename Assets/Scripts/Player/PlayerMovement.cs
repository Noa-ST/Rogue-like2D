using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public const float DEFAUlT_MOVESPEED = 5f;

    // Movement
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMoveVector;

    // Tham chiếu 
    Rigidbody2D _rb;
    PlayerStat _player;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GetComponent<PlayerStat>();
        lastMoveVector = new Vector2(1, 0f); // Khởi tạo hướng di chuyển cuối cùng ban đầu (hướng phải)
    }

    // Phương thức Update sẽ gọi mỗi khung hình để lấy input người chơi
    void Update()
    {
        InputManagerment();
    }

    // Phương thức FixedUpdate sẽ gọi theo chu kỳ cố định, phù hợp để xử lý di chuyển dựa trên vật lý
    private void FixedUpdate()
    {
        Move();
    }

    private void InputManagerment()
    {
        if (GameManager.Ins.isGameOver)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveY = Input.GetAxisRaw("Vertical");

        // Tạo vector hướng di chuyển hiện tại và chuẩn hóa nó
        moveDir = new Vector2(moveX, moveY).normalized;

        // Nếu người chơi có di chuyển ngang (moveDir.x khác 0)
        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x; // Lưu hướng ngang cuối cùng (1 nếu phải, -1 nếu trái)
            lastMoveVector = new Vector2(lastHorizontalVector, 0f); // Cập nhật `lastMoveVector` để lưu lại hướng đi ngang cuối
        }

        // Nếu người chơi có di chuyển dọc (moveDir.y khác 0)
        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y; // Lưu hướng dọc cuối cùng (1 nếu lên, -1 nếu xuống)
            lastMoveVector = new Vector2(0f, lastVerticalVector); // Cập nhật `lastMoveVector` để lưu lại hướng đi dọc cuối
        }

        // Nếu người chơi có di chuyển cả ngang và dọc cùng lúc
        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMoveVector = new Vector2(lastHorizontalVector, lastVerticalVector); // Cập nhật `lastMoveVector` lưu cả hướng ngang và dọc
        }
    }

    private void Move()
    {
        if (GameManager.Ins.isGameOver)
        {
            return;
        }

        _rb.velocity = moveDir * DEFAUlT_MOVESPEED *  _player.Stats.moveSpeed;
    }
}
