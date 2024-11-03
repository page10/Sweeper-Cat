using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapManager mapManager;
    public Camera mainCamera;
    [SerializeField] private GameObject playerPrefab;
    private Character _character;
    
    
    
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
    }

    // private bool MoveCharacter(Character mover)
    // {
    //     // todo
    // }
}
