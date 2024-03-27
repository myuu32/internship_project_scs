using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelect : MonoBehaviour
{
    public GameObject backgroung, gameStatus;
    public GameObject player1, player2;

    private void Update()
    {
        var playerInputs = FindObjectsOfType<PlayerInput>();

        bool player0Exists = false;
        bool player1Exists = false;

        foreach (var playerInput in playerInputs)
        {
            int playerIndex = playerInput.playerIndex;

            if (playerIndex == 0)
            {
                player0Exists = true;
            }
            else if (playerIndex == 1)
            {
                player1Exists = true;
            }
        }

        if (player0Exists && !player1Exists)
        {
            backgroung.SetActive(false);
            player1.SetActive(false);
            player2.SetActive(true);
        }
        else if (player0Exists && player1Exists)
        {
            backgroung.SetActive(false);
            player1.SetActive(false);
            player2.SetActive(false);
            gameStatus.SetActive(true);
        }
        else
        {
            backgroung.SetActive(true);
            player1.SetActive(true);
            player2.SetActive(true);
        }
    }
}
