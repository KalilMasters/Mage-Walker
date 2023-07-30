using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAbility : MonoBehaviour
{
    //public AudioClip flame, blueflame, nuke, freeze, chainL;
    Color unselectedColor = new Color(0.1019608f, 0.1019608f, 0.1019608f);
    Color selectedColor = Color.green;
    Color specialUnselectedColor = Color.black;

    [SerializeField] Image[] primaryBackgrounds;

    [SerializeField] int selectedEquipSlot = -1;
    [SerializeField] int selectedSpecialAbility  = -1;
    [SerializeField] Image[] specialBackgrounds; // Stores the clickable buttons that represent the special abilities. It is for changing the background color to green to show that the player has selected a special ability
    [SerializeField] Texture[] specialAbilityIcons; // Library of all our special ability icons
    [SerializeField] Image[] equipSlotBackgrounds; // Stores the clickable buttons that represent the equip slot (1, 2). It is for changing the background color to green to show that the player has selected an equip slot
    [SerializeField] RawImage[] equipSlotIcon; // This is for changing the equip icon to match what the player chose to equip
    [SerializeField] PlayerSettings settings;
    // Start is called before the first frame update
    void Start()
    {
        settings.Load();
        ResetSpecialSelection();
        UpdateSelection();
    }
    public void SetPrimaryAbility(int choice)
    {
        bool alreadySelected = choice == settings.PrimaryAbility;

        settings.PrimaryAbility = choice;
        AudioManager.instance.PlaySound(
            settings.PrimaryAbilityStorage[settings.PrimaryAbility].AbilityComponent.UseSFX);

        UpdateSelection();
    }
    public void SetSpecialAbilitySlot(int choice) // Clicked on equip slots
    {
        selectedEquipSlot = choice == selectedEquipSlot? -1 : choice;

        UpdateSelection();
    }
    public void SetSpecialAbilitySpell(int choice) // Clicked on special ability
    {
        if(choice == selectedSpecialAbility)
        {
            specialBackgrounds[choice].color = specialUnselectedColor;
            selectedSpecialAbility = -1;
            return;
        }

        AudioManager.instance.PlaySound(
            settings.SpecialAbilityStorage[choice].AbilityComponent.UseSFX);


        selectedSpecialAbility = choice;

        UpdateSelection();
    }
    private void UpdateSelection()
    {
        if(selectedSpecialAbility != -1 && selectedEquipSlot != -1)
        {
            if (settings.SpecialAbilities[selectedEquipSlot] != selectedSpecialAbility)
            {
                if (settings.SpecialAbilities.Contains(selectedSpecialAbility))
                {
                    settings.SpecialAbilities[1 - selectedEquipSlot] = settings.SpecialAbilities[selectedEquipSlot];
                }
                settings.SpecialAbilities[selectedEquipSlot] = selectedSpecialAbility;
            }
            ResetSpecialSelection();
        }


        for (int i = 0; i < specialBackgrounds.Length; i++)
        {
            SetSelected(specialBackgrounds[i], i == selectedSpecialAbility);
        }
        for(int i = 0; i < equipSlotBackgrounds.Length; i++)
        {
            SetSelected(equipSlotBackgrounds[i], i == selectedEquipSlot);
        }
        for (int i = 0; i < primaryBackgrounds.Length; i++)
        {
            SetSelected(primaryBackgrounds[i], i == settings.PrimaryAbility);
        }

        equipSlotIcon[0].texture = specialAbilityIcons[settings.SpecialAbilities[0]];
        equipSlotIcon[1].texture = specialAbilityIcons[settings.SpecialAbilities[1]];

        void SetSelected(Image image, bool selected) => image.color = selected ? selectedColor : unselectedColor;
    }
    public void ResetSpecialSelection()
    {
        selectedEquipSlot = -1;
        selectedSpecialAbility = -1;
    }
    private void OnDisable()
    {
        settings.Save();
    }
}
