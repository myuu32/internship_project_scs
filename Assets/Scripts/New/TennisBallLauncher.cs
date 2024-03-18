using UnityEngine;

public class TennisBallLauncher : MonoBehaviour
{

    public GameObject ballPrefab; // 球的预制体
    public GameObject[] launchPoints; // 发球点数组
    public Vector3[] launchDirections; // 每个发球点的发射方向数组
    public float[] launchAngles; // 每个发球点的发射角度数组
    public float[] launchSpeeds; // 每个发球点的发射速度数组
    public float gravityScale = 1f; // 重力缩放系数
    public float bounciness = 0.6f; // 反弹系数
    public float minAngle = 30f; // 最小发射角度
    public float maxAngle = 60f; // 最大发射角度
    public float minSpeed = 10f; // 最小发射速度
    public float maxSpeed = 20f; // 最大发射速度
    private bool[] playerNearLaunchPoint; // 玩家是否接近每个发球点的标志数组

    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);
        playerNearLaunchPoint = new bool[launchPoints.Length]; // 初始化标志数组
    }

    void Update()
    {
        // 按下空格键从接近的发球点发球
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < launchPoints.Length; i++)
            {
                if (playerNearLaunchPoint[i])
                {
                    Debug.Log($"Launching ball from launch point {i}.");
                    LaunchBall(launchPoints[i]);
                }
                else
                {
                    Debug.Log($"Player not near launch point {i}, no ball launched.");
                }
            }
        }
    }

    void LaunchBall(GameObject launchPoint)
    {
        int index = System.Array.IndexOf(launchPoints, launchPoint);
        if (index == -1)
        {
            Debug.LogError("Launch point index not found.");
            return; // 如果找不到发球点索引，则返回
        }

        GameObject ballInstance = Instantiate(ballPrefab, launchPoint.transform.position, Quaternion.identity);
        Rigidbody rb = ballInstance.GetComponent<Rigidbody>();

        PhysicMaterial ballMaterial = new PhysicMaterial
        {
            bounciness = bounciness,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        Collider ballCollider = ballInstance.GetComponent<Collider>();
        if (ballCollider != null)
        {
            ballCollider.material = ballMaterial;
        }

        Vector3 adjustedLaunchDirection = Quaternion.AngleAxis(-launchAngles[index], Vector3.right) * launchDirections[index].normalized * launchSpeeds[index];
        rb.velocity = adjustedLaunchDirection;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 循环遍历launchPoints数组
            for (int i = 0; i < launchPoints.Length; i++)
            {
                // 检查玩家是否触发了任一launchPoints的子Collider
                if (other.gameObject.transform.IsChildOf(launchPoints[i].transform))
                {
                    playerNearLaunchPoint[i] = true;
                    Debug.Log($"Player entered near launch point {i}.");
                    break; // 找到匹配项后跳出循环
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < launchPoints.Length; i++)
            {
                if (other.gameObject.transform.IsChildOf(launchPoints[i].transform))
                {
                    playerNearLaunchPoint[i] = false;
                    Debug.Log($"Player exited from near launch point {i}.");
                    break; // 找到匹配项后跳出循环
                }
            }
        }
    }

    public void EnableLaunch(GameObject launchPoint)
    {
        int index = System.Array.IndexOf(launchPoints, launchPoint);
        if (index != -1)
        {
            Debug.Log($"Enabling launch for point {index}.");
            // 假设 playerNearLaunchPoint 是布尔类型数组，用来追踪每个发球点的状态
            playerNearLaunchPoint[index] = true;
        }
    }

    public void DisableLaunch(GameObject launchPoint)
    {
        int index = System.Array.IndexOf(launchPoints, launchPoint);
        if (index != -1)
        {
            Debug.Log($"Disabling launch for point {index}.");
            playerNearLaunchPoint[index] = false;
        }
    }

}
