using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator pAnimator;
    [SerializeField] ShieldManager sManager;
    [SerializeField] CharacterController cController;
    string lastTrigger = "";
    bool resetRotation = false;
    float elapsedTime = 0;
    float waitTime = 2;
    float rotateSpeed = 0.05f;
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
        cController.enabled = true;

    }
    private void Update()
    {
        HandleResetRotation();
    }
    void HandleResetRotation()
    {
        if (resetRotation && elapsedTime < waitTime)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, elapsedTime * rotateSpeed);
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= waitTime)
            {
                resetRotation = false;
                elapsedTime = 0;
            }
        }
    }
    public void LookAtTarget(RaycastHit hit)
    {
        this.transform.LookAt(hit.transform);
        resetRotation = true;
        elapsedTime = 0;
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
