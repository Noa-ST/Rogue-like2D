using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    public List<Drops> drops;
    bool isQuitting = false;

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (isQuitting || SceneManager.GetActiveScene().isLoaded == false || GameManager.Ins == null || GameManager.Ins.currentState != GameManager.GameState.Gameplay) return;

        float randomNumber = Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops rate in drops)
        {
            if (randomNumber <= rate.dropRate)
            {
                possibleDrops.Add(rate);
            }
        }

        if (possibleDrops.Count > 0)
        {
            Drops selectedDrop = possibleDrops[Random.Range(0, possibleDrops.Count)];
            GameObject spawnedPickup = Instantiate(selectedDrop.itemPrefab, transform.position, Quaternion.identity);

            GameManager.Ins.RegisterPickup(spawnedPickup);
        }
    }
}
