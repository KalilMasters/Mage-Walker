using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSettings", menuName = "MySOs/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public int primaryAbility = 0; // Stores the player's primary ability choice
    public int[] specialAbilities = { 0, 1 }; //Stores the player's special abilities

    public void Reset()
    {
        primaryAbility = 0;
        specialAbilities = new int[] { 0, 1};
    }
}
