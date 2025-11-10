using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour
{
    [Header("Settings")]
    public int ammoAmount = 5;
    public float respawnCooldown = 10f;

    // --- TAMBAHAN BARU UNTUK EFEK NAIK-TURUN ---
    [Header("Bobbing Effect")]
    [Tooltip("Seberapa tinggi 'lompatan' pickup")]
    public float bobHeight = 0.25f; // Naik turun sejauh 0.25 unit
    [Tooltip("Seberapa cepat 'lompatan'")]
    public float bobSpeed = 3f;

    private Vector3 startPosition; // Posisi awal untuk patokan
    // --- AKHIR TAMBAHAN ---

    // Variabel internal
    private bool isCollected = false;
    private SpriteRenderer sprite;
    private Collider2D col;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // --- TAMBAHAN BARU ---
        // Simpan posisi awal untuk patokan bobbing
        startPosition = transform.position;
        // ---

        if (sprite == null || col == null)
        {
            Debug.LogError("AmmoPickup butuh SpriteRenderer dan Collider2D!");
        }
    }

    // --- TAMBAHAN BARU: FUNGSI UPDATE ---
    void Update()
    {
        // 1. Jika sudah diambil, jangan lakukan apa-apa (diam di tempat)
        // Kita sembunyikan di Coroutine, jadi kita cek 'sprite.enabled'
        if (!sprite.enabled) return;

        // 2. Jika belum diambil, buat dia gerak naik-turun (bobbing)
        // Kita gunakan Sinus wave untuk membuat gerakan halus
        // Time.time * bobSpeed = seberapa cepat
        // * bobHeight = seberapa tinggi
        float newY = startPosition.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);

        // Terapkan posisi baru
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
    // --- AKHIR TAMBAHAN ---


    // Biarkan pickup yang mendeteksi player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.AddAmmo(ammoAmount);
                StartCoroutine(RespawnCoroutine());
            }
        }
    }

    // Coroutine untuk proses menghilang dan muncul kembali
    IEnumerator RespawnCoroutine()
    {
        isCollected = true;
        SetVisible(false);

        // --- TAMBAHAN BARU ---
        // Saat disembunyikan, kembalikan ke posisi awal
        transform.position = startPosition;
        // ---

        yield return new WaitForSeconds(respawnCooldown);

        SetVisible(true);
        isCollected = false;
    }

    // Fungsi ini dipanggil dari luar (oleh Player) saat Game Over
    public void ResetPickup()
    {
        StopAllCoroutines();

        // --- TAMBAHAN BARU ---
        // Pastikan kembali ke posisi awal juga saat reset
        transform.position = startPosition;
        // ---

        SetVisible(true);
        isCollected = false;
    }

    // Fungsi helper agar rapi
    private void SetVisible(bool visible)
    {
        if (sprite == null || col == null) return;

        sprite.enabled = visible;
        col.enabled = visible;
    }
}