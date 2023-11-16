using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] GameObject[] mageStaffs; // All of the mage's staff, 0 = Fireball, 1 = Blue Fire
    [SerializeField] CooldownManager primaryAbilityEquipSlot; // Player's usable primary ability (only one)

    [SerializeField] CooldownManager[] specialAbilityEquipSlots; // Player's usable special abilities
    [SerializeField] RawImage[] specialAbilityIconSlots; // The icons that the player sees on the buttons

    [SerializeField] PlayerSettings settings;


    void Start()
    {
        settings.Load();
        HandlePrimaryAbility();
        HandleSpecialAbilities();
    }

    void HandlePrimaryAbility()
    {
        foreach(GameObject x in mageStaffs)
            x.SetActive(false);

        mageStaffs[settings.PrimaryAbility].SetActive(true);
        primaryAbilityEquipSlot.Init(settings.PrimaryAbilityStorage[settings.PrimaryAbility]);
    }
    void HandleSpecialAbilities()
    {
        for(int i = 0; i < specialAbilityEquipSlots.Length; i++)
            specialAbilityEquipSlots[i].Init(settings.SpecialAbilityStorage[settings.SpecialAbilities[i]]);
    }
}
