using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -10);
    public float smoothTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return; // ini mencegah error

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            transform.position.y,
            offset.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}
