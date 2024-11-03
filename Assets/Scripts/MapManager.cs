using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class MapManager : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject collectableItemPrefab;
    
    public static readonly Vector2 TileSize = Vector2.one;
    
    private List<CollectableItem> smallDots = new List<CollectableItem>();
    private List<CollectableItem> bigDots = new List<CollectableItem>();
    
    public int width;
    public int height;

    private Tile[,] map;
    List<MapData> allMapData = new List<MapData>();

    private void ReadMapFromJson(string filePath = "Assets/Scripts/Map/MapData.json")
    {
        // Read existing data if the file exists
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            allMapData = JsonUtility.FromJson<AllMapData>(existingJson).allMapData;
        }
    }
    
    public void GenerateMap(out Vector2Int playerStartPosition)
    {
        ReadMapFromJson();
        // random choose an existing map in allMapData 
        MapData mapData = allMapData[UnityEngine.Random.Range(0, allMapData.Count)];  // todo levels
        width = mapData.mapSize.x;
        height = mapData.mapSize.y;
        map = new Tile[width, height];
        
        // generate map tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // CreateTile with mapData
                GameObject tileGo = CreateTile(mapData.grids.Find(g => g.position == new Vector2Int(x, y)).gridName,
                    new Vector2Int(x, y));
                tileGo.transform.SetParent(transform);
                map[x, y] = tileGo.GetComponent<Tile>();

            }
        }
        
        // generate collectable items
        foreach (var collectablePos in mapData.collectables)
        {
            GameObject collectableGo = CreateCollectableItem(CollectableItemType.smallDot, collectablePos);
            CollectableItem smallDot = collectableGo.GetComponent<CollectableItem>();
            smallDot.GridPos = collectablePos;
            smallDots.Add(smallDot);
            Debug.Log("smallDot: " + smallDot.GridPos);
        }
        
        // set player start position
        playerStartPosition = mapData.startLocation;
        
    }

    private GameObject CreateTile(string type, Vector2Int pos)  // todo use TileType instead of string
    {
        switch (type)
        {
            case "Floor":
                return Instantiate(floorPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            case "Wall":
                return Instantiate(wallPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            default:
                return null;
        }
    }


    public GameObject CreateCollectableItem(CollectableItemType type, Vector2Int pos)
    {
        GameObject collectableGo =
            Instantiate(collectableItemPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        return collectableGo;
    }
    
    public bool IsInMap(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    
    public bool CheckPickUpCollectable(Vector2 characterPos, float radius = 0.1f)
    {
        foreach (var smallDot in smallDots)
        {
            if (Vector2.Distance(characterPos, smallDot.transform.position) < radius)
            {
                smallDots.Remove(smallDot);
                Destroy(smallDot.gameObject);
                return true;
            }
        }

        return false;
    }
    
    public int GetRemainingCollectableCount()
    {
        return smallDots.Count;
    }


}
