using UnityEngine;

public class TrapActivator : MonoBehaviour
{
    public GameObject barrier;
    private bool isActivated = false;

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
            }
        }
    }
}
