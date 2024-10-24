using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponsBehavior : MonoBehaviour
{
    protected Vector3 direction;
    public float destroyAfterSeconds;

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rocation = transform.rotation.eulerAngles;

        if (dirx < 0 && diry == 0) //left
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry < 0) //down
        {
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry > 0) //up
        {
            scale.x = scale.x * -1;
        }
        else if (dir.x > 0 && dir.y > 0) //right up
        {
            rocation.z = 0f;
        }
        else if (dir.x > 0 && dir.y < 0) //right down
        {
            rocation.z = -90f;
        }
        else if (dir.x < 0 && dir.y > 0) //left up
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rocation.z = -90f;
        }
        else if (dir.x < 0 && dir.y < 0) //left down
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rocation.z = 0f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rocation);
    }
}
