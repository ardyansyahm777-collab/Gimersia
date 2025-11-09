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

    // --- PERUBAHAN DI SINI ---
    [Header("Phase 2: Dash Attack")]
    public float phaseSwitchTime = 10f;
    public float dashSpeed = 20f;
    public GameObject warningIndicator;

    [Tooltip("Daftar titik spawn di sisi KIRI arena")]
    public Transform[] leftDashPoints; // <-- Array baru

    [Tooltip("Daftar titik spawn di sisi KANAN arena (harus sejajar!)")]
    public Transform[] rightDashPoints; // <-- Array baru

    [Tooltip("Berapa kali bos akan dash bolak-balik")]
    public int numberOfDashes = 2; // Anda minta 2 (Kanan->Kiri, Kiri->Kanan)

    public float fadeDuration = 1f;
    public float warningTime = 1.5f;
    // --- AKHIR PERUBAHAN ---

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

        if (bossSprite == null)
        {
            Debug.LogError("[Start] GAGAL: Boss Controller butuh SpriteRenderer!");
        }
        if (bossAnimator == null)
        {
            Debug.LogWarning("[Start] Boss TIDAK memiliki Animator. Ini tidak apa-apa jika disengaja.");
        }

        // PENTING: Cek setup array
        if (leftDashPoints.Length != rightDashPoints.Length)
        {
            Debug.LogError("Setup SALAH! Jumlah LeftDashPoints harus SAMA DENGAN RightDashPoints!");
        }

        startPosition = transform.position;
    }

    public void ActivateBoss()
    {
        if (!isActive)
        {
            isActive = true;
            StartShootingPhase();
        }
    }

    // === Fase Menembak ===
    void StartShootingPhase()
    {
        currentState = BossState.Shooting;
        phaseTimer = phaseSwitchTime;

        if (bossAnimator != null)
        {
            bossAnimator.enabled = true;
        }

        transform.position = startPosition;
        StartCoroutine(FadeBoss(1f)); // Muncul
        InvokeRepeating(nameof(ShootAtPlayer), 1f, shootInterval);
    }

    void Update()
    {
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
        currentState = BossState.Dashing;
        CancelInvoke(nameof(ShootAtPlayer));

        if (bossAnimator != null)
        {
            bossAnimator.enabled = false;
        }

        StartCoroutine(DashAttackSequence());
    }

    // --- PERUBAHAN LOGIKA UTAMA DI SINI ---
    // === Urutan Dash (Logika Bolak-balik Baru) ===
    IEnumerator DashAttackSequence()
    {
        // 1. Cek setup
        if (leftDashPoints.Length == 0)
        {
            Debug.LogError("Dash Points belum di-setup! Membatalkan dash.");
            StartShootingPhase();
            yield break;
        }

        // 2. Menghilang
        yield return StartCoroutine(FadeBoss(0f));

        // 3. Tentukan arah dash pertama (Kiri ke Kanan / Kanan ke Kiri)
        bool startFromRight = (Random.Range(0, 2) == 0); // 50/50 chance

        // 4. Lakukan dash sebanyak numberOfDashes
        for (int i = 0; i < numberOfDashes; i++)
        {
            // 5. Pilih "jalur" (atas atau bawah) secara acak
            // Misal: leftDashPoints[0] = BawahKiri, rightDashPoints[0] = BawahKanan
            // Misal: leftDashPoints[1] = AtasKiri,  rightDashPoints[1] = AtasKanan
            int pathIndex = Random.Range(0, leftDashPoints.Length);

            // 6. Tentukan titik awal dan akhir
            Transform startPoint;
            Transform endPoint;

            if (startFromRight)
            {
                startPoint = rightDashPoints[pathIndex];
                endPoint = leftDashPoints[pathIndex];
            }
            else // Mulai dari Kiri
            {
                startPoint = leftDashPoints[pathIndex];
                endPoint = rightDashPoints[pathIndex];
            }

            // 7. Lakukan Dash
            yield return StartCoroutine(PerformDash(startPoint, endPoint));

            // 8. Balik arah untuk dash berikutnya
            startFromRight = !startFromRight;
        }

        // 9. Selesai dash, kembali ke fase menembak
        StartShootingPhase();
    }
    // --- AKHIR PERUBAHAN ---


    // === Helper 1x Dash (Tidak Berubah) ===
    IEnumerator PerformDash(Transform startPoint, Transform endPoint)
    {
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

    // === Helper Fade (Tidak Berubah) ===
    IEnumerator FadeBoss(float targetAlpha)
    {
        if (bossSprite == null)
        {
            Debug.LogError("[Fade] GAGAL: SpriteRenderer null!");
            yield break;
        }
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

    // === FUNGSI DAMAGE (Tidak Berubah) ===
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState == BossState.Dashing)
            {
                PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    player.TakeDamage(projectileDamage);
                }
                else
                {
                    Debug.LogError("[Collision] Player tertabrak, tapi tidak punya script PlayerMovement!");
                }
            }
        }
    }

    // === Fungsi Tembak (Tidak Berubah) ===
    void ShootAtPlayer()
    {
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

    // === Fungsi Deactivate (Tidak Berubah) ===
    public void DeactivateBoss()
    {
        isActive = false;
        CancelInvoke(nameof(ShootAtPlayer));
        StopAllCoroutines();
        if (bossAnimator != null)
        {
            bossAnimator.enabled = true;
        }
    }
}