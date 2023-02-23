using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    [SerializeField] float CDTimer = 0;
    [SerializeField] float Cooldown;
    [SerializeField] bool Used = false; // if true, cooldown timer starts
    [SerializeField] Slider CDVisual;
    [SerializeField] GameObject AbilityPrefab;
    public IAbility AbilityComponent;
    // Start is called before the first frame update
    void Start()
    {
        AbilityComponent = AbilityPrefab.GetComponent<IAbility>();
        SetCooldown(AbilityComponent.CoolDown());
        StartCoroutine(InitVisual(0.01f));
    }
    IEnumerator InitVisual(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (CDVisual != null)
        {
            CDVisual.maxValue = Cooldown;
            CDVisual.value = CDTimer;
        }
    }
    public void ManageCooldown()
    {
        if (Used)
        {
            CDTimer += Time.deltaTime;
            if (CDTimer >= Cooldown)
            {
                CDTimer = 0;
                Used = false;
            }
            if(CDVisual != null)
                CDVisual.value = CDTimer;
        }
    }
    public void SetCooldown(float CD)
    { Cooldown = CD; }
    public bool GetUsed()
    { return Used; }
    public void SetUsed(bool newUsed)
    { Used = newUsed; }
}
