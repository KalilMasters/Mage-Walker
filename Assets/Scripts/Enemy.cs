using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IFreezable
{
    CharacterController player;
    [SerializeField] FloatContainer MovementSpeed;
    public bool isFrozen = false;
    Animator animator;
    bool isDead = false;
    Collider _collider;
    [field: SerializeField] public float YOffset { get; private set; }
    public virtual void Damage(string owner, DamageType type)
    {
        ScoreSystem.Instance.AddPoints(5);
        isDead = true;
        animator.SetTrigger("Die");
        animator.SetBool("Run Forward", false);
        _collider.enabled = false;
    }

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        if (animator)
            animator.SetBool("Run Forward", true);
        _collider = GetComponentInChildren<Collider>();
    }
    private void OnEnable()
    {
        MapManager.Instance.RegisterEnemy(this);
    }
    private void OnDisable()
    {
        MapManager.Instance.UnRegisterEnemy(this);

    }
    private void Update()
    {
        if (player == null) return;
        if (isFrozen) return;
        if (isDead) return;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, MovementSpeed.Value * Time.deltaTime);
        Vector3 lookAtPosition = player.transform.position;
        lookAtPosition.y = transform.position.y - YOffset;
        transform.LookAt(lookAtPosition);
    }

    public void Freeze()
    {
        isFrozen = true;
        animator.SetBool("Run Forward", false);
    }

    public void UnFreeze()
    {
        isFrozen = false;
        animator.SetBool("Run Forward", true);
    }
    public void Death()
    {
        GameObject.Destroy(gameObject);
    }
}
