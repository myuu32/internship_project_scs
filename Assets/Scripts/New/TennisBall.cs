using UnityEngine;

public class TennisBall : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 10f; // 初始速度
    [SerializeField] private float mass = 1f; // 質量
    [SerializeField] private float gravity = 9.81f; // 重力
    [SerializeField] private float restitution = 0.8f; // 反彈係數
    [SerializeField] private float collisionVelocityIncrease = 1.5f; // 碰撞後速度增加量
    [SerializeField] private float maxHeight = 100f; // 最大高度
    [SerializeField] private float heightOffset = 0.5f; // 高度偏移量

    private Rigidbody rb;
    private bool isFalling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        LaunchBall();
    }

    private void LaunchBall()
    {
        rb.velocity = transform.forward * initialSpeed;
        rb.mass = mass;
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        CheckHeight();
    }

    private void ApplyGravity()
    {
        rb.AddForce(Vector3.down * gravity * rb.mass);
    }

    private void CheckHeight()
    {
        if (transform.position.y > maxHeight)
        {
            if (!isFalling)
            {
                rb.velocity = -Vector3.up * initialSpeed * restitution;
                isFalling = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 velocity = rb.velocity;

        // 計算反彈速度
        Vector3 reflectedVelocity = Vector3.Reflect(velocity, normal);
        rb.velocity = reflectedVelocity * restitution;

        // 增加碰撞後的速度
        rb.velocity += collisionVelocityIncrease * normal;
    }
}
