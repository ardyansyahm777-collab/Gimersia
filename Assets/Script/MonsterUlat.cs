using UnityEngine;

public class MonsterUlat : MonoBehaviour
{
    private GameOverManager gameOverManager;
    public float speed = 2f; // kecepatan maju
    public Transform player; // untuk deteksi tabrakan (opsional)
    private bool canMove = true;

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
        if (!canMove) return; // berhenti kalau tidak boleh bergerak
        // Bergerak terus ke kanan tanpa henti
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("GAME OVER: Monster caught the player!");
            gameOverManager?.ShowGameOver();
        }
        else if (other.CompareTag("Barrier"))
        {
            StopMoving();
            Debug.Log("Monster menabrak barrier dan berhenti (trigger mode)!");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hentikan saat nabrak barrier
        if (collision.gameObject.CompareTag("Barrier"))
        {
            StopMoving();
            Debug.Log("Monster menabrak barrier dan berhenti!");
        }
    }

    public void StopMoving() => canMove = false;
    public void ResumeMoving() => canMove = true;
}
