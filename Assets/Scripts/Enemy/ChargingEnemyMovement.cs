using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyMovement : EnemyMovement
{
    Vector2 _chargeDirection;

    protected override void Start()
    {
        base.Start();
        _chargeDirection = (player.transform.position - transform.position).normalized;
    }

    public override void Move()
    {
        transform.position += (Vector3)_chargeDirection * enemy.Actual.moveSpeed * Time.deltaTime;
    }
}
