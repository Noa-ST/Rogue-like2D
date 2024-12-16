using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    PlayerStat _player;
    CircleCollider2D _detector;
    public float pullSpeed;

    private void Start()
    {
        _player = GetComponentInParent<PlayerStat>();
    }

    public void SetRadius(float r)
    {
        if (!_detector) _detector = GetComponent<CircleCollider2D>();
        _detector.radius = r;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Pickup p))
        {
            p.Collect(_player, pullSpeed);
        }
    }
}
