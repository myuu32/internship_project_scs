using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player Joined: " + playerInput.playerIndex);

        GameObject newPlayer = Instantiate(playerPrefab, new Vector3(playerInput.playerIndex * 2.0f, 0, 0), Quaternion.identity);
        PlayerInput inputComponent = newPlayer.GetComponent<PlayerInput>();
    }
}
