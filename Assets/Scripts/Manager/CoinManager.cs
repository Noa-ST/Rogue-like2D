using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    public TMP_Text coinDisplay;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("CoinManager initialized. Current coins: " + Pref.Coins); 
        }
        else
        {
            Destroy(gameObject);
        }
        UpdateCoinDisplay();
    }

    void Start()
    {
        FindCoinDisplay(); // Tìm coinDisplay khi scene load
    }

    // Tìm coinDisplay trong scene hiện tại
    void FindCoinDisplay()
    {
        if (coinDisplay == null)
        {
            // Tìm theo tên GameObject cụ thể để chính xác hơn
            GameObject go = GameObject.Find("Coin Display Text");
            if (go != null) coinDisplay = go.GetComponent<TMP_Text>();
            
            if (coinDisplay == null) coinDisplay = GameObject.FindObjectOfType<TMP_Text>(true);
            if (coinDisplay == null)
            {
                Debug.LogWarning("No TMP_Text found for coin display!");
            }
            else
            {
                Debug.Log("Found coin display: " + coinDisplay.name);
            }
        }
    }

    public void UpdateCoinDisplay()
    {
        if (coinDisplay != null)
        {
            coinDisplay.text = Pref.Coins.ToString();
        }
        else
        {
            Debug.LogWarning("coinDisplay is null, cannot update!");
            FindCoinDisplay();
        }
    }

    // Phương thức để tăng coin khi nhặt
    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            Pref.Coins += amount;
            UpdateCoinDisplay();
            Debug.Log("Added " + amount + " coins. Total saved coins: " + Pref.Coins);
        }
    }
}