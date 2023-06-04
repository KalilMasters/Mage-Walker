using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public Enemy slowEnemy, fastEnemy;
    private Transform enemyParent;
    public int AliveCount => enemyParent.childCount;

    public Enemy SpawnEnemy(Row row)
    {
        var freeSpaces = row.GetFreeSpaces().Item1;
        int tileIndex = Random.Range(0, freeSpaces.Count);

        Vector3 tilePosition = row.GetLocationAtIndex(tileIndex, false);

        var prefab = Random.value > 0.5 ? fastEnemy : slowEnemy;
        tilePosition += prefab.YOffset * Vector3.up;

        Enemy enemy = Instantiate(prefab, tilePosition,  Quaternion.identity);
        enemy.transform.parent = enemyParent;

        return enemy;
    }
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        //if (aliveEnemies.Contains(enemy)) return;
        //aliveEnemies.Add(enemy);
    }
    public void UnRegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        //if (!aliveEnemies.Contains(enemy)) return;
        //aliveEnemies.Remove(enemy);
    }

    void OnNewChunk(bool isStartChunk, Chunk chunk)
    {
        if (isStartChunk) return;

        foreach(Row row in chunk.Rows)
        {
            if (!row.type.Equals(Row.RowType.Water)) continue;
            if (Random.value < 0.5f) continue;

            SpawnEnemy(row);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        enemyParent = new GameObject("Enemys").transform;
        enemyParent.parent = transform;

        MapScroller.Instance.AddScrollParent(enemyParent);

        MapGenerator.Instance.OnNewChunk.AddListener(OnNewChunk);
    }
}