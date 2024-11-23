using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceGem : Pickup
{
    public int experienceGranted;
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

        Debug.Log("Called");
        PlayerStat player = FindObjectOfType<PlayerStat>();
        player.IncreaseExperience(experienceGranted);
    }
}
