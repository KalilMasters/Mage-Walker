using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public AnimationCurve ValueGraph;
    public int[] TypeCount;
    public int[] PerlinCount;
    public float CurrentScrollAmnount;
    public bool playerAboveHalf;

    [SerializeField] private Direction2D _scrollDirection;
    [SerializeField] private int _visibleLength, _rowSize;
    [SerializeField] private FloatContainer _scrollSpeed, _scale; 
    [SerializeField] private float _seed;
    [field: SerializeField] public bool IsScrolling { get; private set; }
    [SerializeField, Range(0, 1)] private float _speedUpThreshold;
    [SerializeField, Range(0, 10)] private float _speedUpMultiplier;
    [SerializeField] float _scrollSpeedMultiplier,  _currentSpeed;

    private Direction2D _prevScrollDirection;
    private List<Row> _rows = new List<Row>();
    private int _index = 0;
    private CharacterController _player;
    [SerializeField] int playerScore = 0;

    void AddNewRow(bool frontLoad = false, bool startRow = false)
    {
        Row.RowType type = startRow ? Row.RowType.Grass : GetNewType();
        Row newRow = new GameObject(type.ToString() + "," + (_index-2).ToString()).AddComponent<Row>();
        newRow.type = type;
        newRow.transform.parent = transform;
        Vector3 localPos = Vector3.zero;
        if (frontLoad)
        {
            localPos = GetLocalEndPosition() + _scrollDirection.Opposite().ToVector3() * (transform.childCount - 1);
        }
        else
        {
            localPos = _rows[^1].transform.localPosition + _scrollDirection.Opposite().ToVector3();
        }
        newRow.transform.localPosition = localPos;
        newRow.Init(_rowSize, _scrollDirection, !startRow);
        _rows.Add(newRow);
        _index++;
        Row.RowType GetNewType()
        {
            float perlinValue = Mathf.PerlinNoise(_seed, _index * _scale);
            int perlinValueInt = Mathf.RoundToInt(perlinValue * 10);
            perlinValueInt = Mathf.Clamp(perlinValueInt, 0, 9);
            int rowTypeAmount = Enum.GetValues(typeof(Row.RowType)).Length;
            int typeIndex = (perlinValueInt * rowTypeAmount) / 10;
            var type = (Row.RowType)typeIndex;
            ValueGraph.AddKey(_index, perlinValue);
            TypeCount[typeIndex]++;
            PerlinCount[perlinValueInt]++;
            //print(perlinValue + "-" + type);
            return type;
        }
    }
    private Row SpawnRow()
    {
        return null;
    }
    void PurgeOldestRow()
    {
        var row = _rows[0];
        _rows.Remove(row);
        var player = row.gameObject.GetComponentInChildren<CharacterController>();
        if (player != null)
        {
            player.transform.parent = transform;
            player.Kill("Fell Off");
        }
        row.Disable();
    }
    private void Update()
    {
        if (!_scrollDirection.Equals(_prevScrollDirection))
        {
            if (_scrollDirection.Equals(_prevScrollDirection.Opposite()))
            {
                _rows.Reverse();
                _prevScrollDirection = _scrollDirection;
            }
            else
                _scrollDirection = _prevScrollDirection;
        }
        Scroll();
        CheckIfSpawnNew();
        void CheckIfSpawnNew()
        {
            float valueInDirection = _rows[0].transform.localPosition.GetValueInDirection(_scrollDirection);
            bool spawnNext = valueInDirection > _visibleLength/2;
            if (!spawnNext) return;
            //Debug.Log($"{_rows[0].gameObject.name} past due\n{valueInDirection} - {_visibleLength}", _rows[0].gameObject);
            PurgeOldestRow();
            AddNewRow(false);
        }
        void Scroll()
        {
            if (!IsScrolling) return;
            playerAboveHalf = SpeedUpThreshold();
            _currentSpeed = GetCurrwntSpeed();
            CurrentScrollAmnount = _currentSpeed * Time.deltaTime * (playerAboveHalf ? _speedUpMultiplier : 1);
            foreach (Row row in _rows)
                row.transform.localPosition += _scrollDirection.ToVector3() * CurrentScrollAmnount;
        }
    }
    float GetCurrwntSpeed()
    {
        return _scrollSpeed.Value + _scrollSpeedMultiplier * Time.time;
    }
    private void Awake()
    {
        //Setting Initial Rows
        _prevScrollDirection = _scrollDirection;
        TypeCount = new int[Enum.GetValues(typeof(Row.RowType)).Length];
        PerlinCount = new int[10];
        _seed = UnityEngine.Random.value * 10;
        int startPatchAmount = 5;
        for(int i = 0; i < _visibleLength; i++)
        {
            AddNewRow(true, i < startPatchAmount);
        }

        //Setting player start position
        _player = FindObjectOfType<CharacterController>();
        var middleRow = _rows[(startPatchAmount/2)].transform;
        Transform middleBlock = middleRow.GetChild((_rowSize-1)/2);
        _player.transform.parent = middleBlock;
        _player.transform.localPosition = Vector3.up;

        //Setup game starting
        IsScrolling = false;
        _player.OnMove += StartGame;
        void StartGame(Direction2D d)
        {
            Row playerRow = _player.GetComponentInParent<Row>();
            if (!playerRow) return;
            int rowNumber = playerRow.GetRowNumber();
            print("Prestart Row: " + rowNumber);
            if (rowNumber < 1) return;
            playerScore = rowNumber;
            _player.OnMove -= StartGame;
            SetScroll(true);
            _player.OnMove += CheckNewRow;
        }
    }
    void CheckNewRow(Direction2D moveDirection)
    {
        Row playerRow = _player.GetComponentInParent<Row>();
        if (!playerRow) return;
        int rowNumber = playerRow.GetRowNumber();
        if (rowNumber <= playerScore) return;
        playerScore = rowNumber;
    }
    public void ToggleScroll() => SetScroll(!IsScrolling);

    public void SetScroll(bool doScroll)
    {
        IsScrolling = doScroll;
    }
    Vector3 GetLocalEndPosition() => _scrollDirection.ToVector3() * (_visibleLength/2);
    Vector3 GetLocalStartPosition() => _scrollDirection.Opposite().ToVector3() * (_visibleLength/2);
    bool SpeedUpThreshold()
    {
        Row playerRow = _player.transform.GetComponentInParent<Row>();
        if(playerRow == null) return false;
        int rowIndex = playerRow.transform.GetSiblingIndex();
        return rowIndex > _visibleLength * _speedUpThreshold - 1;
    }
    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;
        Vector3 startPosition = GetLocalStartPosition() + position;
        Vector3 endPosition = GetLocalEndPosition() + position;

        Vector3 width = _scrollDirection.Rotate().ToVector3() * _rowSize * 1.00001f;
        Vector3 length = _scrollDirection.ToVector3() * _visibleLength;
        Vector3 height = Vector3.up;
        Vector3 offset =Vector3.up * 0.5f;

        Vector3 slowSectionSize = length * _speedUpThreshold + width + height;
        Vector3 fastSectionSize = length * (1- _speedUpThreshold) + width + height;

        Vector3 slowSectionCenter = endPosition - length * _speedUpThreshold / 2 + offset;
        Vector3 fastSectionCenter = startPosition + length * (1 - _speedUpThreshold) / 2 + offset;

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(position, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fastSectionCenter, fastSectionSize);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(slowSectionCenter, slowSectionSize);

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(startPosition, 0.25f);
        //Gizmos.DrawLine(startPosition, startPosition + Vector3.up * 2);
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(endPosition, 0.25f);
        //Gizmos.DrawLine(endPosition, endPosition + Vector3.up * 2);
    }
}
