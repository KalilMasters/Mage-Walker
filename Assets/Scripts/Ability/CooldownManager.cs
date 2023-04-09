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
    Image BackgroundImage;
    Image BackgroundOutline;
    // Start is called before the first frame update
    void Awake()
    {
        if(CDVisual && CDVisual.transform.childCount > 0)
            CDVisual.transform.GetChild(0).TryGetComponent(out BackgroundImage);
        //if (CDVisual && CDVisual.transform.childCount > 0)
            //CDVisual.transform.GetChild(1).TryGetComponent(out BackgroundOutline);
        AbilityComponent = AbilityPrefab.GetComponent<IAbility>();
        //if(BackgroundOutline)
            //SetOutline(false);
        SetCooldown(AbilityComponent.CoolDown);
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
        if(BackgroundImage)
            BackgroundImage.color = c;
    }
    public void SetOutline(bool b)
    {
        if(BackgroundOutline)
            BackgroundOutline.gameObject.SetActive(b);
    }
    public void SetCooldown(float CD)
    { Cooldown = CD; }
    public bool GetUsed()
    { return Used; }
    public void SetUsed(bool newUsed)
    { Used = newUsed; }
}
