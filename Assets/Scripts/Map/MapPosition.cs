using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPosition : MonoBehaviour
{
    [Tooltip("position in grids")] public Vector2Int pos;
    
    // Set world position by grid position
    public void SynchronizeMapPos()
    {
        transform.position = new Vector3(pos.x, pos.y, 0);
    }
    //
    // public static Vector3 GetWorldPosByGrid(int x, int y) => new Vector3(x, y, 0);
    // public static Vector3 GetWorldPosByGrid(Vector2Int g) => GetWorldPosByGrid(g.x, g.y);
}
