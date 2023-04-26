using UnityEngine;

public abstract class Projectile : MonoBehaviour, IAbility
{
    public AudioClip flame, nuke;
    public Audio adio;
    [SerializeField] float MoveSpeed;
    [SerializeField] protected float cooldown;
    [SerializeField] Rigidbody rb;
    [SerializeField] protected LayerMask HitMask;

    [SerializeField]protected string ownerName;

    [SerializeField] Transform target;
    ILiving livingTarget;
    [SerializeField] bool fireball;
    [SerializeField] bool blueFire;
    float IAbility.CoolDown { get { return cooldown; }  set { cooldown = value; } }
    bool IAbility.NeedsAim { get => true; set { } }
    string IAbility.Name { get => gameObject.name; set { } }

    protected virtual void Awake()
    {
        adio = FindObjectOfType<Audio>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy || (livingTarget != null && !livingTarget.IsAlive))
        {
            target = null;
            return;
        }
        
        transform.forward = GetLookDirection(target.position);
        rb.velocity = transform.forward * MoveSpeed;
    }
    public void Activate(GameObject owner, RaycastHit hit)
    {
        var p = Instantiate(this, owner.transform.position, Quaternion.identity);
        p.ownerName = owner.name;
        if (hit.collider == null)
        {
            p.target = null;
            p.transform.LookAt(hit.point);
            //p.transform.forward = GetLookDirection(hit.point);
            p.rb.velocity = p.transform.forward * MoveSpeed;
        }
        else
        {
            p.target = hit.transform;
            p.livingTarget = p.target.GetComponent<ILiving>();
        }
       
    }

    protected virtual void OnCollision(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if(!fireball)
                damageable.Damage(ownerName, DamageType.Pulse);
            if(blueFire)
                damageable.Damage(ownerName, DamageType.Pulse);
        }
    }
    public float GetCooldown()
    {
        return cooldown;
    }
    void OnTriggerEnter(Collider collision) {
        GameObject hitObject = collision.gameObject;
        
        if ((HitMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            adio.sound(flame);
            OnCollision(collision);
            Destroy(gameObject);
        }
    }
    Vector3 GetLookDirection(Vector3 lookAtPoint)
    {
        Vector3 lookAtVector = lookAtPoint - transform.position;
        lookAtVector.y = 0;
        lookAtVector.Normalize();
        return lookAtVector;
    }
}
