using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : SortTable
{
    public const float DEFAULT_MOVESPEED = 5f;

    // Movement variables
    [HideInInspector] public float lastHorizontalVector;
    [HideInInspector] public float lastVerticalVector;
    [HideInInspector] public Vector2 moveDir;
    [HideInInspector] public Vector2 lastMoveVector;

    // References
    private Rigidbody2D _rb;
    private PlayerStat _player;
    private Animator _anim;
    private SpriteRenderer _sr;

    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
        _player = GetComponent<PlayerStat>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        lastMoveVector = new Vector2(1, 0f); // Initial last move direction (right)
    }

    void Update()
    {
        InputManagement();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void InputManagement()
    {
        if (GameManager.Ins.isGameOver) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0) { lastHorizontalVector = moveDir.x; lastMoveVector = new Vector2(lastHorizontalVector, 0f); }
        if (moveDir.y != 0) { lastVerticalVector = moveDir.y; lastMoveVector = new Vector2(0f, lastVerticalVector); }
        if (moveDir.x != 0 && moveDir.y != 0) lastMoveVector = new Vector2(lastHorizontalVector, lastVerticalVector).normalized;
    }

    private void Move()
    {
        if (GameManager.Ins.isGameOver) return;
        float currentMoveSpeed = DEFAULT_MOVESPEED * (_player != null ? _player.Stats.moveSpeed : 1f);
        _rb.velocity = moveDir * currentMoveSpeed;
    }

    private void UpdateAnimation()
    {
        if (_anim == null || _player == null) return;

        bool isMoving = moveDir.magnitude > 0;
        _anim.SetBool("Move", isMoving);

        if (isMoving)
        {
            SpriteDirectionChecker();
        }
    }

    private void SpriteDirectionChecker()
    {
        _sr.flipX = lastHorizontalVector < 0;
    }
}
