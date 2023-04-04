using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFreezable
{
    public bool IsFrozen { get; }
    public void Freeze();
    public void UnFreeze();
}
