using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;
    public float rotateX = 0f;
    public float rotateY = 0f;
    public float rotateZ = 0f;

    void Update()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position + new Vector3(offsetX, offsetY, offsetZ);
            transform.position = newPosition;

            Quaternion newRotation = Quaternion.Euler(player.rotation.eulerAngles + new Vector3(rotateX, rotateY, rotateZ));
            transform.rotation = newRotation;
        }
    }
}
