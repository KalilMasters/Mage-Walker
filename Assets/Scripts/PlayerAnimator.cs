using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator pAnimator;
    [SerializeField] ShieldManager sManager;
    [SerializeField] CharacterController cController;
    string lastTrigger = "";
    private void Start()
    {
        //pAnimator = GetComponent<Animator>();
    }
    private void Awake()
    {
        cController = GetComponent<CharacterController>();
        cController.OnMove += SetMovement;
        sManager = GetComponent<ShieldManager>();
        sManager.OnShieldDamageTaken += SetShield;
        pAnimator = GetComponentInChildren<Animator>();

    }
    public void ActivateTrigger(string stringName)
    {
        pAnimator.ResetTrigger(lastTrigger);
        pAnimator.SetTrigger(stringName);
        lastTrigger = stringName;
    }
    void SetShield(string source)
    {
        Debug.Log("Damage");
        ActivateTrigger("TakeDamage");
        pAnimator.SetInteger("Shield", sManager.HitPoints);
    }
    void SetMovement(Direction2D direction)
    {
        switch(direction)
        {
            case Direction2D.Up:
                ActivateTrigger("WalkForward");
                break;
            case Direction2D.Down:
                ActivateTrigger("WalkBack");
                break;
            case Direction2D.Left:
                ActivateTrigger("WalkLeft");
                break;
            case Direction2D.Right:
                ActivateTrigger("WalkRight");
                break;
            default:
                ActivateTrigger("");
                break;
        }
    }
}
