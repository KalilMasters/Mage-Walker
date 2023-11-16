using DG.Tweening;
using System;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] Transform cam;
    [SerializeField] Vector3 shakeStrength;
    private static event Action Shake;

    public static void Invoke()
    {
        Shake?.Invoke();
    }
    void OnEnable() => Shake += CameraShake;
    void OnDisable() => Shake -= CameraShake;
    void CameraShake()
    {
        cam.DOComplete();
        cam.DOShakePosition(0.3f, shakeStrength);
    }
}
