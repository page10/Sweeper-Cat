using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditor : MonoBehaviour
{
    public Camera usingCam;
    public Transform itemLayer;
    public InputField mWidth;
    public InputField mHeight;
    public InputField mapName;
    public TerrainPattern[] terrainPatterns;

    private List<MapGrid> _grids = new List<MapGrid>();
    private List<Collectables> _collectables = new List<Collectables>();
    private List<EnemyOnEditor> _enemies = new List<EnemyOnEditor>();
    private StartLocation _start;

    private MapEditorCatchingItem _catching = null;
    private bool _mouseDown = false;
    private Vector2 _mouseDownAtScreen = Vector2.zero;
    private Vector3 _camPosBeforeDragged = Vector2.zero;

    private Vector2Int _mapSize = Vector2Int.zero;

    void Start()
    {
        foreach (TerrainPattern pattern in terrainPatterns)
        {
            pattern.Set(CreateHoverItem);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool notOnUI = (!EventSystem.current.IsPointerOverGameObject());
        if (!_mouseDown && Input.GetMouseButtonDown(0) && notOnUI)
        {
            _mouseDownAtScreen = Input.mousePosition;
            _camPosBeforeDragged = usingCam.transform.position;
            _mouseDown = true;

            if (_catching)
            {
                Debug.Log("Catching: " + _catching.pos.transform.position);
                // decide whether the catching item is a grid or a collectable or a start location
                if (_catching.GetComponent<MapGrid>())
                {
                    MapGrid mg = _catching.GetComponent<MapGrid>();
                    // Debug.Log("mg: " + mg.gridName);
                    ChangeGridAt(_catching.pos.transform.position, mg.gridName);
                }
                else if (_catching.GetComponent<Collectables>())
                {
                    Collectables c = _catching.GetComponent<Collectables>();
                    PutCollectableAt(_catching.pos.transform.position, c.collectableName);
                }
                else if (_catching.GetComponent<StartLocation>())
                {
                    if (_start)
                    {
                        Destroy(_start.gameObject);
                        _start = null;
                    }
                    PutStartLocationAt(_catching.pos.transform.position);
                }
                else if (_catching.GetComponent<EnemyOnEditor>())
                {
                    EnemyOnEditor e = _catching.GetComponent<EnemyOnEditor>();
                    PutEnemyAt(_catching.pos.transform.position);
                }
            }
        }
        else if (_mouseDown && Input.GetMouseButtonUp(0))
        {
            _mouseDown = false;
        }
        else if (_mouseDown)
        {
            Vector3 offset = usingCam.ScreenToWorldPoint((Vector3)_mouseDownAtScreen) -
                             usingCam.ScreenToWorldPoint(Input.mousePosition);
            usingCam.transform.position = _camPosBeforeDragged + offset;
        }
        else if (notOnUI && Input.GetMouseButton(1))
        {
            CleanCatching();
        }
    }

    private void CreateHoverItem(string key) 
    {
        CleanCatching();

        GameObject go = Instantiate(Resources.Load<GameObject>("Terrain/" + key));
        MapEditorCatchingItem mci = go.AddComponent<MapEditorCatchingItem>();
        mci.pos = go.GetComponent<MapPosition>();
        mci.transform.SetParent(itemLayer);
        mci.Set((screenPos) =>
        {
            Vector2 worldPos = usingCam.ScreenToWorldPoint(screenPos);
            if (worldPos.x >= -0.5 && worldPos.x <= _mapSize.x - 0.5f && worldPos.y >= -0.5 &&
                worldPos.y <= _mapSize.y - 0.5f)
            {
                mci.transform.position = new Vector3(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y), 0);
            }
            else
            {
                mci.transform.position = worldPos;
            }
        });

        _catching = mci;
    }

    private void CleanCatching()
    {
        if (_catching)
        {
            Destroy(_catching.gameObject);
            _catching = null;
        }
    }

    public void ResizeMap()
    {
        int mapWidth = Math.Clamp(int.Parse(mWidth.text), 5, 32);
        int mapHeight = Math.Clamp(int.Parse(mHeight.text), 5, 32);
        _mapSize = new Vector2Int(mapWidth, mapHeight);
        CleanMap();
        for (int i = 0; i < mapWidth; i++)
        for (int j = 0; j < mapHeight; j++)
            CreateGridAt(i, j);
    }

    private void CleanMap()
    {
        foreach (MapGrid grid in _grids) Destroy(grid.gameObject);
        foreach (Collectables grid in _collectables) Destroy(grid.gameObject);
        foreach (EnemyOnEditor enemy in _enemies) Destroy(enemy.gameObject);
        if (_start)
        {
            Destroy(_start.gameObject);
            _start = null;
        }
        
        _grids.Clear();
        _collectables.Clear();
        _enemies.Clear();
    }

    private void CreateGridAt(Vector2Int g, string gridName = "Floor") => CreateGridAt(g.x, g.y, gridName);

    private void CreateGridAt(int x, int y, string gridName = "Floor")
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Terrain/" + gridName));
        MapGrid mg = go.GetComponent<MapGrid>();
        mg.grid.pos = new Vector2Int(x, y);
        mg.grid.SynchronizeMapPos();
        mg.transform.SetParent(itemLayer);
        _grids.Add(mg);
    }

    private void ChangeGridAt(Vector3 g, string gridName = "Floor") => ChangeGridAt((int)g.x, (int)g.y, gridName);
    private void ChangeGridAt(Vector2Int g, string gridName = "Floor") => ChangeGridAt(g.x, g.y, gridName);

    private void ChangeGridAt(int x, int y, string gridName = "Floor")
    {
        if (x < 0 || x >= _mapSize.x || y < 0 || y >= _mapSize.y) return;
        foreach (MapGrid grid in _grids)
        {
            if (grid.grid.pos == new Vector2Int(x, y))
            {
                Destroy(grid.gameObject);
                _grids.Remove(grid);
                break;
            }
        }

        if (gridName == "Wall")
        {
            Debug.Log("Wall is going to be created");
        }

        GameObject go = Instantiate(Resources.Load<GameObject>("Terrain/" + gridName));
        MapGrid mg = go.GetComponent<MapGrid>();
        mg.grid.pos = new Vector2Int(x, y);
        Debug.Log("mg.grid.pos" + mg.grid.pos);
        mg.grid.SynchronizeMapPos();
        mg.transform.SetParent(itemLayer);
        _grids.Add(mg);
    }

    private void PutCollectableAt(Vector3 g, string collectableName = "Collectable") =>
        PutCollectableAt((int)g.x, (int)g.y, collectableName);

    private void PutCollectableAt(Vector2Int g, string collectableName = "Collectable") =>
        PutCollectableAt(g.x, g.y, collectableName);

    private void PutCollectableAt(int x, int y, string collectableName = "Collectable")
    {
        // if the position is a wall we can't put the collectable on the map
        if (x < 0 || x >= _mapSize.x || y < 0 || y >= _mapSize.y) return;
        foreach (MapGrid grid in _grids)
        {
            if (grid.grid.pos == new Vector2Int(x, y))
            {
                if (grid.gridName == "Wall")
                {
                    Debug.Log("Can't put collectable on wall");
                    return;
                }
            }
        }

        // if the position already has a collectable we can't put another collectable on the map
        foreach (Collectables collable in _collectables)
        {
            if (collable.GetComponent<MapPosition>().pos == new Vector2Int(x, y))
            {
                Debug.Log("Can't put collectable on collectable");
                return;
            }
        }

        GameObject go = Instantiate(Resources.Load<GameObject>("Terrain/" + collectableName));
        Collectables c = go.GetComponent<Collectables>();
        MapPosition mp = go.GetComponent<MapPosition>();

        mp.pos = new Vector2Int(x, y);
        mp.SynchronizeMapPos();

        c.transform.SetParent(itemLayer);
        _collectables.Add(c);
    }

    private void PutStartLocationAt(Vector3 g, string startLocationName = "StartPosition") =>
        PutStartLocationAt((int)g.x, (int)g.y, startLocationName);

    private void PutStartLocationAt(Vector2Int g, string startLocationName = "StartPosition") =>
        PutStartLocationAt(g.x, g.y, startLocationName);

    private void PutStartLocationAt(int x, int y, string startLocationName = "StartPosition")
    {
        // if the position is a wall we can't put the start location on the map
        if (x < 0 || x >= _mapSize.x || y < 0 || y >= _mapSize.y) return;
        foreach (MapGrid grid in _grids)
        {
            if (grid.grid.pos == new Vector2Int(x, y))
            {
                if (grid.gridName == "Wall")
                {
                    Debug.Log("Can't put start location on wall");
                    return;
                }
            }
        }

        // if the position already has a start location we can't put another start location on the map
        if (_start)
        {
            Debug.Log("Can't put start location on start location");
            return;
        }
        
        // cannot put start location on enemy
        foreach (EnemyOnEditor enemy in _enemies)
        {
            if (enemy.GetComponent<MapPosition>().pos == new Vector2Int(x, y))
            {
                Debug.Log("Can't put start location on enemy");
                return;
            }
        }
        
        GameObject go = Instantiate(Resources.Load<GameObject>("Terrain/" + startLocationName));
        StartLocation sl = go.GetComponent<StartLocation>();
        MapPosition mp = go.GetComponent<MapPosition>();

        mp.pos = new Vector2Int(x, y);
        mp.SynchronizeMapPos();

        sl.transform.SetParent(itemLayer);
        _start = sl;
    }
    
    private void PutEnemyAt(Vector3 g, string enemyName = "EnemyPosition") =>
        PutEnemyAt((int)g.x, (int)g.y, enemyName);
    
    private void PutEnemyAt(Vector2Int g, string enemyName = "EnemyPosition") =>
        PutEnemyAt(g.x, g.y, enemyName);

    private void PutEnemyAt(int x, int y, string enemyName = "EnemyPosition")
    {
        // if the position is a wall we can't put the enemy on the map
        if (x < 0 || x >= _mapSize.x || y < 0 || y >= _mapSize.y) return;
        foreach (MapGrid grid in _grids)
        {
            if (grid.grid.pos == new Vector2Int(x, y))
            {
                if (grid.gridName == "Wall")
                {
                    Debug.Log("Can't put Enemy location on wall");
                    return;
                }
            }
        }

        // cannot put enemy on start location
        if (_start && _start.grid.pos == new Vector2Int(x, y))
        {
            Debug.Log("Can't put enemy on start location");
            return;
        }
        
        // if the position already has a enemy we can't put another enemy on the map
        foreach (EnemyOnEditor enemy in _enemies)
        {
            if (enemy.GetComponent<MapPosition>().pos == new Vector2Int(x, y))
            {
                Debug.Log("Can't put enemy on enemy");
                return;
            }
        }

        GameObject go = Instantiate(Resources.Load<GameObject>("Terrain/" + enemyName));
        EnemyOnEditor sl = go.GetComponent<EnemyOnEditor>();
        MapPosition mp = go.GetComponent<MapPosition>();

        mp.pos = new Vector2Int(x, y);
        mp.SynchronizeMapPos();

        sl.transform.SetParent(itemLayer);
        _enemies.Add(sl);
        
    }

    /// <summary>
    /// save the map data to a json file
    /// </summary>
    /// <returns></returns>
    private MapData GatherMapData()
    {
        MapData mapData = new MapData
        {
            mapSize = _mapSize,
            grids = new List<GridData>(),
            collectables = new List<Vector2Int>(),
            startLocation = Vector2Int.zero,
            enemyLocations = new List<Vector2Int>()
        };

        foreach (MapGrid grid in _grids)
        {
            mapData.grids.Add(new GridData
            {
                position = grid.grid.pos,
                gridName = grid.gridName
            });
        }

        foreach (Collectables collectable in _collectables)
        {
            mapData.collectables.Add(collectable.grid.pos);
        }
        
        foreach (EnemyOnEditor enemy in _enemies)
        {
            mapData.enemyLocations.Add(enemy.grid.pos);
        }

        mapData.startLocation = _start? _start.grid.pos : Vector2Int.zero;  
        
        mapData.levelName = mapName.text;

        return mapData;
    }
    
    public void SaveMapData(string filePath = "MapData.json")
    {
        // MapData mapData = GatherMapData();
        // string json = JsonUtility.ToJson(mapData, true);
        // File.WriteAllText(filePath, json);
        
        List<MapData> allMapData = new List<MapData>();
        
        // Read existing data if the file exists
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            allMapData = JsonUtility.FromJson<AllMapData>(existingJson).allMapData;
        }
        
        // Add new map data
        MapData mapData = GatherMapData();
        allMapData.Add(mapData);
        
        // Serialize the updated list back to the file
        string json = JsonUtility.ToJson(new AllMapData{allMapData = allMapData}, true);
        File.WriteAllText(filePath, json);
    }
}