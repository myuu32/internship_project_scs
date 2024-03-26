using UnityEngine;

public class SpeedBoostCircles : MonoBehaviour
{
    public float speedBoostAmount = 5f;

    public float speed = 5f;
    public float range = 5f;
    public MovementDirection direction = MovementDirection.ForwardBackward;

    private Vector3 startPosition;
    private int directionSign = 1;

    public enum MovementDirection
    {
        ForwardBackward,
        LeftRight
    }

    private void Start()
    {
        startPosition = transform.position;

        if (direction == MovementDirection.LeftRight)
        {
            directionSign = 1;
        }
        else
        {
            directionSign = 1;
        }
    }

    private void Update()
    {
        if (direction == MovementDirection.LeftRight)
        {
            transform.Translate(Vector3.right * directionSign * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.forward * directionSign * speed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, startPosition) >= range)
        {
            directionSign *= -1;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TennisBall"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 ballPosition = other.transform.position;

                Vector3 boostDirection = ballPosition.z > transform.position.z ? Vector3.forward : Vector3.back;

                Vector3 boostVelocity = boostDirection * speedBoostAmount * Time.deltaTime;

                rb.velocity += boostVelocity;

                Debug.Log("SpeedBoost Applied");
            }
        }
    }
}
