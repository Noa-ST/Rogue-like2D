using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{
    Dictionary<EnemyStat, float> affectedTargets = new Dictionary<EnemyStat, float>();
    List<EnemyStat> targetsToUnaffect = new List<EnemyStat>();

    List<EnemyStat> cachedTargets = new List<EnemyStat>();

    private void Update()
    {
        if (affectedTargets.Count == 0) return;

        cachedTargets.Clear();
        cachedTargets.AddRange(affectedTargets.Keys);

        foreach (EnemyStat target in cachedTargets)
        {
            if (target == null)
            {
                affectedTargets.Remove(target);
                continue;
            }

            affectedTargets[target] -= Time.deltaTime;
            if (affectedTargets[target] <= 0)
            {
                if (targetsToUnaffect.Contains(target))
                {
                    affectedTargets.Remove(target);
                    targetsToUnaffect.Remove(target);
                }
                else
                {
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[target] = stats.cooldown * weapon.Owner.Stats.cooldown;
                    target.TakeDamage(GetDamage(), transform.position, stats.knockback);

                    weapon.ApplyBuff(target);

                    if (stats.hitEffect)
                    {
                        Destroy(Instantiate(stats.hitEffect, target.transform.position, Quaternion.identity), 5f);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyStat es))
        {
            if (!affectedTargets.ContainsKey(es))
            {
                affectedTargets.Add(es, 0);
            }
            else
            {
                if (targetsToUnaffect.Contains(es))
                {
                    targetsToUnaffect.Remove(es);
                }    
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyStat es))
        {
            if (affectedTargets.ContainsKey(es))
            {
                targetsToUnaffect.Add(es);
            }
        }
    }
}
