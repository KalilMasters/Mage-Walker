using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    public RowType type;

    [field:  SerializeField] public Tile[] GroundTiles { get; private set; }
    public static int RowsCreated = 0;
    [field: SerializeField] public int RowIndex { get; private set; }


    private int _size;
    private Direction2D _scrollDirection;
    private List<int> _freeSpaces = new();


    public void Init(bool spawnDependents = true)
    {
        if (MapGenerator.Instance == null)
            throw new System.ArgumentNullException("No Map Generator");
        if (MapScroller.Instance == null)
            throw new System.ArgumentNullException("No Map Scroller");

        Init(MapGenerator.Instance.RowSize, MapScroller.Instance.ScrollDirection, spawnDependents);
    }
    public void Init(int size, Direction2D direction, bool spawnDependents = false)
    {
        _size = size;
        _scrollDirection = direction;

        RowIndex = RowsCreated++;

        gameObject.name = type + "," + RowIndex;

        GameObject prefab = Resources.Load<GameObject>("GroundBlocks/" + type.ToString());

        GroundTiles = new Tile[size];

        for (int rowIndex = 0; rowIndex < _size; rowIndex++)
        {
            Vector3 position = GetLocationAtIndex(rowIndex, false);
            GameObject g = Instantiate(prefab, position, Quaternion.identity, transform);
            g.layer = LayerMask.NameToLayer("MoveSpace");
            g.name = type.ToString() + " Tile " + rowIndex;

            Tile t = new(g);
            GroundTiles[rowIndex] = t;

            _freeSpaces.Add(rowIndex);
        }

        if (spawnDependents)
            SpawnDependents();
    }
    public void SetVisualInformation(List<Row> chunkRows)
    {
        int myIndex = chunkRows.IndexOf(this);

        bool isFirstRow = myIndex == 0, isLastRow = myIndex == chunkRows.Count-1;
        Row prevRow = null, nextRow = null;
        if(!isFirstRow)
            prevRow = chunkRows[myIndex-1];
        if(!isLastRow)
            nextRow = chunkRows[myIndex+1];
        
        for(int i = 0; i < GroundTiles.Length; i++)
        {
            Direction2D rotation;
            int visualInfo;

            List <Direction2D> sameSides = new();
            List <Direction2D> differentSides = new();
            if (i == 0)
                differentSides.Add(Direction2D.Left);
            else
                sameSides.Add(Direction2D.Left);

            if (i == _size - 1)
                differentSides.Add(Direction2D.Right);
            else
                sameSides.Add(Direction2D.Right);

            if (isFirstRow || !prevRow.type.Equals(type))
                differentSides.Add(Direction2D.Down);
            else
                sameSides.Add(Direction2D.Down);

            if (isLastRow || !nextRow.type.Equals(type))
                differentSides.Add(Direction2D.Up);
            else
                sameSides.Add(Direction2D.Up);

            if(differentSides.Count == 0)
            {
                rotation = Direction2D.Up;
                visualInfo = 0;
            }
            else if(differentSides.Count == 1)
            {
                rotation = differentSides[0];
                visualInfo = 1;
            }
            else if(sameSides.Count == 1)
            {
                rotation = sameSides[0];
                visualInfo = 3;
            }
            else if(sameSides.Count == 0)
            {
                rotation = Direction2D.Up;
                visualInfo = 5;
            }
            else
            {
                if (differentSides[0].Opposite().Equals(differentSides[1]))
                {
                    rotation = differentSides[0];
                    visualInfo = 4;
                }
                else
                {
                    if (differentSides[0].Rotate().Equals(differentSides[1]))
                        rotation = differentSides[0];
                    else
                        rotation = differentSides[1];
                    visualInfo = 2;
                }
            }

            Tile t = GroundTiles[i];
            t.SetVisualInfo(visualInfo);
            t.SetRotation(rotation);

            t.sameDirections = sameSides;
            t.differentDirections = differentSides;
        }
    }
    private static bool spawnLogsLeft = false;
    private static bool lastWasLog = false;
    private void SpawnDependents()
    {
        if (!GetPrevRowFreeSpaces(out List<int> prevRowFreeSpaces, out RowType prevRowType))
            return;


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
                int numLilyPads = Random.Range(1, (_size * 2) / 3);
                SpawnNonMoversInRow("LilyPad", numLilyPads, 0.5f, SpawnConstraints.InfrontOfAtleastOneWalkable);
                lastWasLog = false;
                return;
            }
            Direction2D d = _scrollDirection.Rotate();
            MoverSpawner spawner = SetUpMoverSpawner("LogDispenser", spawnLogsLeft ? d.Opposite() : d);
            spawner.transform.position += Vector3.up * 0.5f;
            spawnLogsLeft = !spawnLogsLeft;
            lastWasLog = true;
        }
        void Grass()
        {
            int numObstacles = Random.Range(0, _size / 2);
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
            Direction2D side = preSetSide.Equals(Direction2D.None) || preSetSide.Equals(_scrollDirection) || preSetSide.Equals(_scrollDirection.Opposite())? GetRandomSide() : preSetSide;
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
        Vector3 GetSidePosition(Direction2D side) => side.ToVector3() * (_size / 2 + 1);
        Direction2D GetRandomSide()
        {
            bool flip = Random.value > 0.5f;
            Direction2D side = _scrollDirection.Rotate();
            return flip ? side.Opposite() : side;
        }
        void SpawnNonMoversInRow(string nonMoverName, int maxAmount = -1, float heightOffset = 0, SpawnConstraints constraints = SpawnConstraints.None)
        {
            if (maxAmount == 0) return;
            if (maxAmount == -1)
                maxAmount = Random.Range(0, _size);

            int spawnsLeft = maxAmount;
            GameObject prefab = Resources.Load<GameObject>(nonMoverName);
            if (!prefab) return;
            List<int> freeSpacesLeftToCheck = new(_freeSpaces);

           // print("Free spaces Left: " + freeSurfaceSpaces.Count);
            while(spawnsLeft > 0 && freeSpacesLeftToCheck.Count > 0)
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
                    case SpawnConstraints.InfrontOfAtleastOneWalkable:
                        #region Spawn infront of atleast one free space

                        if(spawnsLeft == maxAmount)
                        {
                            spawn = CheckIfPrevSpaceIsWalkable(rowIndex);
                            break;
                        }

                        spawn = Random.value > 0.5f;

                        break;
                        #endregion
                    case SpawnConstraints.AllInfrontOfWalkable:
                        //Only spawn where theres passthrough
                        spawn = CheckIfPrevSpaceIsWalkable(rowIndex);
                        break;
                    case SpawnConstraints.AtleastOnePassThrough:
                        //if its the first one, try to make sure its free
                        if (spawnsLeft == maxAmount)
                        {
                            //if the prev is free, make this one free
                            if (CheckIfPrevSpaceIsWalkable(rowIndex))
                                maxAmount--;
                        }
                        else if (Random.value > 0.5f)
                            spawn = true;
                        break;
                }

                if (!spawn) continue;
                _freeSpaces.Remove(rowIndex);
                Vector3 localPosition = GetLocationAtIndex(rowIndex, true) + Vector3.up * heightOffset;
                SpawnNonMoverAtPosition(prefab, localPosition);

                spawnsLeft--;
            }
            //print("Done");
            bool CheckIfPrevSpaceIsWalkable(int rowIndex)
            {
                bool walkable;
                switch (prevRowType)
                {
                    case RowType.Water:
                        if (prevRowFreeSpaces.Count == _size)
                            walkable = true;
                        else
                            walkable = !prevRowFreeSpaces.Contains(rowIndex);
                        break;
                    case RowType.Grass:
                        walkable = prevRowFreeSpaces.Contains(rowIndex);
                        break;
                    default:
                        walkable = prevRowFreeSpaces.Contains(rowIndex);
                        break;
                }
                return walkable;
            }
        }
        GameObject SpawnNonMoverAtPosition(GameObject prefab, Vector3 localPosition)
        {
            Transform instance = Instantiate(prefab, transform).transform;
            instance.transform.localPosition = localPosition;
            return instance.gameObject;
        }
    }
    public Vector3 GetLocationAtIndex(int index, bool localSpace = true)
    {
        if (index < 0 || index >= _size)
            throw new System.IndexOutOfRangeException("Index is out of range");

        Vector3 pos;
        if (GroundTiles[index] != null)
        {
            Transform t = GroundTiles[index].TileObject.transform;
            if (localSpace)
                pos = t.localPosition;
            else
                pos = t.position;
        }
        else
        {
            pos = _scrollDirection.Rotate().Opposite().ToVector3() * (-_size / 2 + index);
            if (!localSpace)
                pos += transform.position;
        }
        return pos;
    }
    bool GetPrevRowFreeSpaces(out List<int> freeSpaces, out RowType rowType)
    {
        //revisit this could cause problems with new chunk generation
        freeSpaces = new();
        rowType = default;
        int siblingIndex = transform.GetSiblingIndex();
        Row prevRow;
        if (siblingIndex == 0)
        {
            List<Chunk> chunks = MapGenerator.Instance.Chunks;
            if (chunks.Count == 1)
                return false;
            Chunk prevChunk = chunks[chunks.Count - 2];
            prevRow = prevChunk.Rows[^1];
        }
        else if (!transform.parent.GetChild(siblingIndex - 1).TryGetComponent(out prevRow))
            return false;

        var result = prevRow.GetFreeSpaces();
        freeSpaces = result.Item1;
        rowType = result.Item2;
        return true;
    }
    public (List<int>, RowType) GetFreeSpaces() => (new(_freeSpaces), type);
    public int GetRowNumber()
    {
        string rowIndex = gameObject.name.Split(",")[1];
        int rowNumber = System.Convert.ToInt32(rowIndex);
        return rowNumber;
    }
    public void Disable()
    {
        foreach (MoverSpawner spawner in GetComponentsInChildren<MoverSpawner>())
            spawner.RecycleAllMovers();
        GameObject.DestroyImmediate(gameObject);
    }
    public enum RowType { Train, Water, Grass, Road }
    enum SpawnConstraints {AllInfrontOfWalkable, InfrontOfAtleastOneWalkable, AtleastOnePassThrough,None }

    [System.Serializable]
    public class Tile
    {
        [field: SerializeField] public GameObject TileObject { get; private set; }
        private TileVisual _visual;
        [SerializeField] Direction2D direction;
        [SerializeField] int visualIndex;

        public List<Direction2D> sameDirections, differentDirections;
        public Tile(GameObject tileObject)
        {
            TileObject = tileObject;
            TileObject.TryGetComponent(out _visual);
        }
        public void SetRotation(Direction2D d)
        {
            direction = d;
            if (!_visual) return;

            _visual.VisualParent.RotateOnAxis(Vector3.up, d);
        }
        public void SetVisualInfo(int index)
        {
            visualIndex = index;
            if (!_visual) return;
            if (index >= _visual.Visuals.Length)
                index = 0;
            for (int i = 0; i < _visual.Visuals.Length; i++)
                _visual.Visuals[i].SetActive(i == index);
        }
    }
}
