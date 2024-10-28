using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPattern : MonoBehaviour
{
    [Tooltip("地块prefab在Resources/Terrain/的文件名")] public string mapTileName;

    private Action<string> _onClick;

    public void Click()
    {
        _onClick?.Invoke(mapTileName);
    }

    public void Set(Action<string> onClick)
    {
        _onClick = onClick;
    }
}
