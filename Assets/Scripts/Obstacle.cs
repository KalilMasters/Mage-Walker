using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IDamageable
{
    public bool Breakable;
    public ParticleSystem BreakEffect;
    public bool Damage(string owner, DamageType type)
    {
        if (!Breakable) return false;
        if(!owner.Equals("Fell Off"))
        //if(!type.Equals(DamageType.InstantDeath))
        {
            var main = Instantiate(BreakEffect, transform.position, Quaternion.identity).main;
            main.startColor = GetComponent<MeshRenderer>().material.color;
        }
        
        GameObject.Destroy(gameObject);
        return true;
    }
}
