using UnityEngine;

public class AimTargetController : MonoBehaviour
{
    public float moveRange = 5f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        Vector3 moveDirection = transform.position - initialPosition;
        if (moveDirection.magnitude > moveRange)
        {
            transform.position = initialPosition + moveDirection.normalized * moveRange;
        }
    }
}
