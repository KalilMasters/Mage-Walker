using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Damage(string owner, DamageType type);
}

public enum DamageType { Pulse, InstantDeath }