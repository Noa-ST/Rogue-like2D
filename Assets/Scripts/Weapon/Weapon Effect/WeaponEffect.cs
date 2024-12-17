using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector] public PlayerStat owner;
    [HideInInspector] public Weapon weapon;

    public PlayerStat Owner;

    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}


