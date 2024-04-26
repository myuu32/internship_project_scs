using UnityEngine;

public class TennisBall : MonoBehaviour
{
    private ScoreManager scoreManager;
    private Animator animator;

    public GameObject playerimg1, playerimg2, playerEffect1, playerEffect2;

    public int lastPlayerID = 0;
    private int groundHitCount = 0;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        playerEffect1.transform.position = GameObject.FindGameObjectWithTag("Player1Score").transform.position;
        playerEffect2.transform.position = GameObject.FindGameObjectWithTag("Player2Score").transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDetails playerDetails = other.GetComponent<PlayerDetails>();
            if (playerDetails != null)
            {
                lastPlayerID = playerDetails.playerID;
                groundHitCount = 0;
            }
        }
        else if (other.CompareTag("Ground"))
        {
            groundHitCount++;
            if (groundHitCount == 2)
            {
                if (lastPlayerID == 1 || lastPlayerID == 2)
                {
                    Debug.Log("Player " + lastPlayerID + " wins!");
                }
                lastPlayerID = 0;
                groundHitCount = 0;
            }
        }
        else if (other.CompareTag("SpeedBoostCircles"))
        {
            animator.Play("onFire");

            if (lastPlayerID == 1) playerEffect1.SetActive(true);
            else if (lastPlayerID == 2) playerEffect2.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("WallA"))
        {
            if (GameObject.FindGameObjectWithTag("Bot") != null)
            {
                RespawnAt("BallPointA");
            }
            else
            {
                scoreManager.AddScorePlayer2(1);
                RespawnAt("BallPointB");
                playerimg2.SetActive(true);
                playerEffect2.SetActive(true);
            }
        }
        else if (collision.gameObject.CompareTag("WallB"))
        {
            scoreManager.AddScorePlayer1(1);
            RespawnAt("BallPointA");
            playerimg1.SetActive(true);
            playerEffect1.SetActive(true);
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
