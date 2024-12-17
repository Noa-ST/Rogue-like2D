using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount;

    protected override void Update()
    {
        base.Update();

        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefabs has not been set for {0}", name));
            ActivateCooldown(true);
            return false;
        }

        if (!CanAttack()) return false;

        float spawnAngle = GetSpawnAngle();

        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, Owner.transform), 5f);
        }

        Projectile prefabs = Instantiate(currentStats.projectilePrefab, Owner.transform.position + (Vector3)GetSpawnOffSet(spawnAngle), Quaternion.Euler(0, 0, spawnAngle));

        prefabs.weapon = this;
        prefabs.owner = Owner;

        ActivateCooldown(true);

        attackCount--;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }

    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMoveVector.y, movement.lastMoveVector.x) * Mathf.Rad2Deg;
    }

    protected virtual Vector2 GetSpawnOffSet(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.yMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
            );
    }

    public override void OnEquip()
    {
        base.OnEquip();

        if (data == null)
        {
            Debug.LogWarning("No WeaponData assigned to this weapon.");
            return;
        }

        // Gán các chỉ số cơ bản từ WeaponData
        currentStats = data.baseStats;

    }
}
