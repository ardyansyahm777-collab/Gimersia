using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;

    private Vector3 playerStartPos;
    private GameObject player;
    private MonsterUlat monster;
    private TrapActivator[] traps; // Tambahan — biar bisa reset semua trap

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Cari player dan monster
        player = GameObject.FindGameObjectWithTag("Player");

        GameObject m = GameObject.FindGameObjectWithTag("Monster");
        if (m != null)
            monster = m.GetComponent<MonsterUlat>();

        // Simpan posisi awal player
        if (player != null)
            playerStartPos = player.transform.position;

        // Temukan semua trap di scene
        traps = FindObjectsOfType<TrapActivator>();
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

        ResetAll(); // Reset semua elemen
        gameOverPanel.SetActive(false);
    }

    private void ResetAll()
    {
        // Reset Player
        if (player != null)
            player.transform.position = playerStartPos;

        // Reset Monster
        if (monster != null)
            monster.ResetMonster();

        // Reset semua Trap
        if (traps != null)
        {
            foreach (var trap in traps)
                trap.ResetTrap();
        }

        Debug.Log("Semua objek telah di-reset ke posisi awal!");
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
