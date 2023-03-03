using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{
    Audio adio;
    public AudioClip jump, splash;
    private Coroutine _moveCoroutine;
    [SerializeField] private FloatContainer _moveSpeed, _movementCheckSize;
    [SerializeField] private Direction2D _currentDirection;
    Vector3 _checkSpot;
    [SerializeField] Transform hitTransform;
    [SerializeField] LayerMask MoveMask, KillMask;
    Vector3 hitPoint = Vector3.zero;
    float colliderRadius;
    MeshRenderer visual;
    public System.Action<Direction2D> OnMove;
    public void TryMove(InputAction.CallbackContext ctx) =>
        TryMove(ctx.ReadValue<Vector2>().ToDirection());

    [SerializeField] ShieldManager SM;
    [SerializeField] EndScreen _EndScreen;
    public void Kill(string killerName)
    {
        if (killerName.Equals("Water"))
        {
            adio.sound(splash);
            Debug.Log("Working");
        }
        if (SM.TakeDamage(1))
        {
            
            return;
        }
        gameObject.SetActive(false);
        _EndScreen.ActivateEndScreen();
    }
    public void TryMove(Direction2D moveDirection)
    {
        _currentDirection = moveDirection;
        if (_moveCoroutine != null) return;
        if (moveDirection == Direction2D.None) return;
        RaycastHit? hitObject = GetSpaceInDirection(moveDirection, transform.position);
        if (hitObject == null || !hitObject.HasValue) return;
        hitTransform = hitObject.Value.transform;
        if (hitTransform == transform) return;

        transform.parent = hitTransform;
        OnMove?.Invoke(moveDirection);
        _moveCoroutine = StartCoroutine(Move());
        IEnumerator Move()
        {
            float percent = 0;
            Vector3 startPosition = transform.localPosition;
            hitPoint = GetSpaceInDirection(Direction2D.None, hitTransform.position).Value.point;
            float localHitY = hitTransform.InverseTransformPoint(hitPoint).y;
            Vector3 endPosition = Vector3.up * (localHitY + colliderRadius);
            //Play Sound
            adio.sound(jump);
            while(percent < 1)
            {
                percent += Time.deltaTime * _moveSpeed;
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, percent);
                yield return null;
            }
            transform.localPosition = endPosition;
            _moveCoroutine = null;
            //if (_currentDirection != Direction.None)
                //TryMove(_currentDirection);
        }
    }
    RaycastHit? GetSpaceInDirection(Direction2D checkDirection, Vector3 startPosition)
    {
        _checkSpot = startPosition + checkDirection.ToVector3() + Vector3.up * 2;
        if (Physics.SphereCast(_checkSpot, colliderRadius * _movementCheckSize.Value, Vector3.down ,out RaycastHit hit, float.MaxValue ,MoveMask, QueryTriggerInteraction.Ignore))
            if(hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("MoveSpace")))
                return hit;
        return null;
    }
    private void Update()
    {
        Collider[] overlap = Physics.OverlapSphere(transform.position, colliderRadius, KillMask, QueryTriggerInteraction.Collide);
        bool touchingKill = false;
        foreach (Collider col in overlap)
        {
            if(col.TryGetComponent(out KillScript kill))
            {
                Kill(kill.gameObject.name);
                touchingKill = true;
                break;
            }
        }
        visual.material.color = touchingKill? Color.red : Color.white;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_checkSpot, 0.25f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_checkSpot, _checkSpot + Vector3.down * 2);
        if(hitTransform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(hitTransform.position, 0.5f);
            Gizmos.DrawSphere(hitPoint, 0.25f);
        }

        Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, 0.50001f);
    }
    private void Awake()
    {
        adio = FindObjectOfType<Audio>();
        colliderRadius = GetComponent<SphereCollider>().radius;
        visual = GetComponent<MeshRenderer>();
    }
}
public static class Utility
{
    public static Vector3 ToVector3(this Direction2D d) => d switch
    {
        Direction2D.Up => Vector3.forward,
        Direction2D.Down => Vector3.back,
        Direction2D.Left => Vector3.left,
        Direction2D.Right => Vector3.right,
        _ => Vector3.zero
    };
    public static Vector2 ToVector2(this Direction2D d) => d switch
    {
        Direction2D.Up => Vector2.up,
        Direction2D.Down => Vector3.down,
        Direction2D.Left => Vector3.left,
        Direction2D.Right => Vector3.right,
        _ => Vector3.zero
    };
    public static Direction2D ToDirection(this Vector2 v)
    {
        if(v == Vector2.zero) return Direction2D.None;
        if(Mathf.Abs(v.x) > Mathf.Abs(v.y))
            return v.x > 0? Direction2D.Right : Direction2D.Left;
        return v.y > 0? Direction2D.Up : Direction2D.Down;
    }
    public static Direction2D ToDirection(this Vector3 v)
    {
        if (v == Vector3.zero) return Direction2D.None;
        if (v.x > v.z)
            return v.x > 0 ? Direction2D.Right : Direction2D.Left;
        return v.z > 0 ? Direction2D.Up : Direction2D.Down;
    }
    public static Vector3 Expand(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
    public static Vector2 Flatten(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    public static Direction2D Opposite(this Direction2D d)
    {
        return d.Rotate().Rotate();
    }
    public static Direction2D Rotate(this Direction2D d, bool useNone = false)
    {
        if(d == Direction2D.None && !useNone) return Direction2D.None;
        int index = (int)d;
        return (Direction2D)((++index) % (useNone? 5 : 4));
    }
    public static float GetValueInDirection(this Vector3 v, Direction2D d)
    {
        switch (d)
        {
            case Direction2D.Up: return v.z;
            case Direction2D.Down: return -v.z;
            case Direction2D.Right: return v.x;
            case Direction2D.Left: return -v.x;
            default: return 0;
        }
    }
    public static void RotateInDirection(this Transform t, Direction2D d, float degrees = 90)
    {
        t.Rotate(d.DirToRotAxis() * degrees);
    }
    public static Vector3 DirToRotAxis(this Direction2D d) => d switch
    {
        Direction2D.Up => Vector3.right,
        Direction2D.Down => Vector3.left,
        Direction2D.Left => Vector3.down,
        Direction2D.Right => Vector3.up,
        _ => Vector3.zero
    };
    public static void RotateToDirection(this Transform t, Direction2D d)
    {
        t.eulerAngles = d.DirToRotAxis() * 90;
    }
    public static Direction2D GetDirection(this Transform t)
    {
        return Direction2D.None;
    }
    public static Direction3D To3D(this Direction2D d2D)
    {
        switch (d2D)
        {
            case Direction2D.Up: return Direction3D.Forward;
            case Direction2D.Down: return Direction3D.Back;
            case Direction2D.Left: return Direction3D.Left;
            case Direction2D.Right: return Direction3D.Right;
            default: return Direction3D.None;
        }
    }
    public static Direction2D To2D(this Direction3D d3D)
    {
        switch (d3D)
        {
            case Direction3D.Forward: return Direction2D.Up;
            case Direction3D.Back: return Direction2D.Down;
            case Direction3D.Left: return Direction2D.Left;
            case Direction3D.Right: return Direction2D.Right;
            case Direction3D.Up: return Direction2D.None;
            case Direction3D.Down: return Direction2D.None;
            default: return Direction2D.None;
        }
    }
    public static Vector3 Center(this Vector3 v3)
    {
        return new Vector3(v3.x/2, v3.y/2, v3.z/2);
    }
}
public enum Direction2D { Up, Right, Down, Left, None }
public enum Direction3D { Forward, Right , Back, Left, Up, Down, None }