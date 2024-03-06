using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3 offset; // Offset from the target
    public Vector2 boundsCenter; // Center of boundary restrictions
    public Vector2 boundsSize; // Size of boundary restrictions
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement
    public float minBoundsCenter = 20f; // Minimum bounds center value
    public float maxBoundsCenter = 50f; // Maximum bounds center value

    private GameObject[] players; // Array to store player objects
    private Vector3 desiredPosition; // Desired position for the camera

    void FixedUpdate()
    {
        // Check if there are at least two players
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length < 2)
        {
            Debug.LogWarning("Need at least two players with the 'Player' tag.");
            return;
        }

        // Convert GameObject array to Transform array
        Transform[] playerTransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerTransforms[i] = players[i].transform;
        }

        // Calculate the center position between players
        Vector3 centerPosition = (playerTransforms[0].position + playerTransforms[1].position) / 2f;
        boundsCenter.y = Mathf.Lerp(minBoundsCenter, maxBoundsCenter, Vector3.Distance(playerTransforms[0].position, playerTransforms[1].position) / 50f);

        // Calculate the desired position for the camera
        desiredPosition = centerPosition + offset;

        // Apply boundary restrictions
        Vector2 minBounds = boundsCenter - boundsSize / 2f;
        Vector2 maxBounds = boundsCenter + boundsSize / 2f;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
