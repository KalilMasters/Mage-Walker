using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] private AnimationCurve perlinValues;
    [SerializeField] private ChunkType BiomeType;
    [SerializeField] private int length, width, seed;
    [SerializeField] float perlinScale;

    List<Row> rows = new List<Row>();
    Transform rowParent;
    bool generated = false;

    [ContextMenu("Generate Chunk")]
    public void Generate()
    {
        perlinValues = new();
        generated = true;
        ResetRows();

        for(int i = 0; i < length; i++)
        {
            AddNewRow();
        }
    }
    private void AddNewRow()
    {
        Row.RowType type = GetNewType();
        GameObject rowGO = new GameObject(type.ToString());

        rowGO.transform.parent = rowParent;
        Vector3 localPos = Direction2D.Up.ToVector3();


        localPos *= (rows.Count - 1);
        localPos += Direction2D.Down.ToVector3() * length / 2;


        rowGO.transform.localPosition = localPos;

        Row newRow = rowGO.AddComponent<Row>();
        newRow.type = type;
        newRow.Init(width, Direction2D.Down, true);
        rows.Add(newRow);

        Row.RowType GetNewType()
        {
            float perlinValue = Mathf.PerlinNoise(seed, rowParent.childCount * perlinScale);
            perlinValues.AddKey(rowParent.childCount, perlinValue);
            var type = BiomeType.GetType(perlinValue);
            return type;
        }
    }
    void ResetRows()
    {
        if (rowParent == null)
            rowParent = new GameObject("Rows").transform;

        Queue<Row> q = new(rows);
        while(q.Count > 0)
        {
            if (q.Peek() == null)
            {
                q.Dequeue();
                continue;
            }
                
            q.Dequeue().Disable();
        }

        while(rowParent.childCount > 0)
            GameObject.DestroyImmediate(rowParent.GetChild(0).gameObject);

        rows = new();
    }
    private void Awake() => Generate();
}
