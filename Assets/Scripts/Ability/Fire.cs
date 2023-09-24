using UnityEngine;

public class Fire : Projectile
{
    [SerializeField] GameObject Explosion;
    [SerializeField] float _explosionRadius;
    protected override void OnCollision(Collider collision)
    {
        base.OnCollision(collision);
        if(Explosion != null)
        {
            Instantiate(Explosion, transform.position, Quaternion.identity);
            foreach (Collider c in Physics.OverlapSphere(transform.position, _explosionRadius, HitMask))
                if (c.TryGetComponent(out IDamageable d))
                    d.Damage(ownerName, DamageType.Pulse);
        }
    }
}
