using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningProjectile : Projectile
{
    [SerializeField] GameObject chainLightning;
    [SerializeField] GameObject lightningEffect;
    public AudioClip light;
    protected override void OnCollision(Collider collision)
    {
        base.OnCollision(collision);
        AudioManager.instance.PlaySound(light);
        Instantiate(lightningEffect, collision.transform.position, Quaternion.identity);
        Instantiate(chainLightning, collision.transform.position, Quaternion.identity);
    }
}
