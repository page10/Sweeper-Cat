using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapManager mapManager;
    public Camera mainCamera;
    [SerializeField] private GameObject playerPrefab;
    private Character _character;
    private MoveDirection _currentMovingDirection = MoveDirection.None;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Generate the map
        mapManager.GenerateMap(out Vector2Int playerStartPosition);
        // Place the player character on the map
        PlacePlayerCharacter(playerStartPosition);
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
    
    // Update is called once per frame
    void Update()
    {
        if (mapManager.GetRemainingCollectableCount() <= 0)
        {
            Debug.Log("You win!");
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

