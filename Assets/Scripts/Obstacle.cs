using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IDamageable
{
    public bool Breakable;
    public ParticleSystem BreakEffect;
    public void DoDamage(float damage)
    {
        if (!Breakable) return;
        var main = Instantiate(BreakEffect, transform.position, Quaternion.identity).main;
        main.startColor = GetComponent<MeshRenderer>().material.color;
        GameObject.Destroy(gameObject);
    }
}
