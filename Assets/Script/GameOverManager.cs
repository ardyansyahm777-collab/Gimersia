using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel;
    public GameObject gameWinnerPanel;

    private GameObject player;
    private PlayerMovement playerMovement;
    private MonsterUlat monster;
    private TrapActivator[] traps;
    private Vector3 playerStartPos;


    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Cari player dan komponen penting
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerStartPos = player.transform.position;
        }

        // Cari monster jika ada
        GameObject m = GameObject.FindGameObjectWithTag("Monster");
        if (m != null)
            monster = m.GetComponent<MonsterUlat>();

        // Cari semua trap di scene
        traps = FindObjectsOfType<TrapActivator>();
    }

    // === Dipanggil dari PlayerMovement saat HP habis ===
    public void ShowGameOver()
    {
        Debug.Log("Game Over: Player HP habis!");

        if (gameOverPanel == null)
        {
            Debug.LogWarning("⚠️ GameOverPanel belum di-assign di Inspector!");
            return;
        }
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Stop();
        }

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void ShowGameWinner()
    {
        Debug.Log("Win, Bos telah dikalahkan");

        if (gameWinnerPanel == null)
        {
            Debug.LogWarning("⚠️ GameWinnerPanel belum di-assign di Inspector!");
            return;
        }
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Stop();
        }

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }


    // === Tombol “Coba Lagi” ===
    public void ReturnToStart()
    {
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
        ResetAll();
    }

    // === Tombol “Restart Scene” ===
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // === Reset semua elemen penting di scene ===
    private void ResetAll()
    {
        Debug.Log("🔄 Reset posisi, HP, monster, dan trap...");

        // Reset Player
        if (player != null)
        {
            player.transform.position = playerStartPos;
            if (playerMovement != null)
                playerMovement.ResetHealth();
        }

        // Reset Monster
        if (monster != null)
            monster.ResetMonster();

        // Reset semua trap
        if (traps != null)
        {
            foreach (var trap in traps)
                trap.ResetTrap();
        }

        Debug.Log("✅ Semua objek telah direset ke kondisi awal.");
    }
}
