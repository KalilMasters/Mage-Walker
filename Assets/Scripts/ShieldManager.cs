using TMPro;
using UnityEngine;
public class ShieldManager : MonoBehaviour, IDamageable
{
    //[SerializeField] bool HardcoreMode;

    [SerializeField] int ShieldHitPoints;
    bool _shieldBroken => ShieldHitPoints <= 0;

    [SerializeField] bool Invincible = false;
    [SerializeField] float InvincibilityTime;
    [SerializeField] float counter = 0;
    [SerializeField] TMP_Text ShieldText; // Debug for now
    [SerializeField] TMP_Text HardcoreModeText;

    public event System.Action<string> OnShieldBroken, OnShieldDamageTaken, OnRealDamageTaken;
    // Start is called before the first frame update
    void Start()
    {
        if (MapManager.isHardMode)
            ShieldHitPoints = 0;
        ShieldText.text = "Shield: " + ShieldHitPoints.ToString();
        HardcoreModeText.text = "HARDCORE:" + (MapManager.isHardMode ? "ON" : "OFF");
    }

    // Update is called once per frame
    void Update()
    {
        ManageInvincibilityTime();
    }
    void ManageInvincibilityTime()
    {
        if (!Invincible)
            return;

        if (counter < InvincibilityTime)
        {
            counter += Time.deltaTime;
        }
        else
        {
            counter = 0;
            Invincible = false;
        }
    }

    public void Damage(string owner, DamageType type)
    {
        if (owner == null || owner.Equals(gameObject.name)) return;
        if (_shieldBroken || type.Equals(DamageType.InstantDeath))
        {
            OnRealDamageTaken?.Invoke(owner);
            return;
        }

        if (Invincible) return;

        ShieldHitPoints--;
        OnShieldDamageTaken?.Invoke(owner);
        if (_shieldBroken)
            OnShieldBroken?.Invoke(owner);
        ShieldText.text = "Shield: " + ShieldHitPoints.ToString();
        Invincible = true;
    }
}
