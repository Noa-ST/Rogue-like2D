using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour,IColectible
{
    public int healthToRestore;
    public void Collect()
    {
        PlayerStat player = FindObjectOfType<PlayerStat>();
        player.Restore(healthToRestore);
        Destroy(gameObject);
    }
}
