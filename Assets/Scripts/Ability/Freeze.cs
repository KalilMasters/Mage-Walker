using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour, IAbility
{
    public AudioClip freeze;
    Audio adio;
    [SerializeField] float cooldown;
    [SerializeField] float activeTime, effectRadius;
    [SerializeField] LayerMask effectMask;
    float timeLeft;
    Queue<IFreezable> effectedObjects;

    float IAbility.CoolDown { get => cooldown; set => cooldown = value; }
    bool IAbility.NeedsAim { get => false; set { } }
    string IAbility.Name { get => gameObject.name; set { } }

    public void Activate(GameObject owner, RaycastHit hit)
    {
        Instantiate(gameObject, owner.transform.position, Quaternion.identity);
    }

    private void Awake()
    {
        //Play Freeze Sound
        adio = FindObjectOfType<Audio>();
        adio.sound(freeze);
        CanvasEnabler.EnableCanvas("FreezeUI", true);
        timeLeft = activeTime;
        effectedObjects = new();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);
        foreach(Collider c in hitColliders)
        {
            if(c.TryGetComponent(out IFreezable freezable))
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
        CanvasEnabler.EnableCanvas("FreezeUI", false);

        GameObject.Destroy(gameObject);
    }
}
