using UnityEngine;
using System.Collections;

public class BossSceneController : MonoBehaviour
{
    public CameraFollow mainCamera;
    public Transform lockPoint;
    public BossController boss;

    [Header("Timing")]
    public float delayBeforeLock = 1f;
    public float delayBeforeBossStart = 1f;

    void Start()
    {
        StartCoroutine(StartBossSequence());
    }

    IEnumerator StartBossSequence()
    {
        yield return new WaitForSeconds(delayBeforeLock);

        // Kunci kamera ke titik boss dan zoom out
        if (mainCamera != null && lockPoint != null)
        {
            mainCamera.LockCamera(lockPoint.position);
            Debug.Log("Camera locked to boss arena");
        }

        yield return new WaitForSeconds(delayBeforeBossStart);

        // Aktifkan boss
        if (boss != null)
        {
            boss.ActivateBoss();
        }
    }
}
