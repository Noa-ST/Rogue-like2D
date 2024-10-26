using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    // Thêm kiểu `Drops` vào danh sách
    public List<Drops> drops;
    bool isQuitting = false;

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (isQuitting) return;

        float randomNumber = Random.Range(0f, 100f);
        // Thêm kiểu `Drops` vào danh sách
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
            Instantiate(selectedDrop.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
