using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAbility : MonoBehaviour
{
    Color unselectedColor = new Color(0.1019608f, 0.1019608f, 0.1019608f);
    Color selectedColor = Color.green;
    Color specialUnselectedColor = Color.black;

    public static int primaryAbility = 0; // Stores the player's primary ability choice
    [SerializeField] Image[] primaryBackgrounds;

    [SerializeField] bool selectedSpecialAbility = false;
    [SerializeField] bool selectedSpecialEquipSlot = false;
    [SerializeField] int currentEquipSlot;
    [SerializeField] int currentSpecialAbility;
    [SerializeField] Image[] specialBackgrounds; // Stores the clickable buttons that represent the special abilities. It is for changing the background color to green to show that the player has selected a special ability
    [SerializeField] Texture[] specialAbilityIcons; // Library of all our special ability icons
    [SerializeField] Image[] equipSlotBackgrounds; // Stores the clickable buttons that represent the equip slot (1, 2). It is for changing the background color to green to show that the player has selected an equip slot
    [SerializeField] RawImage[] equipSlotIcon; // This is for changing the equip icon to match what the player chose to equip
    public static int[] specialAbilities = new int[2] { 0, 1 }; // Stores the player's special ability choices
    // Start is called before the first frame update
    void Start()
    {
        primaryBackgrounds[primaryAbility].color = selectedColor;
        InitSelection();
        ResetSpecialSelection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void InitSelection() // This is the make sure the icons are saved after the player returns to the main menu
    {
        primaryBackgrounds[primaryAbility].color = selectedColor; // Primary ability
        if(primaryAbility == 0) { primaryBackgrounds[1].color = unselectedColor; }
        else { primaryBackgrounds[0].color = unselectedColor; }

        for(int i = 0; i < equipSlotIcon.Length; i++) // Special ability
        {
            equipSlotIcon[i].texture = specialAbilityIcons[specialAbilities[i]];
        }
    }
    public void SetPrimaryAbility(int choice)
    {
        bool alreadySelected = choice == primaryAbility;
        if (alreadySelected)
        {
            return;
        }
        primaryBackgrounds[primaryAbility].color = unselectedColor;
        primaryAbility = choice;
        primaryBackgrounds[primaryAbility].color = selectedColor;
    }
    public void SetSpecialAbilitySlot(int choice) // Clicked on equip slots
    {
        if (CheckForDuplicatedSpellSlots(currentSpecialAbility))
            return;
        if(choice != currentEquipSlot && !(currentEquipSlot == -1)) // If true, it means that the player selected a new equip slot and this is to make the old equip an unselected color
        {
            equipSlotBackgrounds[currentEquipSlot].color = unselectedColor;
        }
        if (choice == currentEquipSlot) // Deselecting
        {
            equipSlotBackgrounds[choice].color = unselectedColor;
            currentEquipSlot = -1;
            selectedSpecialEquipSlot = false;
            return;
        }

        if (selectedSpecialAbility)
        {
            specialAbilities[choice] = currentSpecialAbility;
            equipSlotIcon[choice].texture = specialAbilityIcons[currentSpecialAbility];
            ResetSpecialSelection();
            return;
        }
        selectedSpecialEquipSlot = true;
        equipSlotBackgrounds[choice].color = selectedColor;
        currentEquipSlot = choice;
    }
    public void SetSpecialAbilitySpell(int choice) // Clicked on special ability
    {
        
        if (choice != currentSpecialAbility && !(currentSpecialAbility == -1)) // If true, it means that the player selected a new ability and this is to make the old spell an unselected color
        {
            specialBackgrounds[currentSpecialAbility].color = specialUnselectedColor;
        }
        if (choice == currentSpecialAbility) // Deselecting
        {
            specialBackgrounds[choice].color = specialUnselectedColor;
            currentSpecialAbility = -1;
            selectedSpecialAbility = false;
            return;
        }
        if (selectedSpecialEquipSlot)
        {
            if (CheckForDuplicatedSpellSlots(choice))
                return;
            specialAbilities[currentEquipSlot] = choice;
            equipSlotIcon[currentEquipSlot].texture = specialAbilityIcons[choice];
            ResetSpecialSelection();
            return;
        }
        selectedSpecialAbility = true;
        specialBackgrounds[choice].color = selectedColor;
        currentSpecialAbility = choice;

    }
    public void ResetSpecialSelection()
    {
        foreach(Image x in specialBackgrounds)
        {
            x.color = Color.black;
        }
        foreach(Image x in equipSlotBackgrounds) 
        { 
            x.color = unselectedColor;
        }
        selectedSpecialEquipSlot = false;
        selectedSpecialAbility = false;
        currentEquipSlot = -1;
        currentSpecialAbility = -1;
    }
    public bool CheckForDuplicatedSpellSlots(int choice) // True = dupe, false = no dupe
    {
        bool dupeFound = false;
        foreach (int x in specialAbilities)
        {
            if (x == choice)
            {
                dupeFound = true;
            }
        }
        return dupeFound;
    }
}
