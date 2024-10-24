using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject prefab;
    public float damage;
    public float speed;
    public float cooldownDuration;
    float _curCooldown;
    public int pierce;

    protected PlayerMovement pm;

    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        _curCooldown = cooldownDuration;

    }

    protected virtual void Update()
    {
        _curCooldown -= Time.deltaTime;
        if (_curCooldown <= 0f)
        {
            Attack();
        }

    }

    protected virtual void Attack()
    {
        _curCooldown = cooldownDuration;
    }
}
