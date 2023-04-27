using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk : MonoBehaviour
{
    #region Bounds
    public Vector3 L_CenterPosition, L_BackPosition, L_FrontPosition;
    public Vector3 WS_CenterPosition
    {
        get
        {
            return transform.TransformPoint(L_CenterPosition);
        }
        private set
        {
            L_CenterPosition = transform.InverseTransformPoint(value);
        }
    }
    public Vector3 WS_BackPosition
    {
        get
        {
            return transform.TransformPoint(L_BackPosition);
        }
        private set
        {
            L_BackPosition = transform.InverseTransformPoint(value);
        }
    }
    public Vector3 WS_FrontPosition
    {
        get 
        { 
            return transform.TransformPoint(L_FrontPosition); 
        }
        private set
        {
            L_FrontPosition = transform.InverseTransformPoint(value);
        }
    }

    public float FrontPosition => MapManager.Instance.GetPercent(WS_FrontPosition);
    public float BackPosition => MapManager.Instance.GetPercent(WS_BackPosition);
    public float CenterPosition => MapManager.Instance.GetPercent(WS_CenterPosition);
    private readonly int MinLength = 4, MaxLength = 10;

    [SerializeField] float front, back, center;
    #endregion

    [field: SerializeField] public ChunkType Type { get; private set; }
    public int Length = -1;
    public bool SpawnDependets = true;

    private Transform rowParent;
    List<Row> rows = new();
    public void Generate(ChunkType chunkType, float WS_backPositionPercent)
    {
        #region Bounds Creation

        if(Length == -1)
            Length = Random.Range(MinLength, MaxLength + 1);

        float lengthAsPercent = MapManager.Instance.LengthToPercent(Length-1);
        float middlePercent = WS_backPositionPercent - (lengthAsPercent / 2);
        float frontPercent = WS_backPositionPercent - lengthAsPercent;

        Vector3 centerPosition = MapManager.Instance.GetPosition(middlePercent);
        transform.position = centerPosition;
        WS_CenterPosition = centerPosition;

        WS_BackPosition = MapManager.Instance.GetPosition(WS_backPositionPercent);
        WS_FrontPosition = MapManager.Instance.GetPosition(frontPercent);
        #endregion
        //return;
        
        Type = chunkType;

        rowParent = new GameObject("Rows").transform;
        rowParent.parent = transform;
        rowParent.localPosition = Vector3.zero;

        while(rows.Count < Length)
        {
            Row.RowType type = GetNewType();
            GameObject rowGO = new GameObject(type.ToString());

            rowGO.transform.parent = rowParent;
            float percent = (float)rows.Count / (Length-1);

            rowGO.transform.localPosition = 
                Vector3.Lerp(L_BackPosition, L_FrontPosition, percent);

            Row newRow = rowGO.AddComponent<Row>();
            newRow.type = type;
            newRow.Init(MapGenerator.Instance.RowSize, MapScroller.Instance.ScrollDirection, SpawnDependets);
            rows.Add(newRow);
        }
        Row.RowType GetNewType()
        {
            float perlinValue = Mathf.PerlinNoise(MapGenerator.Instance.Seed, rowParent.childCount * MapGenerator.Instance.Scale);
            return Type.GetType(perlinValue);
        }
    }
    private void Update()
    {
        if (Length == 0) return;
        front = FrontPosition;
        center = CenterPosition;
        back = BackPosition;
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        //GameObject.Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (MapManager.Instance == null) return;

        Gizmos.color = BackPosition > 0? Color.red : Color.magenta;
        Gizmos.DrawSphere(WS_BackPosition, 0.1f);
        
        Gizmos.color = CenterPosition > 0 ? Color.white : Color.grey;
        Gizmos.DrawSphere(WS_CenterPosition, 0.1f);

        Gizmos.color = FrontPosition > 0 ? Color.green : Color.yellow;
        Gizmos.DrawSphere(WS_FrontPosition, 0.1f);
    }
}
