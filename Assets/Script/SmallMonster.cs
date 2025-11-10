using UnityEngine;

public class SmallMonster : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    [Tooltip("Jarak maksimal monster berjalan ke kanan/kiri dari titik awal")]
    public float patrolDistance = 3f;
    private Vector3 startPosition;
    private bool movingRight = true;

    [Header("Stats")]
    public int health = 1;
    public GameObject deathEffect;

    // --- VARIABEL BARU ---
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false; // Penanda untuk menghentikan Update
    // ---

    void Start()
    {
        startPosition = transform.position;

        // --- TAMBAHAN BARU ---
        // Ambil komponen fisika
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        // ---
    }

    void Update()
    {
        // --- TAMBAHAN BARU ---
        // Jika sudah mati, jangan lakukan apa-apa (berhenti patroli)
        if (isDead) return;
        // ---

        // Logika Gerak Bolak-Balik (Patroli)
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= startPosition.x + patrolDistance)
            {
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= startPosition.x - patrolDistance)

            {
                Flip();
            }
        }

    }
    // Fungsi untuk membalik arah hadap monster
    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // Fungsi menerima damage (dari peluru player)
    public void TakeDamage(int damage)
    {
        // --- TAMBAHAN BARU ---
        // Jangan terima damage jika sudah mati
        if (isDead) return;
        // ---

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // --- FUNGSI 'DIE' YANG DIMODIFIKASI ---
    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 1. Tandai sebagai mati untuk menghentikan Update()
        isDead = true;

        // 2. Matikan Collider agar bisa jatuh menembus tanah
        if (col != null)
        {
            col.enabled = false;
        }

        // 3. Hentikan gerakan horizontal
        rb.velocity = new Vector2(0, rb.velocity.y);

        // 4. (Opsional) Beri sedikit efek berputar saat jatuh
        float randomTorque = Random.Range(-150f, 150f);
        rb.AddTorque(randomTorque);

        // 5. Hancurkan objek setelah 3 detik (agar hilang di bawah layar)
        Destroy(gameObject, 3f);
    }
    // --- AKHIR PERUBAHAN ---

    // Deteksi tabrakan dengan player (INSTA-KILL)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // --- TAMBAHAN BARU ---
        // Jika sudah mati, jangan bunuh player
        if (isDead) return;
        // ---

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.DieInstantly();
            }
        }
    }
}
