using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    // === Variabel Inspector ===
    [Header("Boss Settings")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 0.7f;

    [Tooltip("Jeda (detik) setelah bos muncul SEBELUM dia mulai menembak")]
    public float delayAfterEmerge = 1f;
    public float projectileSpeed = 6f;
    public int projectileDamage = 1;

    [Header("Phase 2: Dash Attack")]
    public float phaseSwitchTime = 10f;
    public float dashSpeed = 20f;
    public GameObject warningIndicator;
    public Transform[] leftDashPoints;
    public Transform[] rightDashPoints;
    public int numberOfDashes = 2;
    public float fadeDuration = 1f; // Ini akan jadi durasi 'Sink' (turun)
    public float warningTime = 1.5f;

    [Header("Dash Visuals")]
    [Tooltip("Multiplier ukuran bos saat dash. 0.75 = 75% ukuran asli")]
    public float dashScaleMultiplier = 0.75f;
    [Tooltip("Sorting Order bos saat dash agar di atas 'ground melayang'")]
    public int dashSortingOrder = 10;

    [Header("Sink Effect")]
    public float sinkDistance = 2f; // Seberapa jauh dia turun

    [Header("Boss Health")]
    public int maxHealth = 20;
    private int currentHealth;
    public BossHealthBar healthBar;
    private GameOverManager gameOverManager;

    // === Variabel Internal ===
    private bool isActive = false;
    private SpriteRenderer bossSprite;
    private Vector3 startPosition;
    private float phaseTimer;
    private enum BossState { Shooting, Dashing }
    private BossState currentState;

    private Animator anim; // Hanya satu variabel Animator
    private bool isFacingRight = true;
    private Vector3 originalScale;

    private Vector3 dashScale; // Ukuran kecil untuk dash
    private int originalSortingOrder; // Sorting order asli

    // =============================================
    // === FUNGSI UTAMA ===
    // =============================================

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;
        currentHealth = maxHealth;
        gameOverManager = FindObjectOfType<GameOverManager>();
        originalScale = transform.localScale;

        dashScale = originalScale * dashScaleMultiplier;
        originalSortingOrder = bossSprite.sortingOrder;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("HealthBar belum di-assign di BossController!");
        }

        if (bossSprite == null)
        {
            Debug.LogError("[Start] GAGAL: Boss Controller butuh SpriteRenderer!");
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
            if (healthBar != null) healthBar.gameObject.SetActive(true);

            if (player != null)
            {
                if (transform.position.x < player.position.x && !isFacingRight) FlipBoss();
                else if (transform.position.x > player.position.x && isFacingRight) FlipBoss();
            }
            StartShootingPhase();
        }
    }

    void StartShootingPhase()
    {
        currentState = BossState.Shooting;
        phaseTimer = phaseSwitchTime;
        if (anim != null) anim.SetBool("isDashing", false);

        StartCoroutine(EmergeFromGround());

    }
    void Update()
    {
        if (!isActive) return;

        // Jangan lakukan update jika bos 'tidak terlihat' (misal: di dalam tanah)
        if (bossSprite.enabled == false) return;

        if (currentState == BossState.Shooting && player != null)
        {
            if (transform.position.x < player.position.x && !isFacingRight) FlipBoss();
            else if (transform.position.x > player.position.x && isFacingRight) FlipBoss();
        }

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
        currentState = BossState.Dashing;
        CancelInvoke(nameof(ShootAtPlayer));
        // JANGAN ubah animasi dulu, biarkan 'idle'
        StartCoroutine(DashAttackSequence());
    }

    // === Urutan Dash ===
    IEnumerator DashAttackSequence()
    {
        // 1. Cek setup
        if (leftDashPoints.Length == 0)
        {
            Debug.LogError("Dash Points belum di-setup! Membatalkan dash.");
            StartShootingPhase();
            yield break;
        }

        // 2. Turun ke tanah (sambil tetap 'idle')
        yield return StartCoroutine(SinkIntoGround());

        // 3. SETELAH di dalam tanah, ganti animasi ke 'Dashing' (UlatMove)
        if (anim != null) anim.SetBool("isDashing", true);

        // 4. Tentukan arah mulai (50/50)
        bool startFromRight = (Random.Range(0, 2) == 0);

        // 5. Lakukan dash sebanyak numberOfDashes
        for (int i = 0; i < numberOfDashes; i++)
        {
            int pathIndex = Random.Range(0, leftDashPoints.Length);
            Transform startPoint;
            Transform endPoint;

            if (startFromRight)
            {
                startPoint = rightDashPoints[pathIndex];
                endPoint = leftDashPoints[pathIndex];
                if (isFacingRight) FlipBoss(); // Hadap Kiri
            }
            else // Mulai dari Kiri
            {
                startPoint = leftDashPoints[pathIndex];
                endPoint = rightDashPoints[pathIndex];
                if (!isFacingRight) FlipBoss(); // Hadap Kanan
            }

            // 7. Lakukan Dash (Fungsi ini akan memunculkan bos)
            yield return StartCoroutine(PerformDash(startPoint, endPoint));

            // 8. Jeda singkat antar dash
            yield return new WaitForSeconds(1f);

            // 9. Balik arah untuk dash berikutnya
            startFromRight = !startFromRight;
        }

        // 10. Selesai, kembali menembak
        StartShootingPhase();
    }
    // --- AKHIR PERBAIKAN ---

    // === Helper 1x Dash ===
    IEnumerator PerformDash(Transform startPoint, Transform endPoint)
    {
        // 1. Tampilkan Tanda Seru
        GameObject warning = Instantiate(warningIndicator, startPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(warningTime);
        Destroy(warning);

        // 2. "Pop-up" di titik awal dash
        transform.position = startPoint.position;
        bossSprite.enabled = true;
        bossSprite.color = new Color(bossSprite.color.r, bossSprite.color.g, bossSprite.color.b, 1f);

        // 2a. Atur Sorting Order ke ATAS
        bossSprite.sortingOrder = dashSortingOrder;

        // 2b. Atur Ukuran menjadi KECIL
        transform.localScale = new Vector3(
            isFacingRight ? dashScale.x : -dashScale.x,
            dashScale.y,
            dashScale.z);

        // 3. Bergerak Cepat (Dash)
        float dashStartTime = Time.time;
        float distance = Vector3.Distance(startPoint.position, endPoint.position);
        float duration = distance / dashSpeed;
        while (Time.time < dashStartTime + duration)
        {
            float t = (Time.time - dashStartTime) / duration;
            transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
            yield return null;
        }

        // 4. "Pop-off" (hilang) di titik akhir
        transform.position = endPoint.position;
        bossSprite.enabled = false;
        yield return new WaitForSeconds(0.5f);
    }

    // --- FUNGSI 'FadeBoss' LAMA DIHAPUS ---

    // --- FUNGSI BARU: Masuk ke Tanah ---
    IEnumerator SinkIntoGround()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos - new Vector3(0, sinkDistance, 0);

        while (timer < fadeDuration) // Pakai 'fadeDuration' sebagai 'sinkDuration'
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        bossSprite.enabled = false; // Sembunyikan setelah di bawah tanah
    }

    // --- FUNGSI BARU: Muncul dari Tanah ---
    IEnumerator EmergeFromGround()
    {
        // Mulai dari bawah tanah di posisi 'startPosition'
        Vector3 startPos = startPosition - new Vector3(0, sinkDistance, 0);
        Vector3 endPos = startPosition;

        // Pastikan bos di posisi awal (di bawah tanah) dan terlihat
        transform.position = startPos;
        if (!isFacingRight) FlipBoss(); // Pastikan hadap kanan
        bossSprite.enabled = true;

        bossSprite.sortingOrder = originalSortingOrder;

        float timer = 0f;
        while (timer < fadeDuration) // Pakai 'fadeDuration' sebagai 'emergeDuration'
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;

        transform.localScale = originalScale;

        // 1. Tunggu jeda tambahan sesuai yang diatur di Inspector
        yield return new WaitForSeconds(delayAfterEmerge);

        // 2. Baru mulai menembak berulang kali
        InvokeRepeating(nameof(ShootAtPlayer), 0f, shootInterval);
    }
    // --- AKHIR FUNGSI BARU ---

    // === FUNGSI DAMAGE ===
    private void OnCollisionEnter2D(Collision2D collision)
    {
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

    // --- Fungsi Bos Menerima Damage ---
    public void TakeDamage(int damage)
    {
        // Jangan beri damage jika bos sedang 'di dalam tanah' atau 'pop-off'
        if (!bossSprite.enabled) return;

        currentHealth -= damage;
        if (healthBar != null) healthBar.SetHealth(currentHealth);
        Debug.Log($"BOSS TERKENA HIT! HP tersisa: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- Fungsi Bos Mati ---
    void Die()
    {
        Debug.Log("BOSS TELAH DIKALAHKAN!");
        DeactivateBoss();
        if (healthBar != null) healthBar.gameObject.SetActive(false);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameWinner();
            SceneManager.LoadScene("epilog");
        }
        Destroy(gameObject, 2f);
    }

    void FlipBoss()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x = isFacingRight ? originalScale.x : -originalScale.x;
        transform.localScale = scaler;
    }

    // === Fungsi Tembak ===
    void ShootAtPlayer()
    {
        if (currentState != BossState.Shooting) return;

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
        isActive = false;
        CancelInvoke(nameof(ShootAtPlayer));
        StopAllCoroutines();
    }
}