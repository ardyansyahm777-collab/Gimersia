using UnityEngine;

public class TrapActivator : MonoBehaviour
{
    public GameObject barrier;
    private bool isActivated = false;
    private Vector3 barrierStartPos;       // posisi awal
    private Rigidbody2D barrierRb;         // rigidbody (kalau ada)

    private void Start()
    {
        if (barrier != null)
        {
            barrierStartPos = barrier.transform.position;
            barrierRb = barrier.GetComponent<Rigidbody2D>();

            // Reset ke keadaan awal
            barrier.SetActive(false);
        }

        isActivated = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;
            if (barrier != null)
            {
                barrier.SetActive(true);
                Debug.Log("Barrier dijatuhkan!");

                // Reset velocity biar jatuh dengan benar
                if (barrierRb != null)
                    barrierRb.velocity = Vector2.zero;
            }
        }
    }

    public void ResetTrap()
    {
        isActivated = false;

        if (barrier != null)
        {
            barrier.SetActive(false);
            barrier.transform.position = barrierStartPos;

            if (barrierRb != null)
            {
                barrierRb.velocity = Vector2.zero;
                barrierRb.angularVelocity = 0f;
            }
        }
    }
}
