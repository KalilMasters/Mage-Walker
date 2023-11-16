using System;
using System.Collections;
using UnityEngine;
// Kalil is bronze
public class MapManager : MonoBehaviour 
{
    public static MapManager Instance;
    public static bool IsHardMode = true;

    public MapScroller Scroller { get; private set; }
    public MapGenerator Generator { get; private set; }
    public EnemyManager Enemys { get; private set; }
    public CharacterController Player { get; private set; }

    [Header("Game Info")]
    [SerializeField, Range(0, 1)] private float _speedUpThreshold, _slowDownThreshold;
    [SerializeField, Range(-1, 2)] private float _debugPercent;
    [SerializeField] private Vector3 _debugPositionToPercent;
    [field: SerializeField] public int VisibleLength { get; private set; } = 0;
    [field: SerializeField] public int PlayerScore { get; private set; } = 0;

    [SerializeField] private float _nextStopTime;

    [Header("Music")]
    [SerializeField] SoundProfile _normalMusic, _hardMusic;
    
    
    IEnumerator StoppedSection()
    {
        yield return Scroller.ChangeToStop();

        int enemiesToSpawn = 3;
        while(enemiesToSpawn > 0)
        {
            Enemys.SpawnEnemy(GetRandomPosition(true) + Vector3.up);
            enemiesToSpawn--;
            yield return null;
        }

        while (Enemys.AliveCount > 0)
            yield return null;

        yield return Scroller.ChangeToNaturalSpeed();

        ScoreSystem.Instance.AddPoints(5);
        _nextStopTime = Time.time + 25f;
    }
    private void Update()
    {
        Vector3 playerPos = Player.transform.position;

        if (IsAboveLimit(playerPos, _slowDownThreshold))
            Scroller.ScrollSpeedType = MapScroller.SpeedType.Slow;
        else if(IsAboveLimit(playerPos, _speedUpThreshold))
            Scroller.ScrollSpeedType = MapScroller.SpeedType.Normal;
        else
            Scroller.ScrollSpeedType = MapScroller.SpeedType.Fast;

        if (Time.time > _nextStopTime && !Scroller.IsOverrideSpeed && Scroller.IsScrolling)
            StartCoroutine(StoppedSection());
    }

    private void CheckNewRow(Direction2D moveDirection)
    {
        Row playerRow = Player.GetComponentInParent<Row>();
        if (!playerRow) return;
        int rowNumber = playerRow.GetRowNumber();
        PlayerScore = Mathf.Max(PlayerScore, rowNumber);
    }

