using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target; // Player object to follow
    public Vector3 offset; // Offset from the target
    public Vector2 boundsCenter; // Center of boundary restrictions
    public Vector2 boundsSize; // Size of boundary restrictions
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement

    private void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target not set.");
            return;
        }

        // Calculate the desired position for the camera
        Vector3 desiredPosition = target.position + offset;

        // Apply boundary restrictions
        Vector2 minBounds = boundsCenter - boundsSize / 2f;
        Vector2 maxBounds = boundsCenter + boundsSize / 2f;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    // Method to set the target the camera will follow
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Method to set the boundary restrictions
    public void SetBounds(Vector2 center, Vector2 size)
    {
        boundsCenter = center;
        boundsSize = size;
    }
}
