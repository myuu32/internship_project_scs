using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour
{
    public PlayerInputManager playerInputManager;

    private void Start()
    {
        var wasdInput = new InputAction(binding: "<Keyboard>/w");
        wasdInput.performed += context => OnControlDetected("Player1");
        wasdInput.Enable();

        var arrowInput = new InputAction(binding: "<Keyboard>/upArrow");
        arrowInput.performed += context => OnControlDetected("Player2");
        arrowInput.Enable();
    }

    private void OnControlDetected(string controlScheme)
    {
        if (playerInputManager.playerCount == 0)
        {
            var playerInput = playerInputManager.JoinPlayer();
            playerInput.SwitchCurrentControlScheme(controlScheme, Keyboard.current);
        }
    }
}
