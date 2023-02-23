using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour, IAbility
{
    [SerializeField] float cooldown;
    [SerializeField] float activeTime, effectRadius;
    [SerializeField] LayerMask effectMask;
    float timeLeft;
    Queue<IFreezable> effectedObjects;
    public void Activate(Transform player, RaycastHit hit)
    {
        Instantiate(gameObject, player.transform.position, Quaternion.identity);
    }
    private void Awake()
    {
        timeLeft = activeTime;
        effectedObjects = new();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius, effectMask);
        foreach(Collider c in hitColliders)
        {
            if(c.TryGetComponent<IFreezable>(out IFreezable freezable))
            {
                freezable.Freeze();
                effectedObjects.Enqueue(freezable);
            }
        }
        StartCoroutine(UnFreeze());
    }
    IEnumerator UnFreeze()
    {
        while(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        while (effectedObjects.Count > 0)
            effectedObjects.Dequeue().UnFreeze();
        GameObject.Destroy(gameObject);
    }
    public float CoolDown() => cooldown;
    public bool NeedsAim() => false;
}
