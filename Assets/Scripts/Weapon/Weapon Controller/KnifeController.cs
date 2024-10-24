using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : WeaponController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnKnife = Instantiate(prefab);
        spawnKnife.transform.position = transform.position;
        spawnKnife.GetComponent<KniefBehavior>().DirectionChecker(pm.lastMoveVector);
    }

}
