using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    CharacterController player;
    [SerializeField] FloatContainer MovementSpeed;
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
