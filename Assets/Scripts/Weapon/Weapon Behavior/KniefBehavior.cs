using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KniefBehavior : ProjectileWeaponsBehavior
{
    protected override void Start()
    {
        base.Start();

    }

    private void Update()
    {
        transform.position += direction * weaponData.Speed * Time.deltaTime;
    }
}
