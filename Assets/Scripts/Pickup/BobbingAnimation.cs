using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency;
    public float magitude;
    public Vector3 direction;
    Vector3 _initialPostion;
    Pickup _pickup;

    void Start()
    {
        _pickup = GetComponent<Pickup>();
        _initialPostion = transform.position;
    }

    void Update()
    {
        if (_pickup && !_pickup.hasBeenCollected)
        {
            transform.position = _initialPostion + direction * Mathf.Sin(Time.time * frequency) * magitude;
        }
    }
}
