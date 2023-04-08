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


    [SerializeField, Range(0, 1)] private float _speedUpThreshold, _slowDownThreshold;
    [field: SerializeField] public int VisibleLength { get; private set; } = 0;
    [field: SerializeField] public int PlayerScore { get; private set; } = 0;

    [SerializeField] private float _nextStopTime;
    [SerializeField] float distanceFromEnd;
    [SerializeField] private float _playerPercent;
    
    IEnumerator StoppedSection()
    {
        yield return Scroller.ChangeToStop();

        int enemiesToSpawn = 3;
        while(enemiesToSpawn > 0)
        {
            Enemys.SpawnEnemy();
            enemiesToSpawn--;
            yield return null;
        }

        while (Enemys.AliveCount > 0)
            yield return null;

        Scroller.SetScroll(true);

        yield return Scroller.ChangeToNaturalSpeed();

        ScoreSystem.Instance.AddPoints(5);
        _nextStopTime = Time.time + 25f;
    }
    private void Update()
    {
        Vector3 playerPos = Player.transform.position;
        _playerPercent = GetPosition(playerPos);

        if (!IsAboveLimit(playerPos, _slowDownThreshold))
            Scroller.ScrollSpeedType = MapScroller.SpeedType.Slow;
        else if(!IsAboveLimit(playerPos, _speedUpThreshold))
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
        if (rowNumber <= PlayerScore) return;
        PlayerScore = rowNumber;
    }

    public bool IsAboveLimit(Vector3 worldPosition, float limit)
    {
        return GetPosition(worldPosition) > limit;
    }
    public float GetPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        distanceFromEnd = GetLocalEndPosition().GetValueInDirection(Scroller.ScrollDirection) - localPosition.GetValueInDirection(Scroller.ScrollDirection);

        float percentValue = distanceFromEnd / VisibleLength;

        return Mathf.Clamp01(percentValue);
    }
    public Vector3 GetLocalEndPosition() => Scroller.ScrollDirection.ToVector3() * (VisibleLength / 2);
    public Vector3 GetLocalStartPosition() => Scroller.ScrollDirection.Opposite().ToVector3() * (VisibleLength / 2);
    private void OnDrawGizmos()
    {
        //return;

        HandleReferences();
        Direction2D scrollDirection = Scroller.ScrollDirection;
        Vector3 position = transform.position;
        Vector3 endPosition = GetLocalEndPosition() + position;

        Vector3 width = scrollDirection.Rotate().ToVector3() * Generator.RowSize * 1.00001f;
        Vector3 length = scrollDirection.ToVector3() * VisibleLength;
        Vector3 height = Vector3.up;
        Vector3 offset = Vector3.up * 0.5f;

        Vector3 slowSectionLength = length * _slowDownThreshold;
        Vector3 slowSectionSize = slowSectionLength + width + height;

        Vector3 normalSectionLength = length * (_speedUpThreshold - _slowDownThreshold);
        Vector3 normalSectionSize = normalSectionLength + width + height;

        Vector3 fastSectionLength = length * (1 - _speedUpThreshold);
        Vector3 fastSectionSize = fastSectionLength + width + height;

        Vector3 slowSectionCenter = endPosition - (slowSectionLength / 2) + offset;
        Vector3 normalSectionCenter = endPosition - slowSectionLength - (normalSectionLength / 2) + offset;
        Vector3 fastSectionCenter = endPosition - slowSectionLength - normalSectionLength - (fastSectionLength / 2) + offset;

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(position, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fastSectionCenter, fastSectionSize);
        Gizmos.color = Color.white;
        Gizmos.DrawCube(normalSectionCenter, normalSectionSize);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(slowSectionCenter, slowSectionSize);

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(startPosition, 0.25f);
        //Gizmos.DrawLine(startPosition, startPosition + Vector3.up * 2);
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(endPosition, 0.25f);
        //Gizmos.DrawLine(endPosition, endPosition + Vector3.up * 2);
    }
    private void Awake()
    {
        HandleReferences();
        Generator.Init();
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
        //Setting player start position
        var middleRow = Generator.Rows[(Generator.StartPatchAmount / 2)].transform;
        Transform middleBlock = middleRow.GetChild((Generator.RowSize - 1) / 2);
        Player.transform.parent = middleBlock;
        Player.transform.localPosition = Vector3.up;

        //Setup game starting
        Player.OnMove += StartGame;
        void StartGame(Direction2D d)
        {
            Row playerRow = Player.GetComponentInParent<Row>();
            if (!playerRow) return;
            int rowNumber = playerRow.GetRowNumber();
            if (rowNumber < 1) return;
            PlayerScore = rowNumber;
            Player.OnMove -= StartGame;
            Scroller.SetScroll(true);
            Player.OnMove += CheckNewRow;
            _nextStopTime = Time.time + 25f;
        }
    }
}
