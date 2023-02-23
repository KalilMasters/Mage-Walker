using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IAbility
{
    public void Activate(Transform player, RaycastHit hit);
    public float CoolDown();
    public bool NeedsAim();
}
