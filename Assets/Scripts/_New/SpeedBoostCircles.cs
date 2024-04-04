using UnityEngine;

public class SpeedBoostCircles : MonoBehaviour
{
    public Vector3 boostDirectionPositive = new Vector3(0, 1, 0);
    public Vector3 boostDirectionNegative = new Vector3(0, -1, 0);

    public float speedBoostAmountPositive = 5f;
    public float speedBoostAmountNegative = 5f;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TennisBall"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 ballPosition = other.transform.position;
                float posX = ballPosition.x;
                float posY = ballPosition.y;
                float posZ = ballPosition.z;

                Vector3 boostDirection;
                float boostAmount;

                if (posX >= transform.position.x)
                {
                    boostDirection = boostDirectionPositive;
                    boostAmount = speedBoostAmountPositive;
                }
                else
                {
                    boostDirection = boostDirectionNegative;
                    boostAmount = speedBoostAmountNegative;
                }

                if (posY >= transform.position.y)
                {
                    boostDirection = boostDirectionPositive;
                    boostAmount = speedBoostAmountPositive;
                }
                else
                {
                    boostDirection = boostDirectionNegative;
                    boostAmount = speedBoostAmountNegative;
                }

                if (posZ >= transform.position.z)
                {
                    boostDirection = boostDirectionPositive;
                    boostAmount = speedBoostAmountPositive;
                }
                else
                {
                    boostDirection = boostDirectionNegative;
                    boostAmount = speedBoostAmountNegative;
                }

                Vector3 boostVelocity = boostDirection.normalized * boostAmount * Time.deltaTime;
                rb.velocity += boostVelocity;

                Debug.Log("SpeedBoost Applied");
            }
        }
    }
}
