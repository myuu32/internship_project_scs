using UnityEngine;

public class TennisBallLauncher : MonoBehaviour
{

    public GameObject ballPrefab;
    public GameObject[] launchPoints;
    public Vector3[] launchDirections;
    public float[] launchAngles;
    public float[] launchSpeeds;
    public float gravityScale = 1f;
    public float bounciness = 0.6f;
    public float minAngle = 30f;
    public float maxAngle = 60f;
    public float minSpeed = 10f;
    public float maxSpeed = 20f;
    private bool[] playerNearLaunchPoint;

    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);
        playerNearLaunchPoint = new bool[launchPoints.Length];
    }

    void Update()
    {
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
            return;
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
            for (int i = 0; i < launchPoints.Length; i++)
            {
                if (other.gameObject.transform.IsChildOf(launchPoints[i].transform))
                {
                    playerNearLaunchPoint[i] = true;
                    Debug.Log($"Player entered near launch point {i}.");
                    break;
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
                    break;
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
