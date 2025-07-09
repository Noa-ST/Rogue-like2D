using UnityEngine;

public class Pickup : SortTable
{
    public float lifespan = 0.5f;
    protected PlayerStat target; 
    protected float speed;
    Vector2 _initialPosition; 
    float _initialOffset;

    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }

    public BobbingAnimation bobbingAnimation = new BobbingAnimation
    {
        frequency = 2f,
        direction = new Vector2(0, 0.3f)
    };

    [Header("Bonuses")]
    public int experience;
    public int health;
    public int coins;

    // Biến tĩnh để theo dõi tổng coin đã nhặt
    private static int totalCoinsCollected = 0; 

    protected override void Start()
    {
        base.Start();
        _initialPosition = transform.position;
        _initialOffset = Random.Range(0, bobbingAnimation.frequency);
    }

    protected virtual void Update()
    {
        if (target)
        {
            Vector2 distance = (Vector2)target.transform.position - (Vector2)transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                Destroy(gameObject);
        }
        else
        {
            transform.position = _initialPosition + bobbingAnimation.direction * Mathf.Sin(Time.time * bobbingAnimation.frequency + _initialOffset); // Sửa toán tử
        }
    }

    public virtual bool Collect(PlayerStat target, float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!target) return;
        if (experience != 0) target.IncreaseExperience(experience);
        if (health != 0) target.RestoreHealth(health);
        if (coins != 0)
        {
            totalCoinsCollected += coins;
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoins(coins);
            }
            else
            {
                Pref.Coins += coins;
                Debug.LogWarning("CoinManager not found, using Pref directly. Collected " + coins + " coins.");
            }
            Debug.Log("Collected " + coins + " coins. Total coins collected: " + totalCoinsCollected + ", Current coins: " + Pref.Coins);
        }
    }

    // Phương thức tĩnh để lấy số coin spawn dựa trên tổng coin đã nhặt
    public static int GetCoinSpawnAmount()
    {
        // Ví dụ: Cứ mỗi 10 coin nhặt được, tăng thêm 1 coin spawn
        int baseCoin = 1;
        int bonusCoin = totalCoinsCollected / 10; 
        return Mathf.Max(1, baseCoin + bonusCoin);
    }
}