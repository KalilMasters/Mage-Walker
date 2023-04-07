using UnityEngine;

public abstract class Projectile : MonoBehaviour, IAbility
{
    public AudioClip flame;
    Audio adio;
    [SerializeField] float MoveSpeed;
    [SerializeField] protected float cooldown;
    [SerializeField] Rigidbody rb;
    [SerializeField] protected LayerMask HitMask;

    [SerializeField]protected string ownerName;

    [SerializeField] Transform target;
    ILiving livingTarget;

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
        Vector3 lookAtVector = target.position - transform.position;
        lookAtVector.y = 0;
        lookAtVector.Normalize();
        transform.forward = lookAtVector;
        rb.velocity = lookAtVector * MoveSpeed;
    }
    public void Activate(GameObject owner, RaycastHit hit)
    {
        var p = Instantiate(this, owner.transform.position, Quaternion.identity);
        p.ownerName = owner.name;
        p.target = hit.transform;
        p.livingTarget = p.target.GetComponent<ILiving>();
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
        
        if ((HitMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            adio.sound(flame);
            OnCollision(collision);
            Destroy(gameObject);
        }
    }
}
