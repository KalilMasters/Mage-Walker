using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningProjectile : Projectile
{
    [SerializeField] GameObject chainLightning;


    protected override void OnCollision(Collider collision)
    {
        base.OnCollision(collision);
        Instantiate(chainLightning, collision.transform.position, Quaternion.identity);
    }
}
