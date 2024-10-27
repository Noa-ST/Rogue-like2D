using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Transform _player;
    EnemyStat _enemy;

    private void Start()
    {
        _enemy = GetComponent<EnemyStat>();
        _player = FindObjectOfType<PlayerMovement>().transform;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _enemy.currentMoveSpeed * Time.deltaTime);
    }
}
