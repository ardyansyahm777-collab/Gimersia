using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    // === Variabel Inspector ===
    [Header("Boss Settings")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 0.7f;
    public float projectileSpeed = 6f;
    public int projectileDamage = 1;

    [Header("Phase 2: Dash Attack")]
    public float phaseSwitchTime = 10f;
    public float dashSpeed = 20f;
    public GameObject warningIndicator;
    public Transform[] leftDashPoints;
    public Transform[] rightDashPoints;
    public int numberOfDashes = 2;
    public float fadeDuration = 1f;
    public float warningTime = 1.5f;

    // --- BARU: Pengaturan HP Bos ---
    [Header("Boss Health")]
    public int maxHealth = 20;
    private int currentHealth;
    public BossHealthBar healthBar; // Referensi ke script HP Bar
    private GameOverManager gameOverManager; // Untuk pop-up "Game Over"
    // --- AKHIR BAGIAN BARU ---

    // === Variabel Internal ===
    private bool isActive = false;
    private SpriteRenderer bossSprite;
    private Vector3 startPosition;
    private float phaseTimer;
    private enum BossState { Shooting, Dashing }
    private BossState currentState;
    private Animator bossAnimator;

    // =============================================
    // === FUNGSI UTAMA ===
    // =============================================

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        bossAnimator = GetComponent<Animator>();

        startPosition = transform.position;

        // --- BARU: Inisialisasi HP ---
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.gameObject.SetActive(false); // Sembunyikan HP Bar dulu
        }
        else
        {
            Debug.LogError("HealthBar belum di-assign di BossController!");
        }
        // --- AKHIR BAGIAN BARU ---

        if (bossSprite == null)
        {
            Debug.LogError("[Start] GAGAL: Boss Controller butuh SpriteRenderer!");
        }
        if (bossAnimator == null)
        {
            Debug.LogWarning("[Start] Boss TIDAK memiliki Animator. Ini tidak apa-apa jika disengaja.");
        }
        if (leftDashPoints.Length != rightDashPoints.Length)
        {
            Debug.LogError("Setup SALAH! Jumlah LeftDashPoints harus SAMA DENGAN RightDashPoints!");
        }
    }

    public void ActivateBoss()
    {
        if (!isActive)
        {
            isActive = true;
            StartShootingPhase();
            // Tampilkan HP Bar saat bos aktif
            if (healthBar != null) healthBar.gameObject.SetActive(true);

            StartShootingPhase();
        }
    }

    // === Fase Menembak ===
    void StartShootingPhase()
    {
        // (Fungsi ini tidak berubah)
        currentState = BossState.Shooting;
        phaseTimer = phaseSwitchTime;
        if (bossAnimator != null) { bossAnimator.enabled = true; }
        transform.position = startPosition;
        StartCoroutine(FadeBoss(1f));
        InvokeRepeating(nameof(ShootAtPlayer), 1f, shootInterval);
    }

    void Update()
    {
        // (Fungsi ini tidak berubah)
        if (!isActive) return;
        if (currentState == BossState.Shooting)
        {
            phaseTimer -= Time.deltaTime;
            if (phaseTimer <= 0)
            {
                StartDashPhase();
            }
        }
    }

    // === Fase Dash ===
    void StartDashPhase()
    {
        // (Fungsi ini tidak berubah)
        currentState = BossState.Dashing;
        CancelInvoke(nameof(ShootAtPlayer));
        if (bossAnimator != null) { bossAnimator.enabled = false; }
        StartCoroutine(DashAttackSequence());
    }

    // === Urutan Dash ===
    IEnumerator DashAttackSequence()
    {
        // (Fungsi ini tidak berubah)
        if (leftDashPoints.Length == 0)
        {
            Debug.LogError("Dash Points belum di-setup! Membatalkan dash.");
            StartShootingPhase();
            yield break;
        }
        yield return StartCoroutine(FadeBoss(0f));
        bool startFromRight = (Random.Range(0, 2) == 0);
        for (int i = 0; i < numberOfDashes; i++)
        {
            int pathIndex = Random.Range(0, leftDashPoints.Length);
            Transform startPoint;
            Transform endPoint;
            if (startFromRight)
            {
                startPoint = rightDashPoints[pathIndex];
                endPoint = leftDashPoints[pathIndex];
            }
            else
            {
                startPoint = leftDashPoints[pathIndex];
                endPoint = rightDashPoints[pathIndex];
            }
            yield return StartCoroutine(PerformDash(startPoint, endPoint));
            startFromRight = !startFromRight;
        }
        StartShootingPhase();
    }

    // === Helper 1x Dash ===
    IEnumerator PerformDash(Transform startPoint, Transform endPoint)
    {
        // (Fungsi ini tidak berubah)
        GameObject warning = Instantiate(warningIndicator, startPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(warningTime);
        Destroy(warning);
        transform.position = startPoint.position;
        bossSprite.enabled = true;
        bossSprite.color = new Color(bossSprite.color.r, bossSprite.color.g, bossSprite.color.b, 1f);
        float dashStartTime = Time.time;
        float distance = Vector3.Distance(startPoint.position, endPoint.position);
        float duration = distance / dashSpeed;
        while (Time.time < dashStartTime + duration)
        {
            float t = (Time.time - dashStartTime) / duration;
            transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
            yield return null;
        }
        transform.position = endPoint.position;
        bossSprite.enabled = false;
        yield return new WaitForSeconds(0.5f);
    }

    // === Helper Fade ===
    IEnumerator FadeBoss(float targetAlpha)
    {
        // (Fungsi ini tidak berubah)
        if (bossSprite == null) { Debug.LogError("[Fade] GAGAL: SpriteRenderer null!"); yield break; }
        float startAlpha = bossSprite.color.a;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            bossSprite.color = new Color(bossSprite.color.r, bossSprite.color.g, bossSprite.color.b, newAlpha);
            yield return null;
        }
        bossSprite.color = new Color(bossSprite.color.r, bossSprite.color.g, bossSprite.color.b, targetAlpha);
        if (targetAlpha == 0) bossSprite.enabled = false;
        else bossSprite.enabled = true;
    }

    // === FUNGSI DAMAGE ===
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // (Fungsi ini tidak berubah)
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState == BossState.Dashing)
            {
                PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
                if (player != null) { player.TakeDamage(projectileDamage); }
                else { Debug.LogError("[Collision] Player tertabrak, tapi tidak punya script PlayerMovement!"); }
            }
        }
    }

    // --- BARU: Fungsi Bos Menerima Damage ---
    public void TakeDamage(int damage)
    {
        // Jangan beri damage jika bos sedang fade (nonaktif)
        if (!bossSprite.enabled) return;

        currentHealth -= damage;
        if (healthBar != null) healthBar.SetHealth(currentHealth);
        Debug.Log($"BOSS TERKENA HIT! HP tersisa: {currentHealth}");

        // Di sini Anda bisa menambahkan efek visual (misal, bos berkedip)

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- BARU: Fungsi Bos Mati ---
    void Die()
    {
        Debug.Log("BOSS TELAH DIKALAHKAN!");
        DeactivateBoss(); // Hentikan semua aksi (termasuk dash/tembak)
        if (healthBar != null) healthBar.gameObject.SetActive(false);

        // Matikan collider agar tidak bisa ditabrak lagi
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // TODO: Mainkan animasi kematian di sini
        // ...

        // Tampilkan pop-up "Game Over" sebagai "You Win" sementara
        if (gameOverManager != null)
        {
            // Tips: Anda bisa buat panel "You Win" terpisah 
            // dan memanggilnya di sini, tapi untuk sementara:
            gameOverManager.ShowGameOver();
        }

        // Hancurkan objek bos setelah 2 detik
        Destroy(gameObject, 2f);
    }

    // === Fungsi Tembak ===
    void ShootAtPlayer()
    {
        // (Fungsi ini tidak berubah)
        if (!isActive || player == null || projectilePrefab == null) return;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;
        BossProjectile bossProj = projectile.GetComponent<BossProjectile>();
        if (bossProj != null)
        {
            bossProj.Initialize(direction, projectileSpeed, projectileDamage);
        }
        else
        {
            Debug.LogError("Projectile Prefab is missing BossProjectile component!");
            Destroy(projectile);
        }
    }

    // === Fungsi Deactivate ===
    public void DeactivateBoss()
    {
        // (Fungsi ini tidak berubah)
        isActive = false;
        CancelInvoke(nameof(ShootAtPlayer));
        StopAllCoroutines();
        if (bossAnimator != null)
        {
            bossAnimator.enabled = true;
        }
    }
}