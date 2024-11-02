using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponsBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    protected Vector3 direction;
    public float destroyAfterSeconds;
    //Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
    }

    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStat>().currentMight;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    // Phương thức nhận hướng di chuyển của đạn và điều chỉnh hướng bắn
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;   

        float dirx = direction.x;  
        float diry = direction.y;   

        Vector3 scale = transform.localScale;        // Lấy scale của đạn để điều chỉnh hướng
        Vector3 rocation = transform.rotation.eulerAngles; // Lấy góc xoay hiện tại

        // Xác định hướng và điều chỉnh hướng di chuyển, scale, và góc xoay của đạn theo hướng đã cho
        if (dirx < 0 && diry == 0)       // Nếu đi sang trái
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry < 0)  // Nếu đi xuống
        {
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry > 0)  // Nếu đi lên
        {
            scale.x = scale.x * -1;
        }
        else if (dirx > 0 && diry > 0)   // Nếu đi chéo lên phải
        {
            rocation.z = 0f;
        }
        else if (dirx > 0 && diry < 0)   // Nếu đi chéo xuống phải
        {
            rocation.z = -90f;
        }
        else if (dirx < 0 && diry > 0)   // Nếu đi chéo lên trái
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rocation.z = -90f;
        }
        else if (dirx < 0 && diry < 0)   // Nếu đi chéo xuống trái
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rocation.z = 0f;
        }

        transform.localScale = scale;    // Cập nhật scale
        transform.rotation = Quaternion.Euler(rocation); // Cập nhật góc xoay
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyStat enemy = collision.GetComponent<EnemyStat>();
            enemy.TakeDamage(GetCurrentDamage());
            ReducePierce();
        }
        else if (collision.CompareTag("Prop"))
        {
            if (collision.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                ReducePierce();
            }
        }
    }


    private void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
