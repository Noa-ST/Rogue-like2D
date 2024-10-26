using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarlicBehavior : MeleWeaponBehavior
{
    List<GameObject> markedEnemies;

    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStat enemy = col.GetComponent<EnemyStat>();
            enemy.TakeDamage(currentDamage);

            markedEnemies.Add(col.gameObject);  //Mark the enemy
        }
    }
}
