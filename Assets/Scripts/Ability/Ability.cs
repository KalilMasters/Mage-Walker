using UnityEngine;

public interface IAbility
{
    public void Activate(GameObject owner, RaycastHit hit);
    public bool Active { get; protected set; }
    public float CoolDown { get; protected set; }
    public bool NeedsAim { get; protected set; }
    public string Name { get; protected set; }

    public void Cancel();
}
