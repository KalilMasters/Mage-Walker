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

    public static int primaryAbility = 0;
    [SerializeField] Image[] primaryBackgrounds;

    [SerializeField] bool selectedSpecialAbility = false;
    [SerializeField] bool selectedSpecialEquipSlot = false;
    [SerializeField] int currentEquipSlot;
    [SerializeField] int currentSpecialAbility;
    [SerializeField] Image[] specialBackgrounds;
    [SerializeField] Texture[] specialAbilityIcons;
    [SerializeField] Image[] equipSlotBackgrounds;
    [SerializeField] RawImage[] equipSlotIcon;
    public static int[] specialAbilities = new int[2] { 0, 1 };
    // Start is called before the first frame update
    void Start()
    {
        primaryBackgrounds[primaryAbility].color = selectedColor;
        ResetSpecialSelection();
    }

    // Update is called once per frame
    void Update()
    {
        
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
