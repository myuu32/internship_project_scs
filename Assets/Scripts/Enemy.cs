using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Basic Settings")]
    public int ID;
    public string Name;
    public string ItemType;
    public Mesh mesh;

    public enum MovementDirection
    {
        LeftRight,
        ForwardBackward
    }

    [Header("Movement Settings")]
    [Tooltip("敵の移動速度")]
    public float moveSpeed = 2f;
    [Tooltip("敵の回転速度")]
    public float turnSpeed = 5f;
    [Tooltip("敵が移動できる最大距離")]
    [Range(1f, 100f)]
    public float moveDistance = 5f;
    private Vector3 startPosition;
    private bool movingRight = true;
    public MovementDirection movementDirection = MovementDirection.LeftRight;

    [Header("Attack Settings")]
    [Tooltip("撃つ弾のプレハブ")]
    public GameObject bulletPrefab;
    [Tooltip("弾が発射されるポイント")]
    public Transform firePoint;
    [Tooltip("敵が攻撃できる頻度（秒間の発射数）")]
    [Range(0.1f, 5f)]
    public float fireRate = 1f;
    private float lastFireTime;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (Time.time - lastFireTime > 1f / fireRate)
        {
            Attack();
            lastFireTime = Time.time;
        }

        Move();
        FaceClosestPlayer();
    }

    void Move()
    {

        float step = moveSpeed * Time.deltaTime;
        Vector3 moveVector;

        switch (movementDirection)
        {
            case MovementDirection.LeftRight:
                moveVector = new Vector3(step * (movingRight ? 1 : -1), 0, 0);
                break;
            case MovementDirection.ForwardBackward:
                moveVector = new Vector3(0, 0, step * (movingRight ? 1 : -1));
                break;
            default:
                moveVector = Vector3.zero;
                break;
        }

        transform.position += moveVector;

        if (movementDirection == MovementDirection.LeftRight &&
            Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance)
        {
            movingRight = !movingRight;
        }
        else if (movementDirection == MovementDirection.ForwardBackward &&
                 Mathf.Abs(transform.position.z - startPosition.z) >= moveDistance)
        {
            movingRight = !movingRight;
        }
    }

    void Attack()
    {
        // 攻撃ロジック
        if (bulletPrefab && firePoint)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void FaceClosestPlayer()
    {
        // 最も近いプレイヤーを向くロジック
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }

        if (closestPlayer != null)
        {
            Vector3 direction = closestPlayer.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }
}
