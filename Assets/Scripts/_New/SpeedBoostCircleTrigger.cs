using UnityEngine;

public class SpeedBoostCircleTrigger : MonoBehaviour
{
    public float speed = 5f;
    public int score;
    public SpeedBoostCircles speedBoostCircles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TennisBall"))
        {
            TennisBall tennisBall = other.GetComponent<TennisBall>();
            if (tennisBall != null)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 boostDirection = (other.transform.position - transform.position).normalized;
                    rb.velocity += boostDirection * speed * Time.deltaTime;
                }

                ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
                if (scoreManager != null)
                {
                    if (tennisBall.lastPlayerID == 1)
                    {
                        scoreManager.AddScorePlayer1(score);
                    }
                    else if (tennisBall.lastPlayerID == 2)
                    {
                        scoreManager.AddScorePlayer2(score);
                    }
                }
            }
        }
    }
}
