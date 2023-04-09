using UnityEngine;

public interface IAbility
{
    public void Activate(GameObject owner, RaycastHit hit);
    public float CoolDown { get; protected set; }
    public bool NeedsAim { get; protected set; }
    public string Name { get; protected set; }
}
