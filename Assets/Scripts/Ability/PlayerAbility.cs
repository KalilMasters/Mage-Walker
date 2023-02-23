using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] CooldownManager[] AbilityCooldowns;
    CooldownManager selectedAbility = null;
    private void OnEnable()
    {
        TargetingSystem.OnTargetObject += UseAbility;
    }
    private void OnDisable()
    {
        TargetingSystem.OnTargetObject -= UseAbility;
    }
    void Update()
    {
        foreach (CooldownManager cooldown in AbilityCooldowns)
            cooldown.ManageCooldown();
    }
    public void SetAbility(int index)
    {
        if (index != Mathf.Clamp(index, 0, AbilityCooldowns.Length - 1)) return;
        if (AbilityCooldowns[index] == selectedAbility)
        {
            selectedAbility = null;
            return;
        }
        if (AbilityCooldowns[index].GetUsed())
        {
            print("Ability: " + (index + 1) + " cannot be used");
            return;
        }
        selectedAbility = AbilityCooldowns[index];
        print("Selected ability: " + selectedAbility.AbilityComponent.Name());
        if (!selectedAbility.AbilityComponent.NeedsAim())
            UseAbility(new RaycastHit());
    }
    public void UseAbility(RaycastHit hit)
    {
        if (selectedAbility == null) selectedAbility = AbilityCooldowns[2];
        if (selectedAbility.GetUsed()) return;
        print("Using ability: " + selectedAbility.AbilityComponent.Name());
        selectedAbility.AbilityComponent.Activate(transform, hit);
        selectedAbility.SetUsed(true);
        selectedAbility = null;
    }
}