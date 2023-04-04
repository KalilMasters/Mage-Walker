using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    [SerializeField] private FloatContainer _scrollSpeed;
    [field: SerializeField] public bool IsScrolling { get; private set; }

    [SerializeField, Range(0, 1)] private float _speedUpThreshold;
    [SerializeField, Range(0, 10)] private float _speedUpMultiplier;
    [SerializeField] float _scrollSpeedMultiplier, _currentSpeed;
    [SerializeField] private int _visibleLength;
    [field: SerializeField] public Direction2D ScrollDirection { get; private set; }
    [field: SerializeField] public Direction2D PrevScrollDirection { get; private set; }

    private bool isOverrideSpeed;
    private float overrideSpeed;
}
