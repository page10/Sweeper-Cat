using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    [FormerlySerializedAs("TileName")] public TileType TileType;
    public bool CanPass;
}


public enum TileType
{
    Floor,
    Wall
}