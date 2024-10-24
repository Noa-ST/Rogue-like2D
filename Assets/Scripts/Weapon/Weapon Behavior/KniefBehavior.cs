using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KniefBehavior : ProjectileWeaponsBehavior
{
    KnifeController _kc;

    protected override void Start()
    {
        base.Start();
        _kc = FindObjectOfType<KnifeController>();
    }

    private void Update()
    {
        transform.position += direction * _kc.speed * Time.deltaTime;
    }
}
