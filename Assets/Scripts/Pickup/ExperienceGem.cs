using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceGem : Pickup, IColectible
{
    public int experienceGranted;
    public void Collect()
    {
        Debug.Log("Called");
        PlayerStat player = FindObjectOfType<PlayerStat>();
        player.IncreaseExperience(experienceGranted);
    }
}
