using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour, IAbility
{
    [SerializeField] float cooldown;
    [SerializeField] float activeTime, effectRadius;
    [SerializeField] LayerMask effectMask;
    [SerializeField] Texture Icon;
    float timeLeft;
    Queue<IFreezable> effectedObjects;

    float IAbility.CoolDown { get => cooldown; set => cooldown = value; }
    bool IAbility.NeedsAim { get => false; set { } }
    string IAbility.Name { get => gameObject.name; set { } }
    bool IAbility.Active { get => gameObject.activeSelf; set { } }
    [SerializeField] SoundProfileSO UseSFX;
    SoundProfile IAbility.UseSFX { get => UseSFX.SoundProfile; set { } }


    public static Freeze Instance;
    private bool cancel = false;

    public void Activate(GameObject owner, RaycastHit hit)
    {
        Cancel();
        Instance = Instantiate(this, owner.transform.position, Quaternion.identity);
    }

    private void Awake()
    {
        //Play Freeze Sound
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
        while(timeLeft > 0 && !cancel)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        while (effectedObjects.Count > 0)
            effectedObjects.Dequeue().UnFreeze();
        CanvasEnabler.EnableCanvas("FreezeUI", false);

        GameObject.Destroy(gameObject);
    }

    public void Cancel()
    {
        if (!Instance) return;
        print("canceling freeze");
        Instance.cancel = true;
    }
}
