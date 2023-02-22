using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShieldManager : MonoBehaviour
{
    [SerializeField] bool HardcoreMode;

    [SerializeField] int ShieldHitPoints;
    [SerializeField] bool ShieldBroken;

    [SerializeField] bool Invincible = false;
    [SerializeField] float InvincibilityTime;
    [SerializeField] float counter = 0;
    [SerializeField] TMP_Text ShieldText; // Debug for now
    [SerializeField] TMP_Text HardcoreModeText; 
    // Start is called before the first frame update
    void Start()
    {
        if (HardcoreMode)
            ShieldHitPoints = 0;
        ShieldBroken = !(ShieldHitPoints > 0);
        ShieldText.text = "Shield: " + ShieldHitPoints.ToString();
        HardcoreModeText.text = "HARDCORE:" + (HardcoreMode ? "ON" : "OFF");
    }

    // Update is called once per frame
    void Update()
    {
        ManageInvincibilityTime();
    }
    void ManageInvincibilityTime()
    {
        if (!Invincible)
            return;

        if (counter < InvincibilityTime)
        {
            counter += Time.deltaTime;
        }
        else
        {
            counter = 0;
            Invincible = false;
        }
    }
    public void TakeDamage(int damage)
    {
        if(ShieldBroken) { return; }

        if (!Invincible)
        {
            ShieldHitPoints -= damage;
            if (ShieldHitPoints <= 0)
                ShieldBroken = true;
            ShieldText.text = "Shield: " + ShieldHitPoints.ToString();
            Invincible = true;
        }
    }
}
