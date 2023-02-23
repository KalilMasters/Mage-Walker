using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Projectile, IAbility
{
    [SerializeField] GameObject Explosion;
    public void Activate(Transform player, RaycastHit hit)
    {
        Instantiate(gameObject, player.position, Quaternion.identity).transform.LookAt(hit.transform);
    }

    public float CoolDown() => cooldown;
    protected override void DoStuff(Collider collision)
    {
        base.DoStuff(collision);
        Instantiate(Explosion, transform.position, Quaternion.identity);
        //Destroy(Explosion, 2f);
    }
    public bool NeedsAim() => true;
}
