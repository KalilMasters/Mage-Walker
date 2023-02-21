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
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitVisual(0.01f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator InitVisual(float delay)
    {
        yield return new WaitForSeconds(delay);
        CDVisual.maxValue = Cooldown;
        CDVisual.value = CDTimer;
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
