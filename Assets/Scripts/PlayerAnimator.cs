using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator _amin;
    PlayerMovement _pm;
    SpriteRenderer _sr;

    private void Start()
    {
        _amin = GetComponent<Animator>();
        _pm = GetComponent<PlayerMovement>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(_pm.moveDir.x != 0 || _pm.moveDir.y != 0)
        {
            _amin.SetBool("Move", true);
            SpriteDirectionChecker();
        }
        else
        {
            _amin.SetBool("Move", false);
        }
    }

    private void SpriteDirectionChecker()
    {
        if (_pm.lastHorizontalVector < 0)
        {
            _sr.flipX = true;
        }
        else
        {
            _sr.flipX = false;
        }
    }
}
