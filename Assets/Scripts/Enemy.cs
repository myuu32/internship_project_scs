using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Basic Settings")]
    public int ID;
    public string Name;
    public string ItemType;
    public Mesh mesh;

    [Header("Movement Settings")]
    [Tooltip("Movement speed of the enemy")]
    public float moveSpeed = 2f;
    [Tooltip("Turn speed of the enemy")]
    public float turnSpeed = 5f;
    [Tooltip("How far the enemy can move from its starting position")]
    [Range(1f, 10f)]
    public float moveDistance = 5f;
    private Vector3 startPosition;
    private bool movingRight = true;

    [Header("Attack Settings")]
    [Tooltip("Prefab of the bullet to shoot")]
    public GameObject bulletPrefab;
    [Tooltip("Point from where the bullet is fired")]
    public Transform firePoint;
    [Tooltip("How often the enemy can shoot (shots per second)")]
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
        // 移動ロジック
        float step = moveSpeed * Time.deltaTime * (movingRight ? 1 : -1);
        transform.position += new Vector3(step, 0, 0);

        if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance)
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
