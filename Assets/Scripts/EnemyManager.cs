using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public Enemy slowEnemy, fastEnemy;
    private Transform enemyParent;
    public int AliveCount => enemyParent.childCount;

    public Enemy SpawnEnemy(bool NewestRow = false)
    {
        int rowIndex;
        List<Row> rows = MapGenerator.Instance.Rows;
        int rowCount = rows.Count;

        if (NewestRow)
            rowIndex = rowCount - 1;
        else
            rowIndex = Random.Range(0, rowCount);

        Row row = rows[rowIndex];
        var freeSpaces = row.GetFreeSpaces().Item1;
        int tileIndex = Random.Range(0, freeSpaces.Count);

        Vector3 tilePosition = row.GetLocationAtIndex(tileIndex, false);


        var prefab = Random.value > 0.5 ? fastEnemy : slowEnemy;
        Enemy enemy = Instantiate(prefab);
        enemy.transform.parent = enemyParent;
        enemy.transform.position = tilePosition + Vector3.up * enemy.YOffset;

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

    void OnNewRow(bool isStartRow, Row row)
    {
        if (isStartRow) return;
        if (!row.type.Equals(Row.RowType.Water)) return;
        if (Random.value < 0.5f) return;

        SpawnEnemy(true);
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

        MapGenerator.Instance.OnNewRow.AddListener(OnNewRow);
    }
}