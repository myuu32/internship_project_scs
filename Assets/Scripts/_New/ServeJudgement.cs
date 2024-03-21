using UnityEngine;
using UnityEngine.InputSystem;

public class ServeJudgement : MonoBehaviour
{
    public Transform ballpointA;
    public Transform ballpointB;
    public GameObject tennisBallPrefab;

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

        if (players.Length == 1 && bots.Length == 1)
        {
            spawnPoint = ballpointA;
        }
        else
        {
            spawnPoint = Random.Range(0, 2) == 0 ? ballpointA : ballpointB;
        }

        Instantiate(tennisBallPrefab, spawnPoint.position, Quaternion.identity);
    }
}
