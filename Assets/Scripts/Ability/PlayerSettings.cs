using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSettings", menuName = "MySOs/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public int PrimaryAbility = 0; // Stores the player's primary ability choice
    public List<int> SpecialAbilities = new(){ 0, 1 }; //Stores the player's special abilities

    public List<AbilitySO> PrimaryAbilityStorage;
    public List<AbilitySO> SpecialAbilityStorage;
    public void Save()
    {
        PlayerPrefs.SetString("Abilities", $"{PrimaryAbility},{SpecialAbilities[0]},{SpecialAbilities[1]}");
    }
    public void Load()
    {
        string[] split = PlayerPrefs.GetString("Abilities", "0,0,1").Split(",");
        PrimaryAbility = int.Parse(split[0]);
        SpecialAbilities = new() { int.Parse(split[1]), int.Parse(split[2]) };
    }
    public void Reset()
    {
        PrimaryAbility = 0;
        SpecialAbilities = new() { 0, 1};
    }
}
