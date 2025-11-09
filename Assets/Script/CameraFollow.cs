using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -10);
    public bool isLocked = false;
    public Vector3 lockPosition;
    public float smoothTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    [Header("Zoom Settings")]
    public float normalZoom = 5f;
    public float bossZoom = 7f;
    public float zoomSmoothSpeed = 2f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition;

        if (isLocked)
        {
            // Jika kamera dikunci, tetap di posisi lock
            desiredPosition = new Vector3(lockPosition.x, lockPosition.y, offset.z);
        }
        else
        {
            // Kamera mengikuti target secara halus
            desiredPosition = new Vector3(
                target.position.x + offset.x,
                transform.position.y,
                offset.z
            );
        }

        // Gerakkan kamera dengan smooth
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Smooth zoom
        float targetZoom = isLocked ? bossZoom : normalZoom;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSmoothSpeed);
    }

    public void LockCamera(Vector3 position)
    {
        lockPosition = position;
        isLocked = true;
    }

    public void UnlockCamera()
    {
        isLocked = false;
    }
}
