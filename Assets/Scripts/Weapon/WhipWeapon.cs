using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : ProjectileWeapon
{
    int _currentSpawnCount;
    float _currentSpawnYOffset;

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            currentCooldown = data.baseStats.cooldown;
            return false;
        }

        if (!CanAttack()) return false;

        if (currentCooldown <= 0)
        {
            _currentSpawnCount = 0;
            _currentSpawnYOffset = 0f;
        }

        float spawnDir = Mathf.Sign(movement.lastMoveVector.x) * (_currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2(spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax), _currentSpawnYOffset);

        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)spawnOffset, Quaternion.identity);

        prefab.owner = owner;

        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(-Mathf.Abs(prefab.transform.localScale.x), prefab.transform.localScale.y, prefab.transform.localScale.z);

            Debug.Log(spawnDir + " | " + prefab.transform.localScale);
        }

        prefab.weapon = this;
        currentCooldown = data.baseStats.cooldown;
        attackCount--;

        _currentSpawnCount++;
        if (_currentSpawnCount > 1 && _currentSpawnCount % 2 == 0)
            _currentSpawnYOffset += 1;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }
}
