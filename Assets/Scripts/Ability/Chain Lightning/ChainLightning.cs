using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    SphereCollider coll;

    public GameObject chainLightningEffect;

    [SerializeField] LayerMask HitMask;
    public GameObject beenStruck;

    public int amountToChain;

    GameObject startObject;
    public GameObject endObject;

    Animator ani;

    [SerializeField] ParticleSystem parti;

    int singleSpawns;

    void Start()
    {
        if(amountToChain== 0) Destroy(gameObject);

        Destroy(gameObject, .75f);
        coll = GetComponent<SphereCollider>();
        ani = GetComponent<Animator>();
        parti = GetComponent<ParticleSystem>();

        startObject = gameObject;
        singleSpawns = 1;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer + " " + HitMask);
        if(HitMask == (HitMask | (1 << other.gameObject.layer)) && !other.GetComponentInChildren<EnemyStruck>())
        {
            if (singleSpawns != 0)
            {
                if (other.TryGetComponent(out IDamageable d))
                    d.Damage("ChainLightning", DamageType.Pulse);

                endObject = other.gameObject;

                amountToChain -= 1;

                Instantiate(chainLightningEffect, other.gameObject.transform.position, Quaternion.identity);

                Instantiate(beenStruck, other.gameObject.transform);

                ani.StopPlayback();
                coll.enabled = false;
                singleSpawns--;

                parti.Play();

                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = startObject.transform.position;

                parti.Emit(emitParams, 1);

                emitParams.position = endObject.transform.position;

                parti.Emit(emitParams, 1);


                Destroy(gameObject, .69f);
            }
        }
    }
}
