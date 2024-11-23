using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Pickup
{
    public int healthToRestore;
    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }

        PlayerStat player = FindObjectOfType<PlayerStat>();
        player.Restore(healthToRestore);
        Destroy(gameObject);
    }
}
