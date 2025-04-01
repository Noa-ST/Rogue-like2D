using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : SortTable
{
    public float lifespan = 0.5f;
    protected PlayerStat targer;
    protected float speed;
    Vector2 _initialPostion;
    float _intitialOffset;

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

    protected override void Start()
    {
        base.Start();
        _initialPostion = transform.position;
        _intitialOffset = Random.Range(0, bobbingAnimation.frequency);
    }

    protected virtual void Update()
    {
        if (targer)
        {
            Vector2 distance = targer.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                Destroy(gameObject);
        }
        else
        {
            transform.position = _initialPostion + bobbingAnimation.direction * Mathf.Sin(Time.time *+ _intitialOffset);
        }
    }

    public virtual bool Collect(PlayerStat target, float speed, float lifespan = 0f)
    {
        if (!this.targer)
        {
            this.targer = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!targer) return;
        if (experience != 0) targer.IncreaseExperience(experience);
        if (health != 0) targer.RestoreHealth(health);
    }
}
