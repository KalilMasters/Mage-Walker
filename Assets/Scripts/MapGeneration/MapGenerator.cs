using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    [Header("Debug Info")]
    [SerializeField] private AnimationCurve _valueGraph;

    [Header("Perlin Configuration")]
    [SerializeField] private FloatContainer _perlinScale;
    public float Scale => _perlinScale ? _perlinScale.Value : 1;
    [field: SerializeField] public float Seed { get; private set; }
    private int _perlinIndex = 0;

    [SerializeField] ChunkType[] _chunkTypes;
    [field: SerializeField] public List<Chunk> Chunks { get; private set; } = new();
    private Transform _chunkParent;

    [SerializeField] private ChunkType _startChunkType;
    [SerializeField] private int _startChunkAmount;
    [field: SerializeField] public int RowSize { get; private set; }

    private List<Row> AllRows = new();
    bool initialized = false;
    public int RowCount => AllRows.Count;

    public UnityEvent<bool, Chunk> OnNewChunk;
    private void GenerateChunk()
    {
        Chunk chunk = new GameObject("Chunk").AddComponent<Chunk>();
        chunk.transform.parent = _chunkParent;

        float newChunkBackPosition;
        bool isStartChunk = false;
        if (Chunks.Count == 0)
        {
            print("No Chunks. Creating Start");
            newChunkBackPosition = 1;
            isStartChunk = true;

            chunk.Length = _startChunkAmount;
            chunk.SpawnDependets = false;
        }
        else
            newChunkBackPosition = Chunks[^1].FrontPosition - MapManager.Instance.LengthToPercent(1);

        Chunks.Add(chunk);

        int chunkIndex = UnityEngine.Random.Range(0, _chunkTypes.Length);
        ChunkType newChunkType = isStartChunk? _startChunkType : _chunkTypes[chunkIndex];
        chunk.Generate(newChunkType, newChunkBackPosition);

        OnNewChunk?.Invoke(!initialized, chunk);

        Debug.Log($"{newChunkType.name}\n B: {chunk.BackPosition} F: {chunk.FrontPosition}", chunk);
    }
    public void Init()
    {
        if (initialized) return;
        Instance = this;

        _chunkParent = new GameObject("Chunks").transform;
        _chunkParent.parent = transform;
        MapManager.Instance.Scroller.AddScrollParent(_chunkParent);

        //Setting Initial Rows
        if (Seed == 0)
            Seed = UnityEngine.Random.value * 10;

        Row.RowsCreated = 0;

        do
        {
            CheckChunkBoundaries();
        } while (Chunks[^1].FrontPosition > 0);
        initialized = true; 
    }
    void CheckChunkBoundaries()
    {
        //Generate New Chunk If there arent Any
        if (Chunks.Count == 0)
        {
            Debug.Log("No Chunks found.. creating one");
            GenerateChunk();
            return;
        }

        Chunk oldestChunk = Chunks[0];
        Vector3 WS_frontPosition = oldestChunk.WS_FrontPosition;
        bool oldestChunkCompletelyOutOfView = MapManager.Instance.IsAboveLimit(WS_frontPosition, 1);
        if (oldestChunkCompletelyOutOfView)
        {
            Debug.Log("Oldest Chunk is out of view.. Disabling Chunk", oldestChunk);
            Chunks.Remove(oldestChunk);
            oldestChunk.Disable();
        }

        Chunk newestChunk = Chunks[^1];
        bool newestChunkCompletelyVisible = newestChunk.FrontPosition > 0;//MapManager.Instance.IsAboveLimit(WS_backPosition, 0);
        if (newestChunkCompletelyVisible)
        {
            Debug.Log("newest chunk is completely visible! " +newestChunk.FrontPosition, newestChunk);
            GenerateChunk();
        }
    }
    private void Update()
    {
        CheckChunkBoundaries();
    }
    public void AddRows(List<Row> rows)
    {
        foreach (Row row in rows)
            if (!AllRows.Contains(row))
                AllRows.Add(row);
    }
    public void RemoveRows(List<Row> rows)
    {
        foreach (Row row in rows)
            if (AllRows.Contains(row))
                AllRows.Remove(row);
    }
    public Row GetRow(int index)
    {
        if (AllRows.Count == 0 || index >= AllRows.Count || index < 0)
            throw new ArgumentOutOfRangeException("Row Index not Valid: " + index);

        return AllRows[index];
    }
}