using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    public enum RowType { Train, Water, Grass, Road }

    public RowType type;
    List<int> freeSurfaceSpaces = new();
    Direction2D scrollDirection;
    int size;

    public void Init(int size, Direction2D scrollDirection, bool spawnDependents = false)
    {
        this.scrollDirection = scrollDirection;
        this.size = size;
        //gameObject.name = type.ToString();
        bool kill = type.Equals(RowType.Water);
        //Debug.Log("New " + type + " row");
        for(int x = 0; x < size; x++)
        {
            Vector3 position = GetLocationAtIndex(x,size, this.scrollDirection);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.GetComponent<Renderer>().material.color = GetColor();
            go.transform.parent = transform;
            go.transform.localPosition = position;
            go.layer = LayerMask.NameToLayer("MoveSpace");
            if (!kill) continue;
            var killScript = go.AddComponent<KillScript>();
            killScript.name = "Water";
        }
        for (int i = 0; i < size; i++)
            freeSurfaceSpaces.Add(i);
        if(spawnDependents)
            SpawnDependents();
    }
    private static bool spawnLogsLeft = false;
    private static bool lastWasLog = false;
    void SpawnDependents()
    {
        List<int> prevRowFreeSpaces;
        RowType? prevRowType;
        (prevRowFreeSpaces, prevRowType) = GetPrevRowFreeSpaces();

        switch (type)
        {
            case RowType.Water:
                Water();
                break;
            case RowType.Grass:
                Grass();
                break;
            case RowType.Train:
                Train();
                break;
            case RowType.Road:
                Road();
                break;
        }
        void Water()
        {
            bool spawnLogs = Random.value > 0.5f;
            if (!spawnLogs && lastWasLog)
            {
                int numLilyPads = Random.Range(1, (size*2) / 3);
                SpawnNonMoversInRow("LilyPad", numLilyPads, 0.5f, SpawnConstraints.InfrontOfAtleastOneFree);
                lastWasLog = false;
                return;
            }
            Direction2D d = scrollDirection.Rotate();
            MoverSpawner spawner = SetUpMoverSpawner("LogDispenser", spawnLogsLeft ? d.Opposite() : d);
            spawner.transform.position += Vector3.up * 0.5f;
            spawnLogsLeft = !spawnLogsLeft;
            lastWasLog = true;
        }
        void Grass()
        {
            int numObstacles = Random.Range(0, size / 2);
            SpawnNonMoversInRow("Bush", numObstacles, 1, SpawnConstraints.AtleastOnePassThrough);
        }
        void Train()
        {
            MoverSpawner spawner = SetUpMoverSpawner("TrainDispenser");
            if (spawner == null) return;
            spawner.transform.position += Vector3.up;
        }
        void Road()
        {
            MoverSpawner spawner = SetUpMoverSpawner("CarDispenser");
            if (spawner == null) return;
            spawner.transform.position += Vector3.up;
        }
        
        MoverSpawner SetUpMoverSpawner(string spawnerName, Direction2D preSetSide = Direction2D.None)
        {
            Direction2D side = preSetSide.Equals(Direction2D.None) || preSetSide.Equals(scrollDirection) || preSetSide.Equals(scrollDirection.Opposite())? GetRandomSide() : preSetSide;
            Vector3 sidePosition = GetSidePosition(side);
            MoverSpawner spawner = Resources.Load<MoverSpawner>(spawnerName);
            if (spawner == null)
            {
                Debug.LogWarning($"Spawner of name {spawnerName} does not exist!");
                return null;
            }
            spawner = Instantiate(spawner, transform);
            spawner.transform.localPosition = sidePosition;
            spawner.Init(side.Opposite());
            return spawner;
        }
        Vector3 GetSidePosition(Direction2D side) => side.ToVector3() * (size / 2 + 1);
        Direction2D GetRandomSide()
        {
            bool flip = Random.value > 0.5f;
            Direction2D side = scrollDirection.Rotate();
            return flip ? side.Opposite() : side;
        }
        void SpawnNonMoversInRow(string nonMoverName, int maxAmount = -1, float heightOffset = 0, SpawnConstraints constraints = SpawnConstraints.None)
        {
            if (maxAmount == 0) return;
            if (maxAmount == -1)
                maxAmount = Random.Range(0, size);
            //print("Spawning " + maxAmount + " " + nonMoverName + " " + constraints);
            int spawnsLeft = maxAmount;
            GameObject prefab = Resources.Load<GameObject>(nonMoverName);
            if (!prefab) return;
            List<int> freeSpacesLeftToCheck = new(freeSurfaceSpaces);

           // print("Free spaces Left: " + freeSurfaceSpaces.Count);
            while(spawnsLeft > 0 && freeSpacesLeftToCheck.Count > 1)
            {
                int rowIndex = freeSpacesLeftToCheck[Random.Range(0, freeSpacesLeftToCheck.Count)];
                //Debug.Log($"{maxAmount-spawnsLeft + 1}/{maxAmount} : {rowIndex}", transform.GetChild(rowIndex));
                bool spawn = false;
                freeSpacesLeftToCheck.Remove(rowIndex);

                switch (constraints)
                {
                    case SpawnConstraints.None:
                        spawn = true;
                        break;
                    case SpawnConstraints.InfrontOfAtleastOneFree:
                        if(spawnsLeft == maxAmount)
                            spawn = CheckIfPrevSpaceIsWalkable(rowIndex);
                        else
                            spawn = CheckIfPrevSpaceIsWalkable(rowIndex) || Random.value > 0.5f;
                        break;
                    case SpawnConstraints.ALlInfrontOfFree:
                        spawn = CheckIfPrevSpaceIsWalkable(rowIndex);
                        break;
                    case SpawnConstraints.AtleastOnePassThrough:
                        if (spawnsLeft == maxAmount)
                        {
                            //print("Checking if leave space open");
                            if (CheckIfPrevSpaceIsWalkable(rowIndex))
                            {
                                //Debug.Log($"Leaving space {rowIndex} open", gameObject);
                                maxAmount++;
                            }
                        }
                        else
                        {
                            spawn = true;
                            //print("Already has free space");
                        }
                            
                        break;
                }

                if (!spawn) continue;
                freeSurfaceSpaces.Remove(rowIndex);
                //Debug.Log("Spawning " + nonMoverName + " at " + rowIndex);
                Vector3 localPosition = GetLocationAtIndex(rowIndex, size, scrollDirection) + Vector3.up * heightOffset;
                SpawnNonMoverAtPosition(prefab, localPosition);

                spawnsLeft--;
            }
            //print("Done");
            bool CheckIfPrevSpaceIsWalkable(int rowIndex)
            {
                bool free;
                if (!prevRowType.HasValue) return true;
                switch (prevRowType.Value)
                {
                    case RowType.Water:
                        if (prevRowFreeSpaces.Count == size)
                            free = true;
                        else
                            free = !prevRowFreeSpaces.Contains(rowIndex);
                        break;
                    case RowType.Grass:
                        free = prevRowFreeSpaces.Contains(rowIndex);
                        break;
                    default:
                        free = prevRowFreeSpaces.Contains(rowIndex);
                        break;
                }
                //print($"Prev row {prevRowType} free: " + free);
                return free;
            }
        }
        GameObject SpawnNonMoverAtPosition(GameObject prefab, Vector3 localPosition)
        {
            Transform instance = Instantiate(prefab, transform).transform;
            instance.transform.localPosition = localPosition;
            return instance.gameObject;
        }
    }
    Vector3 GetLocationAtIndex(int rowIndex, int rowSize, Direction2D scrollDirection)
    {
        rowIndex = Mathf.Clamp(rowIndex, 0, rowSize);
        return scrollDirection.Rotate().ToVector3() * (-rowSize / 2 + rowIndex);
    }
    Color GetColor()
    {
        switch (type)
        {
            case RowType.Water:
                return Color.blue;
            case RowType.Grass:
                return Color.green;
            case RowType.Train:
                return Color.grey;
            case RowType.Road:
                return Color.black;
        }
        return Color.red;
    }
    (List<int>, RowType?) GetPrevRowFreeSpaces()
    {
        int siblingIndex = transform.GetSiblingIndex();
        if (siblingIndex == 0) return (new(), null);
        Row prevRow = transform.parent.GetChild(siblingIndex - 1).GetComponent<Row>();
        if(prevRow == null) return (new(), null);
        //Debug.Log("Prev Row: " + prevRow.name, prevRow.gameObject);
        return prevRow.GetFreeSpaces();
    }
    public (List<int>, RowType) GetFreeSpaces() => (new(freeSurfaceSpaces), type);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawCube(transform.position, Vector3.one);
    }
    public int GetRowNumber()
    {
        string rowIndex = gameObject.name.Split(",")[1];
        int rowNumber = System.Convert.ToInt32(rowIndex);
        return rowNumber;
    }
    enum SpawnConstraints {ALlInfrontOfFree, InfrontOfAtleastOneFree, AtleastOnePassThrough,None }
}
