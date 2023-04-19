using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] GameObject[] mageStaffs; // All of the mage's staff, 0 = Fireball, 1 = Blue Fire
    [SerializeField] GameObject[] primaryAbilityPrefabs; // Library of all our primary abilities
    [SerializeField] CooldownManager primaryAbilityEquipSlot; // Player's usable primary ability (only one)

    [SerializeField] Texture[] specialAbilityIcons; // Library of all our icons
    [SerializeField] GameObject[] specialAbilityPrefabs; // Library of all our special abilities
    [SerializeField] CooldownManager[] specialAbilityEquipSlots; // Player's usable special abilities
    [SerializeField] RawImage[] specialAbilityIconSlots; // The icons that the player sees on the buttons


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitAbility(0.01f));
    }
    IEnumerator InitAbility(float delay)
    {
        yield return new WaitForSeconds(delay);
        HandlePrimaryAbility();
        HandleSpecialAbilities();
    }

    void HandlePrimaryAbility()
    {
        foreach(GameObject x in mageStaffs)
        {
            x.SetActive(false);
        }
        mageStaffs[SelectAbility.primaryAbility].SetActive(true);
        primaryAbilityEquipSlot.SetAbility(primaryAbilityPrefabs[SelectAbility.primaryAbility]);
    }
    void HandleSpecialAbilities()
    {
        for(int i = 0; i < specialAbilityEquipSlots.Length; i++)
        {
            specialAbilityIconSlots[i].texture = specialAbilityIcons[SelectAbility.specialAbilities[i]];
            specialAbilityEquipSlots[i].SetAbility(specialAbilityPrefabs[SelectAbility.specialAbilities[i]]);
        }
    }
}
