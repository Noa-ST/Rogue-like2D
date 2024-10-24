using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Transform _player;
    public float moveSpeed;

    private void Start()
    {
        _player = FindObjectOfType<PlayerMovement>().transform;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, moveSpeed*Time.deltaTime);
    }
}
