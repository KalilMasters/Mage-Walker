using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    [field: SerializeField] public bool IsScrolling { get; private set; }
    [field: SerializeField] public Direction2D ScrollDirection { get; private set; }
    [field: SerializeField] public Direction2D PrevScrollDirection { get; private set; }

    public SpeedType ScrollSpeedType = SpeedType.Normal;
    public enum SpeedType { Fast, Normal, Slow }

    [SerializeField] private FloatContainer _scrollSpeed;
    [SerializeField, Range(0, 2)] private float _fastModifier, _slowModifier;
    [SerializeField] float _scrollSpeedMultiplier, _currentSpeed;
    [field: SerializeField] public float TotalDistance { get; private set; }

    [field: SerializeField] public bool IsOverrideSpeed;
    private float _overrideSpeed;

    private List<Transform> _scrollObjectParents = new();
    private Transform _generalScrollParent;


    public static MapScroller Instance;

    public void ToggleScroll() => SetScroll(!IsScrolling);
    public void SetScroll(bool on) => IsScrolling = on;
    private void Scroll()
    {
        if (!IsScrolling) return;

        _currentSpeed = GetCurrentSpeed();
        float currentScrollAmount = _currentSpeed * Time.deltaTime;

        if (ScrollSpeedType.Equals(SpeedType.Fast))
            currentScrollAmount *= _fastModifier;
        else if (ScrollSpeedType.Equals(SpeedType.Slow))
            currentScrollAmount *= _slowModifier;

        TotalDistance += currentScrollAmount;

        Vector3 scrollVector = ScrollDirection.ToVector3() * currentScrollAmount;

        Queue<Transform> parentsToCheck = new Queue<Transform>(_scrollObjectParents);

        int i = 0;

        while(parentsToCheck.Count > 0)
        {
            var t = parentsToCheck.Dequeue();

            if(t == null)
            {
                _scrollObjectParents.RemoveAt(i);
                continue;
            }

            foreach (Transform o in t)
                o.transform.localPosition += scrollVector;

            i++;
        }
    }



    private float GetCurrentSpeed()
    {
        if (IsOverrideSpeed)
            return _overrideSpeed;
        return GetNaturalSpeed();
    }
    private float GetNaturalSpeed()
    {
        return _scrollSpeed.Value + _scrollSpeedMultiplier * Time.time;
    }

    
    public IEnumerator ChangeToNaturalSpeed()
    {
        IsScrolling = true;
        yield return ChangeSpeed(GetNaturalSpeed(), true);
        IsOverrideSpeed = false;
    }
    public IEnumerator ChangeToStop()
    {
        IsScrolling = true;
        yield return ChangeSpeed(0, true);
        IsScrolling = false;
    }

    public IEnumerator ChangeSpeed(float newSpeed = 0, bool autoOverride = false)
    {
        if (autoOverride)
            IsOverrideSpeed = true;

        float percent = 0;
        float currentSpeed = GetCurrentSpeed();
        while (percent < 1)
        {
            percent += Time.deltaTime;
            _overrideSpeed = Mathf.Lerp(currentSpeed, newSpeed, percent);
            yield return null;
        }
    }
    public void AddScrollParent(Transform parent)
    {
        if (_scrollObjectParents.Contains(parent)) return;

        _scrollObjectParents.Add(parent);
    }
    public void RemoveScrollParent(Transform parent)
    {
        if (!_scrollObjectParents.Contains(parent)) return;

        _scrollObjectParents.Remove(parent);
    }
    public void AddScrollObject(Transform t)
    {
        t.parent = _generalScrollParent;
    }
    public void RemoveScrollObject(Transform t)
    {
        if (t.parent != _generalScrollParent) return;
        t.parent = null;
    }

    private void Awake()
    {
        Instance = this;

        _generalScrollParent = new GameObject("ScrollObjects").transform;
        _generalScrollParent.parent = transform;

        AddScrollParent(_generalScrollParent);
        PrevScrollDirection = ScrollDirection;
        IsScrolling = false;
    }
    private void Update() => Scroll();

}
