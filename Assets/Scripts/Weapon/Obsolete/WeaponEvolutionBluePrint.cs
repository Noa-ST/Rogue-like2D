using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponEvolutionBluePrint", menuName = "ScriptableObjects/WeaponEvolutionBluePrint")]
public class WeaponEvolutionBluePrint : ScriptableObject
{
    public WeaponScriptableObject baseWeaponData;
    public PassiveItemsScriptableobject catalystPasiveItemData;
    public WeaponScriptableObject evolvedWeaponData;
    public GameObject evolveWeapon;
}
