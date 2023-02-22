using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float Damage;
    [SerializeField] float Cooldown;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask HitMask;
    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * MoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }
    protected virtual void DoStuff(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            damageable.DoDamage(Damage);
    }
    public float GetDamage()
    {
        return Damage;
    }
    public float GetCooldown()
    {
        return Cooldown;
    }
    void OnTriggerEnter(Collider collision) {
        if ((HitMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            print("Touched: " + collision.gameObject.name);
            DoStuff(collision);
            Destroy(gameObject);
        }
    }
}
