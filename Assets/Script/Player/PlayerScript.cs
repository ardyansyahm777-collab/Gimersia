using UnityEngine;
using System.Collections;
using TMPro; // <-- TAMBAHKAN INI (untuk TextMeshPro)
// (atau 'using UnityEngine.UI;' jika Anda pakai Text biasa)

public class PlayerMovement : MonoBehaviour
{
    private Animator _animator;

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
        // 1. Cek 'isDead' di paling atas
        if (isDead) return;

        if (isAttacking)
        {
            // (Opsional) Hentikan gerakan player saat menembak
            rb.velocity = new Vector2(0, rb.velocity.y);

            // Kirim info ke animator (ini penting agar dia tahu harus 'Idle'
            // setelah menembak jika player tidak menekan apa-apa)
            if (anim != null) anim.SetBool("isRunning", false);

            return; // Lewati sisa fungsi Update
        }

        // 2. Baca input gerak HANYA SEKALI
        float moveInput = Input.GetAxisRaw("Horizontal");

        // 3. Terapkan gerak
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 4. Kirim SEMUA info ke Animator
        if (anim != null)
        {
            anim.SetBool("isRunning", moveInput != 0);
            anim.SetBool("isGrounded", isGrounded);
        }

        // 5. Logika Arah Hadap (Flip)
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

        // 6. Logika Lompat (HANYA SATU BLOK YANG BENAR)
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            // Panggil Trigger "isJumping" di Animator
            if (anim != null)
            {
                anim.SetTrigger("isJumping");
            }

            jumpCount++;
            isGrounded = false; // Paksa 'isGrounded' false agar animasi 'Falling' main
        }

        // 7. Logika Tembak
        if (Input.GetMouseButton(0) && Time.time > nextFireTime && currentAmmo > 0)
        {
            if (playerProjectilePrefab != null && firePoint != null)
            {
                nextFireTime = Time.time + fireRate;
                Shoot(); // Panggil fungsi tembak
            }
        }
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

            if (anim != null)
            {
                // 1. Kunci gerakan
                isAttacking = true;
                // 2. Mainkan animasi tembak
                anim.SetTrigger("isShooting");
            }
        }

        if (anim != null)
        {
            anim.SetTrigger("isShooting");
        }

        // --- BARU: Kurangi Ammo dan Update UI ---
        currentAmmo--;
        UpdateAmmoUI();
        // ---
    }

    public void OnShootAnimationComplete()
    {
        isAttacking = false; // Buka kunci gerakan
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

    public void DieInstantly()
    {
        if (isInvincible || isDead) return;

        currentHealth = 0; // Set HP langsung jadi 0
        Debug.Log("Player mati instan!");

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        isDead = true;
        gameOverManager?.ShowGameOver();
        rb.velocity = Vector2.zero;
        playerSprite.enabled = false;
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