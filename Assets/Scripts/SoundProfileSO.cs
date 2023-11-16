using UnityEngine;

[CreateAssetMenu(fileName = "Sound Profile", menuName = "MySOs/Sound Profile")]
public class SoundProfileSO : ScriptableObject
{
    [field: SerializeField] public SoundProfile SoundProfile { get; private set; }
}
