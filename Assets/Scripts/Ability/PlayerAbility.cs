using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] GameObject Ability1;
    [SerializeField] GameObject Ability2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AbilityOne()
    {
        Instantiate(Ability1, transform.position, transform.rotation);
    }
    public void AbilityTwo()
    {
        Instantiate(Ability2, transform.position, transform.rotation);
    }
}
