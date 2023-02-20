using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] GameObject Ability1;
    [SerializeField] CooldownManager ACD1;

    [SerializeField] GameObject Ability2;
    [SerializeField] CooldownManager ACD2;
    // Start is called before the first frame update
    void Start()
    {
        ACD1.SetCooldown(Ability1.GetComponent<Projectile>().GetCooldown());
        ACD2.SetCooldown(Ability2.GetComponent<Projectile>().GetCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        ACD1.ManageCooldown();
        ACD2.ManageCooldown();
    }
    public void AbilityOne()
    {
        if (!ACD1.GetUsed())
        {
            Instantiate(Ability1, transform.position, transform.rotation);
            ACD1.SetUsed(true);
        }
    }
    public void AbilityTwo()
    {
        if (!ACD2.GetUsed())
        {
            Instantiate(Ability2, transform.position, transform.rotation);
            ACD2.SetUsed(true);
        }
    }
}
