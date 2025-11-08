using UnityEngine;

public class MonsterUlat : MonoBehaviour
{
    private GameOverManager gameOverManager;
    public float speed = 2f; // kecepatan maju
    public Transform player; // untuk deteksi tabrakan (opsional)

    void Start()
    {
        gameOverManager = FindObjectOfType<GameOverManager>();
        // Pastikan monster menghadap arah kanan
        transform.localScale = new Vector3(1, 1, 1);

        // Otomatis cari player (kalau ada)
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        // Bergerak terus ke kanan tanpa henti
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("GAME OVER: Monster caught the player!");
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver();
            }
        }

    }
}
