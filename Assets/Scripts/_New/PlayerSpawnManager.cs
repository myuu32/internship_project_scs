using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform[] spawnLocations;
    public GameObject ground;

    private bool[] locationOccupied;
    private PlayerInputManager playerInputManager;
    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        locationOccupied = new bool[spawnLocations.Length];

        if (playerInputManager != null)
        {
            playerInputManager.onPlayerJoined += OnPlayerJoined;
        }
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("PlayerInput ID: " + playerInput.playerIndex);
        StartCoroutine(AssignSpawnLocation(playerInput));    }

    private IEnumerator AssignSpawnLocation(PlayerInput playerInput)
    {
        yield return new WaitForSeconds(0.2f);

        Collider groundCollider = ground.GetComponent<Collider>();
        float groundHeight = groundCollider.bounds.max.y;

        for (int i = 0; i < spawnLocations.Length; i++)
        {
            if (!locationOccupied[i])
            {
                Vector3 spawnPosition = spawnLocations[i].position;

                if (spawnPosition.y < groundHeight)
                {
                    spawnPosition.y = groundHeight + 0.5f;
                }

                Debug.Log($"Assigning Player {playerInput.playerIndex} to Location {i}");

                playerInput.gameObject.transform.position = spawnPosition;
                if (i == 1)
                {
                    playerInput.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                playerInput.gameObject.GetComponent<PlayerDetails>().playerID = i + 1;
                playerInput.gameObject.GetComponent<PlayerDetails>().startPos = spawnPosition;

                locationOccupied[i] = true;
                break;
            }
        }

        if (!locationOccupied[playerInput.playerIndex])
        {
            Debug.LogWarning("Unable to find a safe spawn location. Consider alternative strategies.");
        }
    }
}
