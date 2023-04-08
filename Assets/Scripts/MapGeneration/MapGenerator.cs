using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }


    [Header("Debug Info")]
    [SerializeField] private AnimationCurve _valueGraph;
    public int[] TypeCount;
    public int[] PerlinCount;

    [Header("Perlin Configuration")]
    [SerializeField] private FloatContainer _perlinScale;
    [SerializeField] private float _perlinSeed;
    private int _perlinIndex = 0;

    public List<Row> Rows { get; private set; } = new List<Row>();
    Transform rowParent;
    [field: SerializeField] public int StartPatchAmount { get; private set; }
    [field: SerializeField] public int RowSize { get; private set; }

    public UnityEvent<bool, Row> OnNewRow;





    private void AddNewRow(bool frontLoad = false, bool startRow = false)
    {
        Row.RowType type = startRow ? Row.RowType.Grass : GetNewType();
        GameObject rowGO = new GameObject(type.ToString() + "," + (_perlinIndex - 2).ToString());

        rowGO.transform.parent = rowParent;
        Vector3 localPos = MapManager.Instance.Scroller.ScrollDirection.Opposite().ToVector3();
        if (frontLoad)
        {
            localPos *= (rowParent.childCount - 1);
            localPos += MapManager.Instance.GetLocalEndPosition();
        }
        else
        {
            localPos += Rows[^1].transform.localPosition;
        }


        rowGO.transform.localPosition = localPos;

        Row newRow = rowGO.AddComponent<Row>();
        newRow.type = type;
        newRow.Init(RowSize, MapManager.Instance.Scroller.ScrollDirection, !startRow);
        Rows.Add(newRow);
        _perlinIndex++;

        OnNewRow?.Invoke(startRow, newRow);

        Row.RowType GetNewType()
        {
            float perlinValue = Mathf.PerlinNoise(_perlinSeed, _perlinIndex * _perlinScale);
            int perlinValueInt = Mathf.RoundToInt(perlinValue * 10);
            perlinValueInt = Mathf.Clamp(perlinValueInt, 0, 9);
            int rowTypeAmount = Enum.GetValues(typeof(Row.RowType)).Length;
            int typeIndex = (perlinValueInt * rowTypeAmount) / 10;
            var type = (Row.RowType)typeIndex;
            _valueGraph.AddKey(_perlinIndex, perlinValue);
            TypeCount[typeIndex]++;
            PerlinCount[perlinValueInt]++;
            //print(perlinValue + "-" + type);
            return type;
        }
    }
    private void PurgeOldestRow()
    {
        var row = Rows[0];
        Rows.Remove(row);
        Queue<IDamageable> killed = new(row.gameObject.GetComponentsInChildren<IDamageable>());
        while (killed.Count > 0)
            killed.Dequeue().Damage("Fell Off", DamageType.InstantDeath);
        row.Disable();
    }
    public void Init()
    {
        Instance = this;

        rowParent = new GameObject("Rows").transform;
        MapManager.Instance.Scroller.AddScrollParent(rowParent);

        //Setting Initial Rows
        TypeCount = new int[Enum.GetValues(typeof(Row.RowType)).Length];
        PerlinCount = new int[10];
        if (_perlinSeed == 0)
            _perlinSeed = UnityEngine.Random.value * 10;

        for (int i = 0; i < MapManager.Instance.VisibleLength; i++)
            AddNewRow(true, i < StartPatchAmount);
    }

    private void Update()
    {
        
       
        CheckIfSpawnNew();
        void CheckIfSpawnNew()
        {
            float valueInDirection = Rows[0].transform.localPosition.GetValueInDirection(MapScroller.Instance.ScrollDirection);
            bool spawnNext = valueInDirection > MapManager.Instance.VisibleLength / 2;
            if (!spawnNext) return;
            PurgeOldestRow();
            AddNewRow(false);
        }
    }
}