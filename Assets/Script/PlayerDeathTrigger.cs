using UnityEngine;

public class PlayerDeathTrigger : MonoBehaviour
{
    private GameOverManager gameOverManager;

    void Start()
    {
        gameOverManager = FindObjectOfType<GameOverManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("KillZone"))
        {
            Debug.Log("Player tersentuh objek Monster atau jatuh ke jurang — Game Over!");
            gameOverManager?.ShowGameOver();
        }
    }
}
