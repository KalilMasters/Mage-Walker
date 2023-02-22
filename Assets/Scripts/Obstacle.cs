using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IDamageable
{
    public bool Breakable;
    public void DoDamage(float damage)
    {
        if (!Breakable) return;
        GameObject.Destroy(gameObject);
    }
}
