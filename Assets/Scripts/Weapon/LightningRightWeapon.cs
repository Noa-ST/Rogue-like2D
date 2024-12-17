using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRightWeapon : ProjectileWeapon
{
    List<EnemyStat> allSelectedEnemies = new List<EnemyStat>(0);

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.hitEffect)
        {
            Debug.LogWarning(string.Format("Hit effect prefab has not been set for {0}", name));
            ActivateCooldown(true);
            return false;
        }

        if (!CanAttack()) return false;

        if (currentCooldown <= 0)
        {
            allSelectedEnemies = new List<EnemyStat>(FindObjectsOfType<EnemyStat>());
            ActivateCooldown(true);
            currentAttackCount = attackCount;
        }

        EnemyStat target = PickEnemy();
        if (target)
        {
            DamageArea(target.transform.position, GetArea(), GetDamage());

            Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);
        }

        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, Owner.transform), 5f);
        }

        if (attackCount > 0)
        {
            currentAttackCount = attackCount - 1;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;
    }

    EnemyStat PickEnemy()
    {
        EnemyStat target = null;

        while (!target && allSelectedEnemies.Count > 0)
        {
            int idx = Random.Range(0, allSelectedEnemies.Count);
            target = allSelectedEnemies[idx];

            if (!target)
            {
                allSelectedEnemies.RemoveAt(idx);
                continue;
            }

            Renderer r = target.GetComponent<Renderer>();
            if (!r || !r.isVisible)
            {
                allSelectedEnemies.Remove(target);
                target = null;
                continue;
            }
        }

        allSelectedEnemies.Remove(target);
        return target;
    }

    void DamageArea(Vector2 position, float radius, float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(position, radius);

        foreach (Collider2D t in targets)
        {
            EnemyStat es = t.GetComponent<EnemyStat>();

            if (es)
            {
                es.TakeDamage(damage, transform.position);
            }
        }
    }
}
