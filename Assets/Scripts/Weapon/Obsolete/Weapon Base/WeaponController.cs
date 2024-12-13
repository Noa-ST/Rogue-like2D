using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;
    float _curCooldown;

    protected PlayerMovement pm;

    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        _curCooldown = weaponData.CooldownDuration;

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
        _curCooldown = weaponData.CooldownDuration;
    }
}
