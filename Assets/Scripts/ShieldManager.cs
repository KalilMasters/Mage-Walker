using TMPro;
using UnityEngine;
public class ShieldManager : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int MaxHitPoints { get; private set; }
    [field: SerializeField] public int HitPoints { get; private set; }
    [field: SerializeField] public bool IsInvincible { get; private set; } = false;
    public bool IsBroken => HitPoints <= 0;

    [SerializeField] private float InvincibilityTime;
    [SerializeField] private float counter = 0;
    [SerializeField] TMP_Text ShieldText; // Debug for now

    public event System.Action<string> OnShieldBroken, OnShieldDamageTaken, OnRealDamageTaken;
    private void Awake()
    {
        counter = InvincibilityTime;
        UpdateShieldVisual();
    }
    public void SetMaxHitPoints(int max)
    {
        MaxHitPoints = max;

        SetHitPoints(HitPoints);
        UpdateShieldVisual();
    }
    public void SetToMax() => SetHitPoints(MaxHitPoints);
    public void SetHitPoints(int val) => HitPoints = Mathf.Min(val, MaxHitPoints);

    private void Update() => ManageInvincibilityTime();
    void ManageInvincibilityTime()
    {
        if (!IsInvincible)
            return;

        counter = Mathf.Max(0, counter - Time.deltaTime);

        if (counter > 0) return;

        IsInvincible = false;
        counter = InvincibilityTime;
    }

    public bool Damage(string owner, DamageType type)
    {
        if (owner == null || owner.Equals(gameObject.name)) return false;
        if (IsInvincible) return false;

        if (IsBroken || type.Equals(DamageType.InstantDeath))
        {
            SetHitPoints(0);
            OnRealDamageTaken?.Invoke(owner);
            return true;
        }


        HitPoints--;
        OnShieldDamageTaken?.Invoke(owner);
        if (IsBroken)
            OnShieldBroken?.Invoke(owner);


        UpdateShieldVisual();
        IsInvincible = true;
        return false;
    }

    void UpdateShieldVisual() => ShieldText?.SetText("Shield: " + HitPoints.ToString());
}
