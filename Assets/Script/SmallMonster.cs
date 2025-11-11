using UnityEngine;

public class SmallMonster : MonoBehaviour
{
    public enum PatrolType
    {
        Horizontal, // Kanan-Kiri
        Vertical    // Naik-Turun
    }

    [Header("Movement Settings")]
    public PatrolType patrolType = PatrolType.Horizontal;
    public float speed = 2f;
    [Tooltip("Jarak maksimal monster berjalan dari titik awal")]
    public float patrolDistance = 3f;
    private Vector3 startPosition;
    private bool movingPositive = true;

    [Header("Stats")]
    public int health = 1;
    public GameObject deathEffect;

    // Variabel internal
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;

    // --- TAMBAHAN BARU: Referensi ke Animator ---
    private Animator anim;
    // ---

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // --- TAMBAHAN BARU: Ambil komponen Animator ---
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("SmallMonster " + gameObject.name + " tidak memiliki komponen Animator.");
        }
        // ---
    }

    void Update()
    {
        if (isDead) return;

        if (patrolType == PatrolType.Horizontal)
        {
            PatrolHorizontal();
        }
        else // Jika 'Vertical'
        {
            PatrolVertical();
        }
    }

    void PatrolHorizontal()
    {
        if (movingPositive)
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

    void PatrolVertical()
    {
        if (movingPositive)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
            if (transform.position.y >= startPosition.y + patrolDistance)
            {
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
            if (transform.position.y <= startPosition.y - patrolDistance)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        movingPositive = !movingPositive;

        if (patrolType == PatrolType.Horizontal)
        {
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        isDead = true;

        // --- TAMBAHAN BARU DI SINI ---
        // Panggil trigger animasi kematian
        if (anim != null)
        {
            anim.SetTrigger("isDead");
        }

        // Hentikan gerakan horizontal/vertikal sepenuhnya (jika ada Rigidbody)
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Hentikan semua gerakan yang disebabkan Rigidbody
            rb.gravityScale = 1; // Agar jatuh jika sebelumnya 0 (untuk Lalat)
        }

        // Matikan Collider agar bisa jatuh menembus tanah / tidak tabrakan lagi
        if (col != null)
        {
            col.enabled = false;
        }

        // (Opsional) Beri sedikit efek berputar saat jatuh
        if (rb != null)
        {
            float randomTorque = Random.Range(-150f, 150f);
            rb.AddTorque(randomTorque);
        }
        // --- AKHIR TAMBAHAN ---

        // Hancurkan objek setelah durasi animasi dan jatuh
        // Sesuaikan durasi ini agar lebih panjang dari animasi kematian Lalat Anda
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
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