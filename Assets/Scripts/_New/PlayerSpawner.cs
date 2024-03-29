using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;

    private void Start()
    {
        PlayerInputManager.instance.onPlayerJoined += SpawnPlayer;
    }

    private void OnDestroy()
    {
        PlayerInputManager.instance.onPlayerJoined -= SpawnPlayer;
    }

    private void SpawnPlayer(PlayerInput playerInput)
    {
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
