using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    private Vector3 playerStartPos;
    private GameObject player;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStartPos = player.transform.position;
    }

    public void ShowGameOver()
    {
        Debug.Log("ShowGameOver terpanggil!");
        Time.timeScale = 0f; // pause game
        gameOverPanel.SetActive(true);
    }

    public void ReturnToStart()
    {
        Time.timeScale = 1f; // resume game
        player.transform.position = playerStartPos; // reset posisi player
        gameOverPanel.SetActive(false);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
