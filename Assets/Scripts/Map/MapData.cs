using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class MapData
{
    public Vector2Int mapSize;
    public List<GridData> grids;
    public List<Vector2Int> pickups;  // those dots in pac man
    public Vector2Int startLocation;
}

[Serializable]
public class GridData
{
    public Vector2Int position;
    public string gridName;
}