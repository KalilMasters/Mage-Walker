using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Projectile
{
    [SerializeField] GameObject Explosion;
    protected override void DoStuff(Collider collision)
    {
        base.DoStuff(collision);
        Instantiate(Explosion, transform.position, Quaternion.identity);
        //Destroy(Explosion, 2f);
    }
}
