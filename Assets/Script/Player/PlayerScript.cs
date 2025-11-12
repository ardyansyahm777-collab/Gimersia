using UnityEngine;
using System.Collections;
using TMPro; // (atau 'using UnityEngine.UI;' jika Anda pakai Text biasa)

public class PlayerMovement : MonoBehaviour
{
    // Hapus 'private Animator _animator;' (karena duplikat)

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
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Tooltip("Jumlah ammo saat ini")]
    public int currentAmmo = 10;
    [Tooltip("Jumlah ammo awal saat memulai/reset")]
    public int startingAmmo = 10;
    [Tooltip("UI Text untuk menampilkan jumlah ammo")]
    public TMP_Text ammoText;

    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;
    public float blinkSpeed = 0.1f;
    private bool isInvincible = false;
    private bool isDead = false;
    private SpriteRenderer playerSprite;

    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private int jumpCount;
    private bool isGrounded;

    private GameOverManager gameOverManager;
    private Vector3 originalScale;
    private Animator anim;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        gameOverManager = FindObjectOfType<GameOverManager>();
        playerSprite = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        anim = GetComponent<Animator>();

        currentAmmo = startingAmmo;
        UpdateAmmoUI(); // Update UI (Hanya perlu satu kali)

        if (playerSprite == null)
        {
            Debug.LogError("PlayerMovement butuh SpriteRenderer untuk berkedip!");
        }

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
    }

    // --- FUNGSI UPDATE YANG SUDAH DIBERSIHKAN ---
    void Update()
    {
        // 1. Cek 'isDead' di paling atas
        if (isDead) return;

        // 2. Cek 'isAttacking' (mengunci gerakan)
        if (isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Hentikan gerakan
            if (anim != null) anim.SetBool("isRunning", false);
            return; // Lewati sisa fungsi Update
        }

        // 3. Baca input gerak HANYA SEKALI
        float moveInput = Input.GetAxisRaw("Horizontal");

        // 4. Terapkan gerak
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 5. Kirim SEMUA info ke Animator
        if (anim != null)
        {
            anim.SetBool("isRunning", moveInput != 0);
            anim.SetBool("isGrounded", isGrounded);
        }

        // 6. Logika Arah Hadap (Flip)
        if (moveInput > 0)
        {
            isFacingRight = true;
            transform.localScale = originalScale;
        }
        else if (moveInput < 0)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }

        // 7. Logika Lompat (HANYA SATU BLOK)
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            if (anim != null)
            {
                anim.SetTrigger("isJumping");
            }

            jumpCount++;
            isGrounded = false; // Paksa 'isGrounded' false agar animasi 'Falling' main
        }

        // 8. Logika Tembak
        if (Input.GetMouseButton(0) && Time.time > nextFireTime && currentAmmo > 0)
        {
            if (playerProjectilePrefab != null && firePoint != null)
            {
                nextFireTime = Time.time + fireRate;
                Shoot(); // Panggil fungsi tembak
            }
        }
    }
    // --- AKHIR FUNGSI UPDATE ---

    // --- FUNGSI SHOOT YANG SUDAH DIBERSIHKAN ---
    void Shoot()
    {
        Vector2 direction = Vector2.zero;
        direction.y = Input.GetKey(KeyCode.W) ? 1 : 0;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0)
        {
            direction.x = horizontalInput > 0 ? 1 : -1;
        }
        else
        {
            if (direction.y > 0)
            {
                direction.x = 0;
            }
            else
            {
                direction.x = isFacingRight ? 1 : -1;
            }
        }
        direction.Normalize();
        if (direction == Vector2.zero)
        {
            direction.x = isFacingRight ? 1 : -1;
        }
        GameObject projectile = Instantiate(playerProjectilePrefab, firePoint.position, Quaternion.identity);
        PlayerProjectile projScript = projectile.GetComponent<PlayerProjectile>();

        if (projScript != null)
        {
            projScript.Initialize(direction, attackDamage);

            // Panggil trigger HANYA SATU KALI DI SINI
            if (anim != null)
            {
                isAttacking = true; // 1. Kunci gerakan
                anim.SetTrigger("isShooting"); // 2. Mainkan animasi tembak
            }
        }

        // HAPUS PANGGILAN 'anim.SetTrigger("isShooting")' YANG DUPLIKAT DARI SINI

        currentAmmo--;
        UpdateAmmoUI();
    }
    // --- AKHIR FUNGSI SHOOT ---

    public void OnShootAnimationComplete()
    {
        isAttacking = false; // Buka kunci gerakan
    }

    // --- (OnCollisionEnter2D & OnCollisionExit2D tidak berubah) ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek jika yang kita tabrak adalah "Ground"
        if (collision.collider.CompareTag("Ground"))
        {
            // Cek di mana kita menyentuhnya
            // Kita ambil semua titik kontak
            ContactPoint2D[] contacts = new ContactPoint2D[10]; // (Maks 10 kontak)
            int contactCount = collision.GetContacts(contacts);

            for (int i = 0; i < contactCount; i++)
            {
                // 'contacts[i].normal' adalah arah "dorongan" dari platform
                // (0, 1) berarti platform ada DI BAWAH kita (mendorong kita ke ATAS)

                // Jika normal.y lebih besar dari 0.5 (artinya kita mendarat di atas sesuatu)
                if (contacts[i].normal.y > 0.5f)
                {
                    isGrounded = true;
                    jumpCount = 0;

                    // Kita sudah tahu kita mendarat, tidak perlu cek kontak lain
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // --- (UpdateAmmoUI tidak berubah) ---
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "AMMO: " + currentAmmo;
        }
    }

    // --- FUNGSI TAKE DAMAGE (UNTUK ANIMASI KEMATIAN) ---
    // --- INI ADALAH VERSI YANG SUDAH DIPERBAIKI ---
    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead) return;

        currentHealth -= amount;
        Debug.Log($"Player terkena serangan! HP tersisa: {currentHealth}");

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        // --- INI LOGIKA YANG BENAR ---
        if (currentHealth <= 0)
        {
            // --- JIKA PLAYER MATI ---
            isDead = true;
            Debug.Log("Player mati — Game Over!");

            // 1. Panggil trigger animasi
            if (anim != null)
            {
                anim.SetTrigger("isDead");
            }

            // 2. Tampilkan Game Over
            gameOverManager?.ShowGameOver();
            rb.velocity = Vector2.zero; // Hentikan gerakan
        }
        else
        {
            // --- JIKA PLAYER HANYA TERLUKA ---
            // Hanya mulai berkedip JIKA player TIDAK mati
            StartCoroutine(InvincibilityCoroutine());
        }
        // --- AKHIR PERBAIKAN ---
    }
    // --- AKHIR PERBAIKAN TAKE DAMAGE ---

    // --- FUNGSI DIE INSTANTLY (UNTUK ANIMASI KEMATIAN) ---
    public void DieInstantly()
    {
        if (isInvincible || isDead) return;

        currentHealth = 0;
        Debug.Log("Player mati instan!");

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        isDead = true;

        // 1. Panggil trigger animasi
        if (anim != null)
        {
            anim.SetTrigger("isDead");
        }

        // 2. Tampilkan Game Over
        gameOverManager?.ShowGameOver();
        rb.velocity = Vector2.zero; // Hentikan gerakan
    }

    // --- (Coroutine Kebal tidak berubah) ---
    IEnumerator InvincibilityCoroutine()
    {
        Debug.Log("Player Kebal (Invincible)!");
        isInvincible = true;
        float invincibilityTimer = 0f;
        while (invincibilityTimer < invincibilityDuration)
        {
            playerSprite.enabled = !playerSprite.enabled;
            invincibilityTimer += blinkSpeed;
            yield return new WaitForSeconds(blinkSpeed);
        }
        Debug.Log("Player tidak kebal lagi.");
        playerSprite.enabled = true;
        isInvincible = false;
    }

    // --- (ResetHealth tidak berubah) ---
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
        currentAmmo = startingAmmo;
        UpdateAmmoUI();
        isDead = false;
        isInvincible = false;
        playerSprite.enabled = true;
        gameObject.SetActive(true);
        AmmoPickup[] allPickups = FindObjectsOfType<AmmoPickup>();
        foreach (AmmoPickup pickup in allPickups)
        {
            pickup.ResetPickup();
        }
    }

    // --- (AddAmmo tidak berubah) ---
    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        UpdateAmmoUI();
    }
}