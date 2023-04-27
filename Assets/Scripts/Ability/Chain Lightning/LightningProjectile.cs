using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningProjectile : Projectile
{
    [SerializeField] GameObject chainLightning;
    [SerializeField] GameObject lightningEffect;
    Audio adio;
    public AudioClip light;
    protected override void OnCollision(Collider collision)
    {
        adio = FindObjectOfType<Audio>();
        base.OnCollision(collision);
        adio.sound(light);
        Instantiate(lightningEffect, collision.transform.position, Quaternion.identity);
        Instantiate(chainLightning, collision.transform.position, Quaternion.identity);
    }
}
