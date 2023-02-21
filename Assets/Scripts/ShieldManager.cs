using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShieldManager : MonoBehaviour
{
    [SerializeField] int ShieldHitPoints;
    [SerializeField] bool ShieldBroken;

    [SerializeField] bool Invincible = false;
    [SerializeField] float InvincibilityTime;
    float counter = 0;
    [SerializeField] TMP_Text ShieldText; // Debug for now
    // Start is called before the first frame update
    void Start()
    {
        ShieldBroken = !(ShieldHitPoints > 0);
        ShieldText.text = "Shield: " + ShieldHitPoints.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        else if(counter < InvincibilityTime)
        {
            counter += Time.deltaTime;
        }
        else
        {
            counter = 0;
            Invincible = false;
        }
    }
}
