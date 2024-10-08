using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform[] spawnLocations;
    public GameObject ground;
    public GameObject GameTIme;

    private bool[] locationOccupied;
    private PlayerInputManager playerInputManager;
    private PlayerInput[] playerInputs; // Array to keep track of all players

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        locationOccupied = new bool[spawnLocations.Length];
        playerInputs = new PlayerInput[playerInputManager.maxPlayerCount]; // Initialize the array

        if (playerInputManager != null)
        {
            playerInputManager.onPlayerJoined += OnPlayerJoined;
        }
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("PlayerInput ID: " + playerInput.playerIndex);
        StartCoroutine(AssignSpawnLocation(playerInput));

        // Store the player input in the array for later reference
        playerInputs[playerInput.playerIndex] = playerInput;
    }

    private IEnumerator AssignSpawnLocation(PlayerInput playerInput)
    {
        yield return new WaitForSeconds(0.2f);

        Collider groundCollider = ground.GetComponent<Collider>();
        float groundHeight = groundCollider.bounds.max.y;

        int spawnIndex = FindAvailableSpawnIndex();
        if (spawnIndex != -1)
        {
            Vector3 spawnPosition = spawnLocations[spawnIndex].position;

            if (spawnPosition.y < groundHeight)
            {
                spawnPosition.y = groundHeight + 0.5f;
            }

            Debug.Log($"Assigning Player {playerInput.playerIndex} to Location {spawnIndex}");

            playerInput.gameObject.transform.position = spawnPosition;
            if (spawnIndex == 1)
            {
                playerInput.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                GameTIme.SetActive(true);
            }

            playerInput.gameObject.GetComponent<PlayerDetails>().playerID = spawnIndex + 1;
            playerInput.gameObject.GetComponent<PlayerDetails>().startPos = spawnPosition;

            locationOccupied[spawnIndex] = true;
        }
        else
        {
            Debug.LogWarning("Unable to find a safe spawn location. Consider alternative strategies.");
        }
    }

    private int FindAvailableSpawnIndex()
    {
        for (int i = 0; i < locationOccupied.Length; i++)
        {
            if (!locationOccupied[i])
            {
                return i;
            }
        }
        return -1;
    }
}
