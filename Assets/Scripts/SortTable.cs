using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class SortTable : MonoBehaviour
{
    SpriteRenderer sorted;
    public bool sortingActive = true;
    public float minimumDistance = 0.2f;
    int lastSortOrder = 0;

    protected virtual void Start()
    {
        sorted = GetComponent<SpriteRenderer>();
    }

    protected virtual void LateUpdate()
    {

        int newSortOrder = (int)(-transform.position.y / minimumDistance);

        if (lastSortOrder != newSortOrder) sorted.sortingOrder = newSortOrder;
    }
    public override void Move()
    {
        if (player == null || rb == null)
        {
            Debug.LogWarning("Enemy " + gameObject.name + " cannot move: player=" + (player != null) + ", rb=" + (rb != null));
            return;
        }

        Vector2 target = player.position;
        Vector2 current = transform.position;
        Vector2 direction = (target - current);
        float distance = direction.magnitude;

        // Nếu quá xa, không đuổi theo nữa
        if (distance > stopDistance) return;

        direction.Normalize();

        // Nếu khoảng cách nhỏ hơn safeDistance => lùi lại
        if (distance < safeDistance)
        {
            direction = -direction;
        }

        FlipSprite(direction);
        float moveDistance = enemy.Actual.moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + direction * moveDistance);
    }
}
