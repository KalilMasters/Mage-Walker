using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetingSystem : MonoBehaviour
{
    public static event System.Action<RaycastHit> OnTargetObject;
    public LayerMask hitmask;
    public void TryTarget()
    {
        if (Input.touchCount == 0) return;
        var touch = Input.touches[0];
        if (touch.phase != UnityEngine.TouchPhase.Began) return;
        Vector2 touchPosition = touch.position;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out RaycastHit hit, float.MaxValue, hitmask, QueryTriggerInteraction.Collide)) return;
        OnTargetObject?.Invoke(hit);
        print(hit.collider.gameObject.name);
    }
    private void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == UnityEngine.TouchPhase.Began)
            TryTarget();
    }
    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
    }
}
