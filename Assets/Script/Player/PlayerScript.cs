using UnityEngine;
using System.Collections;
using TMPro; // <-- TAMBAHKAN INI (untuk TextMeshPro)
// (atau 'using UnityEngine.UI;' jika Anda pakai Text biasa)

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

    // --- PERUBAHAN DI SINI ---
    [Header("Attack Settings")]
    public GameObject playerProjectilePrefab;
    public Transform firePoint;
    public int attackDamage = 1;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Tooltip("Jumlah ammo saat ini")]
    public int currentAmmo = 10; // Ammo awal
    [Tooltip("Jumlah ammo awal saat memulai/reset")]
    public int startingAmmo = 10;
    [Tooltip("UI Text untuk menampilkan jumlah ammo")]
    public TMP_Text ammoText; // Referensi ke UI Text
    // --- AKHIR PERUBAHAN ---

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        gameOverManager = FindObjectOfType<GameOverManager>();
        playerSprite = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;

        // Atur ammo ke jumlah awal saat game dimulai
        currentAmmo = startingAmmo;
        UpdateAmmoUI(); // Update UI

        if (playerSprite == null)
        {
            Debug.LogError("PlayerMovement butuh SpriteRenderer untuk berkedip!");
        }

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        // --- BARU: Update UI Ammo saat mulai ---
        UpdateAmmoUI();
        // ---
    }

    void Update()
    {
        if (isDead) return;

        // Gerak kiri-kanan
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Logika Arah Hadap

        if (moveInput > 0)
        {
            isFacingRight = true;
            // Atur scale kembali ke scale asli
            transform.localScale = originalScale;
        }
        else if (moveInput < 0)
        {
            isFacingRight = false;
            // Balik sumbu X dari scale asli
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }

        // Lompat / double jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }

        // --- DIUBAH: Logika Tembak ---
        // Sekarang cek cooldown DAN apakah ammo > 0
        if (Input.GetMouseButton(0) && Time.time > nextFireTime && currentAmmo > 0)
        {
            // Cek jika prefab peluru di-set (agar tidak error di level 1)
            if (playerProjectilePrefab != null && firePoint != null)
            {
                nextFireTime = Time.time + fireRate;
                Shoot(); // Panggil fungsi tembak
            }
        }
        // --- AKHIR PERUBAHAN ---
    }

    void Shoot()
    {
        // (Kita pindahkan cek null ke Update, jadi di sini aman)

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
        }

        // --- BARU: Kurangi Ammo dan Update UI ---
        currentAmmo--;
        UpdateAmmoUI();
        // ---
    }

    // --- (FungSI OnCollisionEnter2D & OnCollisionExit2D tidak berubah) ---
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

    // --- BARU: Fungsi untuk Update UI Ammo ---
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "AMMO: " + currentAmmo;
        }
    }
    // ---

    // --- (Sistem HP dan Coroutine Kebal tidak berubah) ---
    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead) return;
        currentHealth -= amount;
        Debug.Log($"Player terkena serangan! HP tersisa: {currentHealth}");
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
        StartCoroutine(InvincibilityCoroutine());
        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log("Player mati — Game Over!");
            gameOverManager?.ShowGameOver();
            rb.velocity = Vector2.zero;
            playerSprite.enabled = false;
        }
    }

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

        // Cari semua objek AmmoPickup di scene dan panggil fungsi Reset-nya
        AmmoPickup[] allPickups = FindObjectsOfType<AmmoPickup>();
        foreach (AmmoPickup pickup in allPickups)
        {
            pickup.ResetPickup();
        }
    }

    // Fungsi ini akan dipanggil oleh script AmmoPickup
    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        UpdateAmmoUI();
    }
    // ---
}