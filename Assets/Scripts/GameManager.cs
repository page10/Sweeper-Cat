using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapManager mapManager;
    public Camera mainCamera;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyBlueGhostPrefab;
    [SerializeField] private GameObject enemyRedGhostPrefab;
    private Character _character;
    private MoveDirection _currentMovingDirection = MoveDirection.None;
    private List<Enemy> _enemies = new List<Enemy>();
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Generate the map
        mapManager.GenerateMap(out Vector2Int playerStartPosition);
        // Place the player character on the map
        PlacePlayerCharacter(playerStartPosition);
        CreateEnemies(2);
        // Center the camera on the map
        CenterCamera();
    }
    private void CenterCamera() {
        // Calculate the center point of the map
        float mapCenterX = mapManager.width / 2f - 0.5f;
        float mapCenterY = mapManager.height / 2f - 0.5f;

        // Set the camera's position to the center of the map
        mainCamera.transform.position = new Vector3(mapCenterX, mapCenterY, mainCamera.transform.position.z);
    }
    
    private void PlacePlayerCharacter(Vector2Int position) {
        GameObject go = Instantiate(playerPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
        _character = go.GetComponent<Character>();
        // _character.TryMove(MoveDirection.Down, 0);
        _character.transform.SetParent(transform);
    }
    
    private void PlaceEnemyCharacter(Vector2Int position, EnemyType type) {
        GameObject enemyPrefab = type == EnemyType.BlueGhost ? enemyBlueGhostPrefab : enemyRedGhostPrefab;
        GameObject go = Instantiate(enemyPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
        Enemy e = go.GetComponent<Enemy>();
        e.TryMove(MoveDirection.Down, 0);
        List<MoveDirection> md = new List<MoveDirection> { MoveDirection.Up ,MoveDirection.Down,MoveDirection.Left, MoveDirection.Right};
        e.movingDir = md[Random.Range(0, md.Count)];
        e.transform.SetParent(transform);
        _enemies.Add(e);
    }
    
    private void CreateEnemies(int enemyCount)
    {
        //筛选出格子
        List<Vector2Int> eg = mapManager.EmptyGrids();
        if (_character)
        {
            Vector2Int cg = mapManager.PositionInGrid(_character.transform.position);
            for (int i = -3; i <= 3; i++)
            for (int j = -3; j <= 3; j++)
            {
                Vector2Int g = new Vector2Int(i + cg.x, j + cg.y);
                eg.Remove(g);  //玩家周围3格内不刷怪
            }
        }
        while (eg.Count > enemyCount) eg.RemoveAt(Random.Range(0, eg.Count));
        //刷怪
        foreach (Vector2Int g in eg) 
        {
            EnemyType type = (Random.value > 0.5f) ? EnemyType.BlueGhost : EnemyType.RedGhost;
            PlaceEnemyCharacter(g, type);
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        if (mapManager.GetRemainingCollectableCount() <= 0)
        {
            Debug.Log("You win!");
        }
        
        foreach (Enemy enemy in _enemies)
        {
            if (enemy.Dead) continue;
            //尝试移动
            if (!MoveCharacter(enemy, enemy.movingDir, Time.deltaTime))
            {
                //todo 如果敌人移动失败，就会运行ai，现在先随机换个方向
                List<MoveDirection> md = new List<MoveDirection>
                    { MoveDirection.Up, MoveDirection.Down, MoveDirection.Left, MoveDirection.Right };
                md.Remove(enemy.movingDir);
                enemy.movingDir = md[Random.Range(0, md.Count)];
            }
            //尝试杀死玩家
            if (Mathf.Abs(Vector2.Distance(enemy.transform.position, _character.transform.position)) < enemy.killRange)
            {
                // todo kill
            }
        }


        HandleInput();
        MoveCharacter(_character, _currentMovingDirection, Time.deltaTime);
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.W)) {  // Move up
            _currentMovingDirection = MoveDirection.Up;
        } else if (Input.GetKey(KeyCode.S)) {  // Move down
            _currentMovingDirection = MoveDirection.Down;
        } else if (Input.GetKey(KeyCode.A)) {  // Move left
            _currentMovingDirection = MoveDirection.Left;
        } else if (Input.GetKey(KeyCode.D)) {  // Move right
            _currentMovingDirection = MoveDirection.Right;
        } 
    }

    private bool MoveCharacter(Character mover, MoveDirection dir, float delta)
    {
        // todo
        if (!mover) return false;
        if (dir == MoveDirection.None) return true;
        
        const float squeezeRate = 0.1f;
        float checkOffsetX = squeezeRate * MapManager.TileSize.x * 0.5f;
        float checkOffsetY = squeezeRate * MapManager.TileSize.y * 0.5f;
        float bodyX = mover.bodySize.x * 0.5f;
        float bodyY = mover.bodySize.y * 0.5f;
        Vector3 dest = mover.TryMove(dir, delta);
        
        //根据方向获得具体的要检查的点，如果2个点都可过，则移动生效，这里的squeezeRate是一个挤过去的倍率，是为了手感
        Vector2[] checkPoints = new Vector2[] { Vector2.zero ,Vector2.zero};
        switch (dir)
        {
            case MoveDirection.Up:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.up * bodyY + Vector3.left * checkOffsetX,
                    dest + Vector3.up * bodyY + Vector3.right * checkOffsetX
                };
                dest = new Vector3(mapManager.CenterOfPosition(dest).x, dest.y, dest.z);
                break;
            case MoveDirection.Down:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.down * bodyY + Vector3.left * checkOffsetX,
                    dest + Vector3.down * bodyY + Vector3.right * checkOffsetX
                };
                dest = new Vector3(mapManager.CenterOfPosition(dest).x, dest.y, dest.z);
                break;
            case MoveDirection.Left:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.left * bodyX + Vector3.up * checkOffsetY,
                    dest + Vector3.left * bodyX + Vector3.down * checkOffsetY
                };
                dest = new Vector3(dest.x, mapManager.CenterOfPosition(dest).y, dest.z);
                break;
            case MoveDirection.Right:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.right * bodyX + Vector3.up * checkOffsetY,
                    dest + Vector3.right * bodyX + Vector3.down * checkOffsetY
                };
                dest = new Vector3(dest.x, mapManager.CenterOfPosition(dest).y, dest.z);
                break;
        }
        Vector2Int targetGrid = mapManager.PositionInGrid(dest);
        bool canMove = true;
        
        foreach (Vector2 point in checkPoints)
        {
            if (!mapManager.IsMoveValid(point))
            {
                canMove = false;
                break;
            }
        }
        
        if (canMove)
        {
            mover.transform.position = dest;
            // todo move out of map to another side
            if (dest.x < 0) mover.transform.position = new Vector3(mapManager.width - 1, dest.y, dest.z);
            if (dest.x >= mapManager.width) mover.transform.position = new Vector3(0, dest.y, dest.z);
            if (dest.y < 0) mover.transform.position = new Vector3(dest.x, mapManager.height - 1, dest.z);
            if (dest.y >= mapManager.height) mover.transform.position = new Vector3(dest.x, 0, dest.z);
            
            // pick up item
            if (mover == _character)
            {
                mapManager.CheckEatDots(_character.transform.position);
            }

            return true;
        }
        return false;

    }
    

}

