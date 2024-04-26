using UnityEngine;
using UnityEngine.InputSystem;

public class ServeJudgement : MonoBehaviour
{
    public Transform ballpointA;
    public Transform ballpointB;
    public GameObject tennisBallPrefab;
    public GameObject playerimg1;
    public GameObject playerimg2;

    private bool isServed = false;

    void Update()
    {
        if (!isServed)
        {
            if (CheckPlayersReady())
            {
                ServeBall();
                isServed = true;
            }
        }
    }

    bool CheckPlayersReady()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        var bots = GameObject.FindGameObjectsWithTag("Bot");

        if (players.Length == 1 && bots.Length == 1)
        {
            return true;
        }

        int player0 = 0, player1 = 0;
        foreach (var player in players)
        {
            var playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                if (playerInput.playerIndex == 0) player0++;
                if (playerInput.playerIndex == 1) player1++;
            }
        }

        return player0 == 1 && player1 == 1;
    }

    void ServeBall()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        var bots = GameObject.FindGameObjectsWithTag("Bot");

        Transform spawnPoint;
        GameObject activePlayerImage;

        if (players.Length == 1 && bots.Length == 1)
        {
            spawnPoint = ballpointA;
            activePlayerImage = playerimg1;
        }
        else
        {
            var playerInput = players[0].GetComponent<PlayerInput>();
            if (playerInput != null && playerInput.playerIndex == 0)
            {
                spawnPoint = ballpointA;
                activePlayerImage = playerimg1;
            }
            else
            {
                spawnPoint = Random.Range(0, 2) == 0 ? ballpointA : ballpointB;
                activePlayerImage = playerimg2;
            }
        }

        activePlayerImage.SetActive(true);
        Instantiate(tennisBallPrefab, spawnPoint.position, Quaternion.identity);
    }


}
