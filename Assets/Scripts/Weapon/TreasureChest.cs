using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInventory p = collision.GetComponent<PlayerInventory>();
        if (p)
        {
            bool randomBool = Random.Range(0, 2) == 0;

            OpenTreasureChest(p, randomBool);
        }
    }

    private void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
    {
        foreach (PlayerInventory.Slot s in inventory.weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data.evolutionsData == null) continue;

            foreach (ItemData.Evolution e in w.data.evolutionsData)
            {
                if (e.condition == ItemData.Evolution.Condition.treasureChest)
                {
                    bool attempt = w.AttemptEvolution(e, 0);
                    if (attempt) return;
                }
            }
        }
    }
}