    public bool IsAboveLimit(Vector3 worldPosition, float limit)
    {
        return GetPercent(worldPosition) > limit;
    }
    public float GetPercent(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        float distanceFromEnd = GetLocalEndPosition().GetValueInDirection(Scroller.ScrollDirection) + localPosition.GetValueInDirection(Scroller.ScrollDirection);

        float percentValue = distanceFromEnd / VisibleLength;

        return percentValue;
    }
    public Vector3 GetRandomPosition(bool forceVisible)
    {
        Vector3 position = GetPosition(UnityEngine.Random.value) +
                Scroller.ScrollDirection.Rotate().Opposite().ToVector3() * (-Generator.RowSize / 2 + UnityEngine.Random.Range(0, Generator.RowSize));
        return position;
    }
    public Vector3 GetPosition(float percent)
    {
        Vector3 end = Scroller.ScrollDirection.ToVector3() * (VisibleLength / 2);
        Vector3 start = Scroller.ScrollDirection.Opposite().ToVector3() * (VisibleLength / 2);
        Vector3 position = Vector3.LerpUnclamped(start, end, percent);
        return position;
    }
    [SerializeField] bool debugHardMode;
    public Vector3 GetLocalEndPosition() => GetPosition(1);
    public Vector3 GetLocalStartPosition() => GetPosition(0);
    public float LengthToPercent(int length)
    {
        return (float)length / VisibleLength;
    }
    private void OnDrawGizmos()
    {
        //return;

        HandleReferences();
        Direction2D scrollDirection = Scroller.ScrollDirection;
        Vector3 position = transform.position;
        Vector3 endPosition = GetLocalEndPosition() + position;

        Vector3 width = scrollDirection.Rotate().ToVector3() * Generator.RowSize * 1.00001f;
        Vector3 length = scrollDirection.ToVector3() * VisibleLength;
        Vector3 height = Vector3.up * 0.05f;
        Vector3 offset = Vector3.up * 0.5f;

        #region SpeedSectionVisuals
        Vector3 slowSectionLength = length * (1 - _slowDownThreshold);
        Vector3 slowSectionSize = slowSectionLength + width + height;

        Vector3 normalSectionLength = length * (_slowDownThreshold - _speedUpThreshold);
        Vector3 normalSectionSize = normalSectionLength + width + height;

        Vector3 fastSectionLength = length * _speedUpThreshold;
        Vector3 fastSectionSize = fastSectionLength + width + height;

        Vector3 slowSectionCenter = endPosition - (slowSectionLength / 2) + offset;
        Vector3 normalSectionCenter = endPosition - slowSectionLength - (normalSectionLength / 2) + offset;
        Vector3 fastSectionCenter = endPosition - slowSectionLength - normalSectionLength - (fastSectionLength / 2) + offset;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fastSectionCenter, fastSectionSize);
        Gizmos.color = Color.white;
        Gizmos.DrawCube(normalSectionCenter, normalSectionSize);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(slowSectionCenter, slowSectionSize);
        #endregion


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetLocalEndPosition() + Vector3.up, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetLocalStartPosition() + Vector3.up, 0.1f);


        Gizmos.color = Color.cyan;
        Vector3 debugPosition = GetPosition(_debugPercent) + Vector3.up;
        Vector3 debugPosition1 = debugPosition + width / 2;
        Vector3 debugPosition2 = debugPosition - width / 2;
        Gizmos.DrawLine(debugPosition1, debugPosition2);
        Gizmos.DrawSphere(debugPosition1, 0.1f);
        Gizmos.DrawSphere(debugPosition2, 0.1f);

        //float debugPositionPercent = GetPercent(_debugPositionToPercent);
        //debugPosition = GetPosition(debugPositionPercent);

        //Gizmos.color = IsAboveLimit(debugPosition, _slowDownThreshold) ? Color.green : Color.red;
        //Gizmos.DrawSphere(debugPosition + Vector3.up, 0.1f);
        //Gizmos.DrawSphere(_debugPositionToPercent, 0.1f);
    }
    private void Awake()
    {
        HandleReferences();
    }
    void HandleReferences()
    {
        Instance = this;

        if(Scroller == null)
            Scroller = GetComponent<MapScroller>();
        if(Generator == null)
            Generator = GetComponent<MapGenerator>();
        if (Enemys == null)
            Enemys = GetComponent<EnemyManager>();

        if (Player == null)
            Player = FindObjectOfType<CharacterController>();
    }
    private void Start()
    {

        Generator.Init();

        //Setting player start position
        Vector3 spawnPosition = Vector3.up + GetPosition(_slowDownThreshold);
        //Player.transform.parent = Player.GetSpaceInDirection(Direction2D.None, spawnPosition).Value.transform;
        Player.transform.position = spawnPosition;

        //Setup game starting

        Player.OnMove += StartGame;


        AudioManager.instance.PlayMusic(IsHardMode? _hardMusic : _normalMusic);


        void StartGame(Direction2D d)
        {
            Row playerRow = Player.GetComponentInParent<Row>();
            if (!playerRow) return;

            if (IsAboveLimit(Player.transform.position, _slowDownThreshold)) return;

            Player.OnMove -= StartGame;
            Scroller.SetScroll(true);
            Player.OnMove += CheckNewRow;
            _nextStopTime = Time.time + 25f;
        }
    }
}
