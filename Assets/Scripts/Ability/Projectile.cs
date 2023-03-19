using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class Projectile : MonoBehaviour, IAbility
{
    public AudioClip flame;
    Audio adio;
    [SerializeField] float MoveSpeed;
    [SerializeField] protected float cooldown;
    [SerializeField] Rigidbody rb;
    [SerializeField] protected LayerMask HitMask;

    [SerializeField]protected string ownerName;

    float IAbility.CoolDown { get { return cooldown; }  set { cooldown = value; } }
    bool IAbility.NeedsAim { get => true; set { } }
    string IAbility.Name { get => gameObject.name; set { } }

    // Start is called before the first frame update
    protected void Start()
    {
        adio = FindObjectOfType<Audio>();
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * MoveSpeed;
    }

    public void Activate(GameObject owner, RaycastHit hit)
    {
        Instantiate(gameObject, owner.transform.position, Quaternion.identity).transform.LookAt(hit.transform);
        this.ownerName = owner.name;
    }

    protected virtual void OnCollision(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            damageable.Damage(ownerName, DamageType.Pulse);
    }
    public float GetCooldown()
    {
        return cooldown;
    }
    void OnTriggerEnter(Collider collision) {
        GameObject hitObject = collision.gameObject;
        float dot = Vector3.Dot(transform.forward, hitObject.transform.position - transform.position);
        if (dot < 0) return;
        if ((HitMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            adio.sound(flame);
            OnCollision(collision);
            Destroy(gameObject);
        }
    }
}
