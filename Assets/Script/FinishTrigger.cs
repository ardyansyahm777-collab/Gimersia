using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return; // mencegah double trigger

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("Level Complete! Pindah ke scene berikutnya...");
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Kalau belum ada scene selanjutnya, kembali ke awal
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Semua level selesai! Kembali ke Level 1.");
            nextIndex = 0;
        }

        SceneManager.LoadScene(nextIndex);
    }
}
