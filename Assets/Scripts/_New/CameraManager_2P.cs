using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager_2P : MonoBehaviour
{
    public Vector3[] offsets;
    public Vector2[] boundsCenters;
    public Vector2[] boundsSizes;
    public float[] smoothSpeeds;
    public float[] minBoundsCenters;
    public float[] maxBoundsCenters;
    public float[] followSpeeds;
    public AnimationCurve[] followCurves;

    public GameObject followTarget;
    public GameObject playerObject;

    private Vector3 desiredPosition;
    private float curveTime = 0f;
    public float player2camRotation;

    void FixedUpdate()
    {
        if (followTarget == null)
        {
            return;
        }

        Vector3 centerPosition = followTarget.transform.position;

        PlayerDetails playerDetails = playerObject.GetComponent<PlayerDetails>();
        int playerID = playerDetails != null ? playerDetails.playerID : 1;

        if (playerDetails != null && playerDetails.playerID == 2)
        {
            transform.rotation = Quaternion.Euler(player2camRotation, 180f, 0f);
        }

        int index = playerID - 1;
        Vector3 offset = offsets[Mathf.Clamp(index, 0, offsets.Length - 1)];
        Vector2 boundsCenter = boundsCenters[Mathf.Clamp(index, 0, boundsCenters.Length - 1)];
        Vector2 boundsSize = boundsSizes[Mathf.Clamp(index, 0, boundsSizes.Length - 1)];
        float smoothSpeed = smoothSpeeds[Mathf.Clamp(index, 0, smoothSpeeds.Length - 1)];
        float minBoundsCenter = minBoundsCenters[Mathf.Clamp(index, 0, minBoundsCenters.Length - 1)];
        float maxBoundsCenter = maxBoundsCenters[Mathf.Clamp(index, 0, maxBoundsCenters.Length - 1)];
        float followSpeed = followSpeeds[Mathf.Clamp(index, 0, followSpeeds.Length - 1)];
        AnimationCurve followCurve = followCurves[Mathf.Clamp(index, 0, followCurves.Length - 1)];

        desiredPosition = centerPosition + offset;
        Vector2 minBounds = boundsCenter - boundsSize / 2f;
        Vector2 maxBounds = boundsCenter + boundsSize / 2f;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        curveTime += Time.fixedDeltaTime * followSpeed;

        float t = followCurve.Evaluate(curveTime);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * t);
        transform.position = smoothedPosition;
    }

    public void UpdateCameraPositionImmediately()
    {
        if (followTarget == null) return;
        Vector3 targetPosition = followTarget.transform.position + offsets[0];
        transform.position = targetPosition;
    }

    public void UpdatePositionForZoom(Vector3 playerPosition)
    {
        Vector3 newCameraPosition = playerPosition + new Vector3(0, 1, -5);
        transform.position = newCameraPosition;
    }

}
