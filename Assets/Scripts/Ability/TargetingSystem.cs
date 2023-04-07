using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public static event System.Action<RaycastHit> OnTargetObject;
    public LayerMask hitmask;
    public void TryTarget()
    {
        Vector2 touchPosition;
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 0) return;
            var touch = Input.touches[0];
            if (touch.phase != UnityEngine.TouchPhase.Began) return;
            touchPosition = touch.position;
        }
        else
        {
            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

            touchPosition = Input.mousePosition;
        }
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out RaycastHit hit, float.MaxValue, hitmask, QueryTriggerInteraction.Collide)) return;
        OnTargetObject?.Invoke(hit);
        print(hit.collider.gameObject.name);
    }
    private void Update() => TryTarget();
    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
    }
}
