using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] PlayerAnimator PlayerAnim;
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
        print("Clicked ability: " + index);
        if (index != Mathf.Clamp(index, 0, AbilityCooldowns.Length - 1)) return;
        if(selectedAbility != null)
        {
            bool alreadySelected = selectedAbility == AbilityCooldowns[index];
            selectedAbility.SetBackgroundColor(Color.black);
            selectedAbility = null;
            if (alreadySelected)
                return;
        }
        if (AbilityCooldowns[index].GetUsed())
        {
            print("Ability: " + (index + 1) + " cannot be used");
            return;
        }
        selectedAbility = AbilityCooldowns[index];
        selectedAbility.SetBackgroundColor(Color.green);
        //selectedAbility.SetOutline(true);
        print("Selected ability: " + selectedAbility.AbilityComponent.Name);
        if (!selectedAbility.AbilityComponent.NeedsAim)
            UseAbility(new RaycastHit());
    }
    public void UseAbility(RaycastHit hit)
    {
        if (selectedAbility == null) selectedAbility = AbilityCooldowns[2];
        if (selectedAbility.GetUsed()) return;

        print("Using ability: " + selectedAbility.AbilityComponent.Name);
        selectedAbility.AbilityComponent.Activate(gameObject, hit);
        selectedAbility.SetUsed(true);
        //selectedAbility.SetOutline(false);
        selectedAbility = null;
        PlayerAnim.ActivateTrigger("Attack");
    }
}