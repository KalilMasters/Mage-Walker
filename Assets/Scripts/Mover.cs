using UnityEngine;

public class Mover : MonoBehaviour, IFreezable
{
    public Direction2D direction = Direction2D.None;
    [SerializeField] FloatContainer _baseSpeed;
    public float BaseSpeed => _baseSpeed ? _baseSpeed.Value : 1;
    public float VariedSpeed;
    public float distanceToStop;
    public Vector3 localDeactivationPosition;
    public System.Action<Mover> OnMoverEnd;
    public bool RespectOtherMovers;
    bool active = false;
    float totalDistance;

    [field: SerializeField] public bool IsFrozen { get; private set; } = false;

    private void Update()
    {
        if (!active) return;
        if (CheckFront())
        {
            if (RespectOtherMovers)
            {
                OnMoverEnd?.Invoke(this);
                return;
            }
        }

        transform.localPosition += direction.ToVector3() * Time.deltaTime * VariedSpeed;
        distanceToStop = Mathf.Abs(localDeactivationPosition.GetValueInDirection(direction) - transform.localPosition.GetValueInDirection(direction));
        if (distanceToStop > 0.5f) return;
        OnMoverEnd?.Invoke(this);
        active = false;
    }
    bool CheckFront()
    {
        float checkDistance = 1;
        Collider[] frontCheck = Physics.OverlapSphere(transform.position + direction.ToVector3() * checkDistance, 0.1f);

        foreach (Collider c in frontCheck)
            if (c.GetComponent<Mover>() && !c.gameObject.Equals(gameObject))
                return true;

        return false;
    }
    public void SetDeactivationPosition(Vector3 pos, Direction2D moveDirection)
    {
        localDeactivationPosition = pos;
        active = true;
        totalDistance = Vector3.Distance(transform.localPosition, localDeactivationPosition);
        direction = moveDirection;
        SetRotation();
        //Debug.Log($"Active: {gameObject.name}", gameObject);
        void SetRotation()
        {
        
            switch (moveDirection)
            {
                case Direction2D.None:
                case Direction2D.Up:
                    transform.RotateToDirection(Direction2D.None);
                    break;
                case Direction2D.Down:
                    transform.RotateToDirection(Direction2D.None);
                    transform.RotateInDirection(Direction2D.Right, 4);
                    break;
                default:
                    transform.RotateToDirection((moveDirection));
                    break;
            }
        }
    }
    private void OnDrawGizmos()
    {
        float percent = distanceToStop / totalDistance;
        Gizmos.color = Color.Lerp(Color.red, Color.green, percent);
        //Gizmos.DrawSphere(transform.position + (localDeactivationPosition - transform.localPosition), 0.25f);
    }
    private void OnDestroy()
    {
        OnMoverEnd?.Invoke(this);
    }
    public void Freeze()
    {
        active = false;
    }

    public void UnFreeze()
    {
        active = true;

    }
}
