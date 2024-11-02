using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable] public class MapData
{
    public string levelName;
    public Vector2Int mapSize;
    public List<GridData> grids;
    [FormerlySerializedAs("pickups")] public List<Vector2Int> collectables;  // those dots in pac man
    public Vector2Int startLocation;
}

[Serializable]
public class GridData
{
    public Vector2Int position;
    public string gridName;
}

