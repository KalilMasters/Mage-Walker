using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
[CreateAssetMenu(fileName = "Chunk Type", menuName = "Generation/Chunk Type")]
public class ChunkType : ScriptableObject
{
    public List<Row.RowType> rowTypes;
    public Enemy enemy;

    public Row.RowType GetType(float percent)
    {
        Debug.Log(percent);
        int count = rowTypes.Count;

        int index = Mathf.RoundToInt(count * percent);
        return rowTypes[index];
    }
}
