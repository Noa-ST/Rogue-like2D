using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Transform _player;
    EnemyStat _enemy;

    Vector2 _knockbackVelocity;
    float _knockbackDuration;

    private void Start()
    {
        _enemy = GetComponent<EnemyStat>();
        _player = FindObjectOfType<PlayerMovement>().transform;
    }

    private void Update()
    {
        if (_knockbackDuration > 0)
        {
            transform.position += (Vector3)_knockbackVelocity * Time.deltaTime;
            _knockbackDuration -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _enemy.currentMoveSpeed * Time.deltaTime);
        }
    }

    public void KnockBack(Vector2 velocity, float duration)
    {
        if (_knockbackDuration > 0) return;

        _knockbackVelocity = velocity;
        _knockbackDuration = duration;
    }
}
