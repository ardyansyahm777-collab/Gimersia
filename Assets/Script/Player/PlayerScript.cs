using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2;

    [Header("Health Settings")]
    public HealthUIManager healthUI;
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Attack Settings")]
    public GameObject playerProjectilePrefab;
    public Transform firePoint;
    public int attackDamage = 1;

    // --- BARU: Pengaturan Jeda Tembak ---
    [Tooltip("Waktu jeda antar tembakan, dalam detik")]
    public float fireRate = 0.5f; // Jeda 0.5 detik
    private float nextFireTime = 0f; // Penanda kapan boleh menembak lagi
    // --- AKHIR BAGIAN BARU ---

    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private int jumpCount;
    private bool isGrounded;
    private GameOverManager gameOverManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        gameOverManager = FindObjectOfType<GameOverManager>();
    }

    void Update()
    {
        // Gerak kiri-kanan
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Logika Arah Hadap
        if (moveInput > 0)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Lompat / double jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }

        // --- UBAH: Logika Tembak ---
        // 1. Ganti ke GetMouseButton(0) agar bisa ditahan
        // 2. Tambahkan cek jeda waktu 'Time.time > nextFireTime'
        if (Input.GetMouseButton(0) && Time.time > nextFireTime)
        {
            // Setel ulang timer jeda
            nextFireTime = Time.time + fireRate;

            // Panggil fungsi tembak (saya pindahkan dari Opsi B sebelumnya)
            Shoot();
        }
        // --- AKHIR BAGIAN UBAH ---
    }

    // --- FUNGSI TEMBAK (Opsi B - 8 Arah) ---
    void Shoot()
    {
        if (playerProjectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Player projectile prefab atau fire point belum di-set di Inspector!");
            return;
        }

        Vector2 direction = Vector2.zero;

        // Baca input vertikal dan horizontal
        direction.y = Input.GetKey(KeyCode.W) ? 1 : 0;
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0)
        {
            direction.x = horizontalInput > 0 ? 1 : -1;
        }
        else // Jika tidak menekan A/D
        {
            if (direction.y > 0) // Jika menekan W
            {
                direction.x = 0; // Tembak lurus ke atas
            }
            else // Jika tidak menekan W
            {
                direction.x = isFacingRight ? 1 : -1; // Tembak sesuai arah hadap
            }
        }

        // Normalisasi agar tembakan diagonal tidak lebih cepat
        direction.Normalize();

        // Jangan tembak jika tidak ada arah (misal: hanya berdiri)
        if (direction == Vector2.zero)
        {
            // Jika tidak ada input, tembak lurus sesuai arah hadap
            direction.x = isFacingRight ? 1 : -1;
        }

        // Buat proyektil
        GameObject projectile = Instantiate(playerProjectilePrefab, firePoint.position, Quaternion.identity);

        // Inisialisasi proyektil
        PlayerProjectile projScript = projectile.GetComponent<PlayerProjectile>();
        if (projScript != null)
        {
            projScript.Initialize(direction, attackDamage);
        }
    }
    // --- AKHIR FUNGSI TEMBAK ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // --- SISTEM HP (Tidak Berubah) ---
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player terkena serangan! HP tersisa: {currentHealth}");

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player mati — Game Over!");
            gameOverManager?.ShowGameOver();
            gameObject.SetActive(false); // Matikan player
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
        gameObject.SetActive(true);
    }
}