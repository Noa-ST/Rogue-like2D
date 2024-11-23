using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    InventoryManager _inventory;

    private void Start()
    {
        _inventory = FindAnyObjectByType<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OpenTreasureChest();
            Destroy(gameObject);
        }
    }

    private void OpenTreasureChest()
    {
        if (_inventory.GetPossibleEvolutions().Count <= 0)
        {
            Debug.LogWarning("No Available Evolutions");
            return;
        }

        WeaponEvolutionBluePrint toEvole = _inventory.GetPossibleEvolutions()[Random.Range(0, _inventory.GetPossibleEvolutions().Count)];
        _inventory.EvolveWeapon(toEvole);
    }
}
