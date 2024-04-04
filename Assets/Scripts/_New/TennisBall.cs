using UnityEngine;

public class TennisBall : MonoBehaviour
{
    private ScoreManager scoreManager;
    private Animator animator;

    public GameObject playerimg1, playerimg2;


    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        int playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        int botCount = GameObject.FindGameObjectsWithTag("Bot").Length;

        if (collision.transform.CompareTag("WallA"))
        {
            if (playerCount == 2)
            {
                scoreManager.AddScorePlayer2(1);
                RespawnAt("BallPointB");
                playerimg2.SetActive(true);
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
