using UnityEngine;

public class TennisBall : MonoBehaviour
{
    private ScoreManager scoreManager;
    private Animator animator;

    public GameObject playerimg1, playerimg2, playerEffect1, playerEffect2;

    private int lastPlayerID = 0;
    private int groundHitCount = 0;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        int playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        int botCount = GameObject.FindGameObjectsWithTag("Bot").Length;

        PlayerDetails playerDetails = collision.gameObject.GetComponent<PlayerDetails>();
        if (playerDetails != null)
        {
            lastPlayerID = playerDetails.playerID;
            groundHitCount = 0;
        }
        else if (collision.transform.CompareTag("Ground"))
        {
            if (lastPlayerID > 0)
            {
                groundHitCount++;
                if (groundHitCount == 2)
                {
                    if (lastPlayerID == 1)
                    {
                        Debug.Log("player1 win");
                    }
                    else if (lastPlayerID == 2)
                    {
                        Debug.Log("player2 win");
                    }
                    lastPlayerID = 0;
                    groundHitCount = 0;
                }
            }
        }

        if (collision.transform.CompareTag("WallA"))
        {
            if (playerCount == 2)
            {
                scoreManager.AddScorePlayer2(1);
                RespawnAt("BallPointB");
                playerimg2.SetActive(true);
                playerEffect2.SetActive(true);
            }
            else if (playerCount == 1 && botCount == 1)
            {
                RespawnAt("BallPointA");
            }
        }
        else if (collision.transform.CompareTag("WallB"))
        {
            if (playerCount == 2)
            {
                scoreManager.AddScorePlayer1(1);
                RespawnAt("BallPointA");
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Serve2P");
                playerimg1.SetActive(true);
                playerEffect1.SetActive(true);
            }
            else if (playerCount == 1 && botCount == 1)
            {
                RespawnAt("BallPointA");
            }
        }

        scoreManager.HandleGameStatus(playerCount, botCount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("SpeedBoostCircles"))
        {
            animator.Play("onFire");
        }
    }

    void RespawnAt(string pointTag)
    {
        GameObject respawnPoint = GameObject.FindGameObjectWithTag(pointTag);
        if (respawnPoint != null)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            transform.position = respawnPoint.transform.position;

            TrailRenderer trail = GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.Clear();
            }
        }
        else
        {
            Debug.LogError("Respawn point with tag " + pointTag + " not found.");
        }
    }
}
