using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinachPassiveItem : PassiveItems
{
    protected override void ApplyModifier()
    {
        player.CurrentMight *= 1 + passiveitemData.Multipler / 100f;
    }
}
