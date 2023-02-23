using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IFreezable
{
    CharacterController player;
    [SerializeField] FloatContainer MovementSpeed;
    public bool isFrozen = false;
    public void DoDamage(float damage)
    {
        ScoreSystem.Instance.AddPoints(5);
        GameObject.Destroy(gameObject);
    }

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>();
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
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, MovementSpeed.Value * Time.deltaTime);
    }

    public void Freeze() => isFrozen = true;

    public void UnFreeze() => isFrozen = false;
}
