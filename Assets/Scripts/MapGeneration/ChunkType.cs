using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Chunk Type", menuName = "Generation/Chunk Type")]
public class ChunkType : ScriptableObject
{
    public List<Row.RowType> rowTypes;
    public Enemy enemy;
    public Row.RowType GetType(float percent)
    {
        int count = rowTypes.Count;

        int index = Mathf.FloorToInt(count * percent);
        return rowTypes[index];
    }
}
