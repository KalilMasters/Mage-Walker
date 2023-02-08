using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Float", menuName = "MySOs/Float")]
public class FloatContainer : ScriptableObject
{
    [field: SerializeField]  public float Value { get; private set; }
    public static float operator *(float a, FloatContainer b) => a * b.Value;
    public static float operator *(FloatContainer a, float b) => a.Value * b;
    public static float operator *(FloatContainer a, FloatContainer b) => a.Value * b.Value;
}
