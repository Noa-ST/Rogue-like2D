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
}
