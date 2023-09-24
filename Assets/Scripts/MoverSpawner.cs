using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoverSpawner : MonoBehaviour
{
    private enum SpawnerType { Constant, Recycle}

    private Direction2D MoverDirection;
    [SerializeField] SpawnerType spawnType;
    [SerializeField] private Mover _moverPrefab;
    [SerializeField, Tooltip("Number between 0 and 1")] private FloatContainer _speedVariabilityContainer;
    [SerializeField] private int _totalMoversAllowed;
    [SerializeField] private bool _sameMoverSpeed;

    [SerializeField]private List<Mover> movers = new();
    private float _speedVariability => _speedVariabilityContainer ? _speedVariabilityContainer.Value : 0;
    private float _nextSpawnTime;
    private float _moverSpeed;
    private float _timeBetweenSpawns;
    [SerializeField]private float timeLeft;
    private Vector3 localEndPosition;
    private static Dictionary<string, Queue<Mover>> moverBank = new();
    private static Dictionary<string, Mover> moverPrefabs = new();
    bool isPaused;
    private void Update()
    {
        if (isPaused) return;
        timeLeft = _nextSpawnTime - Time.time;
        if(Time.time > _nextSpawnTime && movers.Count > 0)
        {
            if (movers[0] == null)
            {
                movers.RemoveAt(0);
                return;
            }
            Mover m = movers[0];
            m.transform.localPosition = transform.localPosition;
            m.SetDeactivationPosition(localEndPosition, MoverDirection);
            movers.Remove(m);
            movers.Add(m);
            m.gameObject.SetActive(true);
            m.VariedSpeed = _sameMoverSpeed ? _moverSpeed : GetVariedSpeed(m.BaseSpeed, _speedVariability);
            _nextSpawnTime = GetNextSpawnTime();
            m.OnMoverEnd += OnMoverEnd;
            m.OnFrozen += Pause;
        }
    }
    void Pause(bool on) => isPaused = on;
    private float GetVariedSpeed(float baseSpeed, float variability)
    {
        variability = Mathf.Clamp01(variability);
        float randomValue = Random.value;
        float addition = (1 - randomValue) * _speedVariability;
        return baseSpeed + baseSpeed * addition;
    }
    private float GetNextSpawnTime(bool random = false)
    {
        if (random)
            return Time.time + _timeBetweenSpawns * Random.value;
        return Time.time + _timeBetweenSpawns;
    }
    private void OnDrawGizmos()
    {
        float percent = (_nextSpawnTime - Time.time) / _timeBetweenSpawns;
        Gizmos.color = Color.Lerp(Color.red,Color.green, percent);
        Gizmos.DrawCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, transform.position + MoverDirection.ToVector3() * 2);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + localEndPosition, 0.25f);
    }
    private void OnMoverEnd(Mover m)
    {
        m.OnMoverEnd -= OnMoverEnd;
        m.OnFrozen -= Pause;
        m.gameObject.SetActive(false);
        var player = m.gameObject.GetComponentInChildren<CharacterController>();
        if (player != null)
        {
            player.transform.parent = transform.parent;
            player.Kill(m.gameObject.name);
        }
    }
    

    public void Init(Direction2D d)
    {
        MoverDirection = d;
        _timeBetweenSpawns = Random.Range(2f, 5);
        _nextSpawnTime = GetNextSpawnTime(true);
        if (_sameMoverSpeed)
            _moverSpeed = GetVariedSpeed(_moverPrefab.BaseSpeed, _speedVariability);


        //float halfTravelDistance = Mathf.Abs(transform.localPosition.x); 
        float halfTravelDistance = Mathf.Abs(transform.localPosition.GetValueInDirection(MoverDirection)); 

        localEndPosition = MoverDirection.ToVector3() * (halfTravelDistance + 1f);
        InstMoverPool();
        void InstMoverPool()
        {
            for (int i = 0; i < _totalMoversAllowed; i++)
            {
                Mover newMover = GetNewMover(_moverPrefab.gameObject.name);
                newMover.transform.parent = transform.parent;
                newMover.transform.localPosition = transform.localPosition;
                newMover.direction = MoverDirection;
                movers.Add(newMover);
                newMover.gameObject.SetActive(false);
            }
        }
    }
    public void RecycleAllMovers()
    {
        while (movers.Count > 0)
        {
            var m = movers[0];
            movers.Remove(m);
            RecycleMover(m);
        }
    }
    private static Transform moverHolder;
    private static Mover GetNewMover(string moverName)
    {
        if (!moverBank.ContainsKey(moverName))
            moverBank.Add(moverName, new Queue<Mover>());
        Queue<Mover> moverQueue = moverBank[moverName];
        
        Mover outMover = null;

        while(outMover == null)
        {
            if(moverQueue.Count == 0) 
            {
                Mover newMover = SpawnMover();
                if (newMover == null) return null;
                moverQueue.Enqueue(newMover);
            }
            if (moverQueue.Peek() == null)
                moverQueue.Dequeue();
            else
                outMover = moverQueue.Dequeue();
        }
        outMover.gameObject.SetActive(true);
        outMover.gameObject.name = moverName;
        return outMover;

        Mover SpawnMover()
        {
            if (!moverPrefabs.ContainsKey(moverName))
            {
                var resourceLoaded = Resources.Load<Mover>(moverName);
                moverPrefabs.Add(moverName, resourceLoaded? resourceLoaded : null);
            }
            if (moverPrefabs[moverName] == null) return null;
            Mover instantiatedMover = Instantiate(moverPrefabs[moverName]);
            instantiatedMover.gameObject.SetActive(false);
            return instantiatedMover;
        }
    }
    private static void RecycleMover(Mover oldMover)
    {
        if (oldMover == null) return;
        if (moverHolder == null)
            moverHolder = new GameObject("Mover Holder").transform;
        oldMover.transform.parent = moverHolder;
        oldMover.gameObject.SetActive(false);
        if (!moverBank.ContainsKey(oldMover.gameObject.name))
        {
            GameObject.Destroy(oldMover.gameObject);
            return;
        }
        Queue<Mover> moverQueue = moverBank[oldMover.gameObject.name];
        moverQueue.Enqueue(oldMover);
    }
    private void OnEnable() => Init(MoverDirection);
}
