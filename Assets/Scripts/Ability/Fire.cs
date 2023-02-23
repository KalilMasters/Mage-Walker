using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Projectile, IAbility
{
    [SerializeField] GameObject Explosion;
    public void Activate(Transform player, RaycastHit hit)
    {
        print("Projectile: " + gameObject.name);
        Instantiate(gameObject, player.position, Quaternion.identity).transform.LookAt(hit.transform);
    }

    public float CoolDown() => cooldown;
    protected override void DoStuff(Collider collision)
    {
        print("Exploding");
        base.DoStuff(collision);
        if(Explosion != null)
        {
            Instantiate(Explosion, transform.position, Quaternion.identity);
            foreach (Collider c in Physics.OverlapSphere(transform.position, 2, HitMask))
                if (c.TryGetComponent(out IDamageable d))
                    d.DoDamage(GetDamage());
        }
    }
    public bool NeedsAim() => true;
    public string Name() => gameObject.name;
}
