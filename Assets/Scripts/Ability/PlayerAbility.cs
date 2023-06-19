using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] PlayerAnimator PlayerAnim;
    CharacterController charContoller;
    [SerializeField] CooldownManager[] AbilityCooldowns;
    CooldownManager selectedAbility = null;
    private void Awake()
    {
        charContoller = GetComponent<CharacterController>();
    }
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
    public void Ability1Ctx(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>().Equals(1))
            SetAbility(0);
    }
    public void Ability2Ctx(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>().Equals(1))
            SetAbility(1);
    }
    public void SetAbility(int index)
    {
        if (!charContoller.IsAlive) return;
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
            AbilityCooldowns[index].AbilityComponent.Cancel();
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
        if (!charContoller.IsAlive) return;
        if (selectedAbility == null) selectedAbility = AbilityCooldowns[2];
        if (selectedAbility.GetUsed())
        {
            selectedAbility.AbilityComponent.Cancel();
            return;
        }

        AudioManager.instance.PlaySound(selectedAbility.AbilityComponent.UseSFX);
        selectedAbility.AbilityComponent.Activate(gameObject, hit);
        selectedAbility.SetUsed(true);
        selectedAbility = null;
        PlayerAnim.ActivateTrigger("Attack");
        PlayerAnim.LookAtTarget(hit);
    }
}