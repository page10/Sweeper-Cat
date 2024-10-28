using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLocation : MonoBehaviour
{
        public MapPosition grid;
        public StartLocation(Vector2Int pos)
        {
            grid.pos = pos;
        }
}
