using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    private Vector3 playerStartPos;
    private Vector3 monsterStartPos;
    private GameObject player;
    private GameObject monster;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Temukan player dan monster di scene
        player = GameObject.FindGameObjectWithTag("Player");
        monster = GameObject.FindGameObjectWithTag("Monster");

        // Simpan posisi awal
        if (player != null)
            playerStartPos = player.transform.position;
        if (monster != null)
            monsterStartPos = monster.transform.position;
    }

    public void ShowGameOver()
    {
        Debug.Log("ShowGameOver() dipanggil!");
        if (gameOverPanel == null)
        {
            Debug.LogWarning("GameOverPanel belum diisi di Inspector!");
            return;
        }
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void ReturnToStart()
    {
        Time.timeScale = 1f;

        // Reset posisi player dan monster
        if (player != null)
            player.transform.position = playerStartPos;
        if (monster != null)
            monster.transform.position = monsterStartPos;

        // Sembunyikan panel
        gameOverPanel.SetActive(false);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
