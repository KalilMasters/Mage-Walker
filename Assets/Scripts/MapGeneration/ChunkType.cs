using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Chunk Type", menuName = "Generation/Chunk Type")]
public class ChunkType : ScriptableObject
{
    public List<Row.RowType> rowTypes;
    public Enemy enemy;
}
