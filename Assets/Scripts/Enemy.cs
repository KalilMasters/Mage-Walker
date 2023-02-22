using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    CharacterController player;
    [SerializeField] FloatContainer MovementSpeed;

    public void DoDamage(float damage)
    {
        ScoreSystem.Instance.AddPoints(5);
        GameObject.Destroy(gameObject);
    }

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>();
    }
    private void Update()
    {
        if (player == null) return;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, MovementSpeed.Value * Time.deltaTime);
    }
}
