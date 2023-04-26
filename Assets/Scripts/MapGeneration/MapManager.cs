using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// Kalil is bronze
public class MapManager : MonoBehaviour
{
    public AnimationCurve ValueGraph;
    public int[] TypeCount;
    public int[] PerlinCount;
    public float CurrentScrollAmount;
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
    [HideInInspector] public CharacterController Player;
    [SerializeField] int playerScore = 0;
    private bool isOverrideSpeed;
    private float overrideSpeed;
    public float NextStopTime;
    public Enemy slowEnemy, fastEnemy;
    [SerializeField] bool debugHardMode;
    public static Transform ScrollObjectsParent;
    List<Enemy> aliveEnemies = new();
    public static MapManager Instance;

    public static bool IsHardMode = true;
    void AddNewRow(bool frontLoad = false, bool startRow = false)
    {
        Row.RowType type = startRow ? Row.RowType.Grass : GetNewType();
        GameObject rowGO = new GameObject(type.ToString() + "," + (_index - 2).ToString());
        
        rowGO.transform.parent = transform;
        Vector3 localPos = _scrollDirection.Opposite().ToVector3();
        if (frontLoad)
        {
            localPos *= (transform.childCount - 1);
            localPos += GetLocalEndPosition();
        }
        else
        {
            localPos += _rows[^1].transform.localPosition;
        }


        rowGO.transform.localPosition = localPos;

        Row newRow = rowGO.AddComponent<Row>();
        newRow.type = type;
        newRow.Init(_rowSize, _scrollDirection, !startRow);
        _rows.Add(newRow);
        _index++;
        if (!startRow && !frontLoad &&!type.Equals(Row.RowType.Water) && UnityEngine.Random.value > 0.5f)
            SpawnEnemy(true);
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
        Queue<IDamageable> killed = new(row.gameObject.GetComponentsInChildren<IDamageable>().ToList());
        while (killed.Count > 0)
            killed.Dequeue().Damage("Fell Off", DamageType.InstantDeath);
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
        if(Time.time > NextStopTime && !isOverrideSpeed && IsScrolling)
        {
            print("Stopping");
            StartCoroutine(StoppedSection());
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
            _currentSpeed = GetCurrentSpeed();
            CurrentScrollAmount = _currentSpeed * Time.deltaTime * (playerAboveHalf ? _speedUpMultiplier : 1);
            Vector3 amount = _scrollDirection.ToVector3() * CurrentScrollAmount;
            foreach (Row row in _rows)
                row.transform.localPosition += amount;
            
            foreach(Transform t in ScrollObjectsParent)
                t.transform.localPosition += amount;
        }
    }
    float GetCurrentSpeed()
    {
        if (isOverrideSpeed)
            return overrideSpeed;
        return GetNaturalSpeed();
    }
    float GetNaturalSpeed()
    {
        return _scrollSpeed.Value + _scrollSpeedMultiplier * Time.time;
    }
    private void Awake()
    {
        // For testing purposes
        //IsHardMode = debugHardMode;
        Instance = this;
        ScrollObjectsParent = new GameObject("ScrollObjects").transform;
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
        Player = FindObjectOfType<CharacterController>();
        var middleRow = _rows[(startPatchAmount/2)].transform;
        Transform middleBlock = middleRow.GetChild((_rowSize-1)/2);
        Player.transform.parent = middleBlock;
        Player.transform.localPosition = Vector3.up;

        //Setup game starting
        IsScrolling = false;
        Player.OnMove += StartGame;
        void StartGame(Direction2D d)
        {
            Row playerRow = Player.GetComponentInParent<Row>();
            if (!playerRow) return;
            int rowNumber = playerRow.GetRowNumber();
            if (rowNumber < 1) return;
            playerScore = rowNumber;
            Player.OnMove -= StartGame;
            SetScroll(true);
            Player.OnMove += CheckNewRow;
            NextStopTime = Time.time + 25f;
        }
    }
    void CheckNewRow(Direction2D moveDirection)
    {
        Row playerRow = Player.GetComponentInParent<Row>();
        if (!playerRow) return;
        int rowNumber = playerRow.GetRowNumber();
        if (rowNumber <= playerScore) return;
        playerScore = rowNumber;
    }
    public void ToggleScroll() => SetScroll(!IsScrolling);
    IEnumerator StoppedSection()
    {
        isOverrideSpeed = true;

        yield return ChangeSpeed(0);
        IsScrolling = false;
        int enemiesToSpawn = 3;
        while(enemiesToSpawn > 0)
        {
            SpawnEnemy();
            enemiesToSpawn--;
            yield return null;
        }
        while (aliveEnemies.Count > 0)
            yield return null;
        IsScrolling = true;
        yield return ChangeSpeed(GetNaturalSpeed());
        isOverrideSpeed = false;
        ScoreSystem.Instance.AddPoints(5);
        NextStopTime = Time.time + 25f;
    }
    IEnumerator ChangeSpeed(float newSpeed)
    {
        float percent = 0;
        float currentSpeed = GetCurrentSpeed();
        while (percent < 1)
        {
            percent += Time.deltaTime;
            overrideSpeed = Mathf.Lerp(currentSpeed, newSpeed, percent);
            yield return null;
        }
    }
    void SpawnEnemy(bool NewestRow = false)
    {
        int rowIndex;
        if (NewestRow)
            rowIndex = _rows.Count -1;
        else
            rowIndex = UnityEngine.Random.Range(0, _rows.Count);

        Row row = _rows[rowIndex];
        var freeSpaces = row.GetFreeSpaces().Item1;
        int tileIndex = UnityEngine.Random.Range(0, freeSpaces.Count);

        Vector3 tilePosition = row.GetLocationAtIndex(tileIndex, false);
        var prefab = UnityEngine.Random.value > 0.5 ? fastEnemy : slowEnemy;
        Enemy enemy = Instantiate(prefab);
        enemy.transform.parent = ScrollObjectsParent;
        enemy.transform.position = tilePosition + Vector3.up * enemy.YOffset;
    }
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        if (aliveEnemies.Contains(enemy)) return;
        aliveEnemies.Add(enemy);
    }
    public void UnRegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        if (!aliveEnemies.Contains(enemy)) return;
        aliveEnemies.Remove(enemy);
    }
    public void SetScroll(bool doScroll)
    {
        IsScrolling = doScroll;
    }
    Vector3 GetLocalEndPosition() => _scrollDirection.ToVector3() * (_visibleLength/2);
    Vector3 GetLocalStartPosition() => _scrollDirection.Opposite().ToVector3() * (_visibleLength/2);
    bool SpeedUpThreshold()
    {
        Row playerRow = Player.transform.GetComponentInParent<Row>();
        if(playerRow == null) return false;
        int rowIndex = playerRow.transform.GetSiblingIndex();
        return rowIndex > _visibleLength * _speedUpThreshold - 1;
    }
    private void OnDrawGizmos()
    {
        return;
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
