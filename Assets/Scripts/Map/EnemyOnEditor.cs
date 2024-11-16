using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnEditor : MonoBehaviour
{
    public MapPosition grid;
    public EnemyOnEditor(Vector2Int pos)
    {
        grid.pos = pos;
    }
}
