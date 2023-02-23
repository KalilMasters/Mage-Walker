using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Projectile, IAbility
{
    public void Activate(Transform player, RaycastHit hit)
    {
        Instantiate(gameObject, player.position, Quaternion.identity).transform.LookAt(hit.transform);
    }

    public float CoolDown() => cooldown;

    protected override void DoStuff(Collider collision)
    {
        base.DoStuff(collision);
    }
    public bool NeedsAim() => true;
}
