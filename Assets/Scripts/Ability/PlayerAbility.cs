using Unity.VisualScripting;
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
        if (AbilityCooldowns[index].GetUsed())
        {
            print("Ability: " + index + " cannot be used");
            return;
        }
        selectedAbility = AbilityCooldowns[index];
        print("selected " + selectedAbility.projectile.name);
    }
    public void UseAbility(RaycastHit hit)
    {
        if (selectedAbility == null) return;
        if (selectedAbility.GetUsed()) return;
        FireProjectile();
        selectedAbility = null;
        void FireProjectile()
        {
            Transform hitObject = hit.collider.gameObject.transform;
            if (hitObject == null) return;
            Transform ability = Instantiate(selectedAbility.projectile, transform.position, Quaternion.identity).transform;
            ability.LookAt(hitObject);
            Destroy(ability.gameObject, 10f);
            selectedAbility.SetUsed(true);
        }
    }
}