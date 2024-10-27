using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStat _player;
    CircleCollider2D _playerCollector;
    public float pullSpeed;

    private void Start()
    {
        _player = FindObjectOfType<PlayerStat>();
        _playerCollector = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        _playerCollector.radius = _player.currentMagnet;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IColectible colectible))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(forceDirection * pullSpeed);
            colectible.Collect();
        }
    }
}
