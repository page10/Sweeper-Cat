using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public string gridName;
    [Header("其他组件拉一下")]
    public MapPosition grid;
    public SpriteRenderer sprite;
    
    [Header("逻辑数据")]
    [Tooltip("这格能不能过")] public bool canPass;
}
