using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float Damage;
    [SerializeField] float Cooldown;
    [SerializeField] Rigidbody rb;
    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * MoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }
    protected virtual void DoStuff()
    {

    }
    public float GetDamage()
    {
        return Damage;
    }
    public float GetCooldown()
    {
        return Cooldown;
    }
    void OnTriggerEnter(Collider collision) {
        if(!(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Projectile" || collision.gameObject.tag == "Lilypad"))
        {
            DoStuff();
            Destroy(gameObject);
        }
    }
}
