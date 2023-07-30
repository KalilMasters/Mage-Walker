using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    [SerializeField] float CDTimer = 0;
    [SerializeField] float Cooldown;
    [field: SerializeField] public bool Used { get; private set; } = false; // if true, cooldown timer starts
    [SerializeField] Slider CDVisual;
    [SerializeField] GameObject AbilityPrefab;
    public IAbility AbilityComponent;
    Image BackgroundImage;
    RawImage IconImage;

    void Awake()
    {
        if(CDVisual && CDVisual.transform.childCount > 0)
            CDVisual.transform.GetChild(0).TryGetComponent(out BackgroundImage);
        IconImage = GetComponentInChildren<RawImage>();
    }
    public void Init(AbilitySO ability)
    {
        Debug.Log("Setting ability " + ability.name, gameObject);
        AbilityPrefab = ability.Prefab;
        AbilityComponent = AbilityPrefab.GetComponent<IAbility>();
        Cooldown = ability.Cooldown;
        if(CDVisual != null)
        {
            CDVisual.maxValue = Cooldown;
            CDVisual.value = CDTimer;
        }
        if (IconImage != null)
            IconImage.texture = ability.Texture;
    }
    public void ManageCooldown()
    {
        if (!Used) return;

        CDTimer += Time.deltaTime;
        SetBackgroundColor(Color.grey);


        if (CDTimer >= Cooldown)
        {
            CDTimer = 0;
            Used = false;
            SetBackgroundColor(Color.black);
        }

        if (CDVisual != null)
            CDVisual.value = CDTimer;
        
    }
    public void SetBackgroundColor(Color c)
    {
        if (!BackgroundImage) return;
        BackgroundImage.color = c;
    }
    public void SetUsed() => Used = true;
}
