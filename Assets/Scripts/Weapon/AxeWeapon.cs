using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : ProjectileWeapon
{
    protected override float GetSpawnAngle()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float directionToMouseX = mousePos.x - transform.position.x;
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 0;
        return 90f - Mathf.Sign(directionToMouseX) * (5 * offset);
    }

    protected override Vector2 GetSpawnOffSet(float spawnAngle = 0)
    {
        return new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));
    }
}
