using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Row;

public class Mover : MonoBehaviour
{
    public Direction2D direction = Direction2D.None;
    [SerializeField] FloatContainer _baseSpeed;
    public float BaseSpeed => _baseSpeed ? _baseSpeed.Value : 1;
    public float speed;
    public float distanceToStop;
    public Vector3 localDeactivationPosition;
    public System.Action<Mover> OnMoverEnd;
    bool active = false;
    float totalDistance;
    private void Update()
    {
        if (!active) return;
        transform.localPosition += direction.ToVector3() * Time.deltaTime * speed;
        distanceToStop = Mathf.Abs(localDeactivationPosition.GetValueInDirection(direction) - transform.localPosition.GetValueInDirection(direction));
        if (distanceToStop > 0.5f) return;
        OnMoverEnd?.Invoke(this);
        active = false;
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
    private void OnDisable()
    {
        
    }
}
