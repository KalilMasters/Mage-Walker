using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public static event System.Action<RaycastHit> OnTargetObject;
    public LayerMask hitmask;
    bool RequireHitObject => !MapManager.IsHardMode;
    Vector3 hitPosition;
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
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        if (RequireHitObject)
        {
            if (!Physics.Raycast(ray, out hit, float.MaxValue, hitmask, QueryTriggerInteraction.Collide)) return;
        }
        else
        {
            Vector3 playerPosition = MapManager.Instance.Player.transform.position;
            Vector3 up = Vector3.up;

            Plane playerPlane = new Plane(up, playerPosition);

            playerPlane.Raycast(ray, out float distance);

            Vector3 planePoint = ray.GetPoint(distance);

            hit = new();
            hit.distance = distance;
            hit.normal = Vector3.up;
            hit.point = planePoint;
        }

        hitPosition = hit.point;
        OnTargetObject?.Invoke(hit);
    }
    private void Update() => TryTarget();
    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(hitPosition, 0.1f);
    }
}
