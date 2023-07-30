using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Ability SO", menuName = "MySOs/Ability")]
public class AbilitySO : ScriptableObject
{
    public GameObject Prefab;
    public IAbility AbilityComponent => Prefab.GetComponent<IAbility>();
    public Image Icon;
    public Texture Texture;
    public float Cooldown;
}
