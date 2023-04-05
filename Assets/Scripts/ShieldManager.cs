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
    [SerializeField] GameObject HardcoreModeText;

    public event System.Action<string> OnShieldBroken, OnShieldDamageTaken, OnRealDamageTaken;

    public void SetMaxHitPoints(int max)
    {
        MaxHitPoints = max;

        HitPoints = Mathf.Min(HitPoints, MaxHitPoints);
    }
    public void SetHitPoints(int val) => HitPoints = Mathf.Min(val, MaxHitPoints);

    public void SetToMax() => HitPoints = MaxHitPoints;
    public void SetBroken() => HitPoints = 0;
    void Awake()
    {
        if(HardcoreModeText != null)
            HardcoreModeText.SetActive(MapManager.IsHardMode);
        if(ShieldText != null)
            ShieldText.text = "Shield: " + HitPoints.ToString();
    }
    void Update() => ManageInvincibilityTime();
    void ManageInvincibilityTime()
    {
        if (!IsInvincible)
            return;

        if (counter < InvincibilityTime)
        {
            counter += Time.deltaTime;
        }
        else
        {
            counter = 0;
            IsInvincible = false;
        }
    }

    public bool Damage(string owner, DamageType type)
    {
        if (owner == null || owner.Equals(gameObject.name)) return false;
        if (IsBroken || type.Equals(DamageType.InstantDeath))
        {
            OnRealDamageTaken?.Invoke(owner);
            return true;
        }

        if (IsInvincible) return false;

        HitPoints--;
        OnShieldDamageTaken?.Invoke(owner);
        if (IsBroken)
            OnShieldBroken?.Invoke(owner);

        if(ShieldText)
            ShieldText.text = "Shield: " + HitPoints.ToString();
        IsInvincible = true;
        return false;
    }
}
